// Graphics render context

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Engine3D
{  
  // 3D rendering context class
  public class CRenderContext
  {
    // Some constants
    Color DEFAULT_BACKGROUND_COLOR = Color.White;
    Color DEFAULT_PEN_COLOR = Color.Red;
    const float DEFAULT_PERSPECTIVE_FACTOR = 600;
    const float MAX_Z_BUFFER_VALUE = -1e10f;

    Color BackgroundColor;

    Bitmap VScreen;
    Graphics VScreenCanvas;

    BitmapData InternalVScreenData;

    float[] ZBuffer;

    TMat4x4 ViewMatrix;

    Pen PenForWireFrame;

    public int Width, Height;

    // Remember also the half width and height for better effeciency
    public int HalfWidth,HalfHeight;

    bool BackfaceCullingMode = true;
    bool WireFrameMode = false;
    bool PerspectiveMode = true;

    // Constructor
    public CRenderContext(int W,int H)
    {
      VScreen = new Bitmap(W,H);
      ZBuffer = new float[W * H];

      // Prepare graphics canvases for the form and for the virtual screen
      VScreenCanvas = Graphics.FromImage(VScreen);

      BackgroundColor = DEFAULT_BACKGROUND_COLOR;
      PenForWireFrame = new Pen(DEFAULT_PEN_COLOR,1);
      ViewMatrix = TMat4x4.UnitMat();

      Width = W;
      Height = H;

      HalfWidth = Width / 2;
      HalfHeight = Height / 2;
    }

    private void ClearBuffers()
    {
      // Clear Z Buffer
      for(int i = 0; i < (Width * Height); i++)
        ZBuffer[i] = MAX_Z_BUFFER_VALUE;

      VScreenCanvas.Clear(BackgroundColor);
    }

    public void SetBackgroundColor(Color NewBackgroundColor)
    {
      BackgroundColor = NewBackgroundColor;
    }

    public TMat4x4 GetViewMat()
    {
      return ViewMatrix;
    }

    public void SetViewMat(TMat4x4 Mat)
    {
      ViewMatrix = Mat;
    }

    public float GetPerspectiveFactor()
    {
      return DEFAULT_PERSPECTIVE_FACTOR;
    }

    public void CopyToScreen(Graphics ScreenCanvas)
    {
      ScreenCanvas.DrawImage(VScreen, 0, 0);
    }

    public void StartRender()
    {
      ClearBuffers();
      StartDraw();
    }

    public void EndRender(Graphics ScreenCanvas)
    {
      EndDraw();
      CopyToScreen(ScreenCanvas);
    }

    public void DrawTriangle(T2DTriangle Triangle)
    {
      Point ScreenPoint1 = new Point(Triangle.Corner1.X + HalfWidth, HalfHeight - Triangle.Corner1.Y);
      Point ScreenPoint2 = new Point(Triangle.Corner2.X + HalfWidth, HalfHeight - Triangle.Corner2.Y);
      Point ScreenPoint3 = new Point(Triangle.Corner3.X + HalfWidth, HalfHeight - Triangle.Corner3.Y);

      VScreenCanvas.DrawLine(PenForWireFrame, ScreenPoint1, ScreenPoint2);
      VScreenCanvas.DrawLine(PenForWireFrame, ScreenPoint2, ScreenPoint3);
      VScreenCanvas.DrawLine(PenForWireFrame, ScreenPoint3, ScreenPoint1);
    }

    public TVertex GetViewDirection()
    {
      return new TVertex(0, 0, -1);
    }

    // Return true if backface culling is ON
    public bool IsBackfaceCullingMode()
    {
      return BackfaceCullingMode;
    }

    // Return true if wireframe rendering is ON
    public bool IsWireFrameMode()
    {
      return WireFrameMode;
    }

    // Return true if perspective viewing is ON
    public bool IsPerspectiveMode()
    {
      return PerspectiveMode;
    }

    public void SetBackfaceCullingMode(bool Mode)
    {
      BackfaceCullingMode = Mode;
    }

    public void SetWireFrameMode(bool Mode)
    {
      WireFrameMode = Mode;
    }

    public void SetOrthogonalMode(bool Mode)
    {
      PerspectiveMode = !Mode;
    }

    public void SetViewerPosition(float X,float Y,float Z)
    {
      ViewMatrix.SetTrans(X,Y,Z);
    }

    private void StartDraw()
    {
      if (!WireFrameMode)
        InternalVScreenData = VScreen.LockBits(new Rectangle(0, 0, VScreen.Width, VScreen.Height),
                                               ImageLockMode.ReadWrite,
                                               PixelFormat.Format32bppArgb);
    }

    public System.IntPtr GetLinePtr(int LineNum)
    {
      unsafe
      {
        return (System.IntPtr)((int*)InternalVScreenData.Scan0 + InternalVScreenData.Stride / 4 * LineNum);
      }
    }

    public float[] GetZBuffer()
    {
      return ZBuffer;
    }

    private void EndDraw()
    {
      if(!WireFrameMode)
        VScreen.UnlockBits(InternalVScreenData);
    }
  }
}
