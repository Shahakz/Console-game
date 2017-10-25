// 3D object class

using System;
using System.Collections;
using System.Drawing;

namespace Engine3D
{
  // Custom exception class for TObject3D
  public class EObject3D : System.ApplicationException
  {
    public EObject3D(string message) : base(message) { }
  }

  struct TFace3D
  {
    // Indices to the vertex table
    public int AIndex, BIndex, CIndex;

    // Face normal
    public TVertex Normal;

    // Face color
    public Color Color;
  }

  // 3D object 
  public class CObject3D
  {
    const float THRESHOLD_FOR_CULLING = -0.18f;
    const float POLYGON_Z_MIN_VALUE = 0.001f;
    const int POLYGON_COORD_MAX_VALUE = 1000;
    const float ORTHO_PROJECTION_SCALING = 0.6f;
    const float MIN_LIGHT_LEVEL = 0.4f;

    // Vertex table
    private TVertex[] VertexTable;

    // Faces table
    private TFace3D[] FaceTable;

    private TMat4x4 ObjectMat;
    private TMat4x4 CombinedMatrix;

    // Array of child objects and their axes
    private ArrayList Children = new ArrayList();
    private ArrayList ChildrenAxes = new ArrayList();

    // Reference to my father object
    private CObject3D Father;

    private TVertex OriginalAxisPointA, OriginalAxisPointB;
    private TVertex TransformedAxisPointA, TransformedAxisPointB;

    private TVertex CurrentPos;
		private TVertex CurrentRot;

    private int Orientation = 0;
    private int OrientationOffset = 0;

    // The object is visible by default
    public bool Visible = true;

    // Convert RGB color to integer
    static public int ConvertRGBToInt(int R,int G,int B)
    {
      return (R | (G << 8) | (B << 16));
    }

    // Constructor
    public CObject3D(string FileName)
    {
      ObjectMat.SetUnit();
      SetMatrix(ObjectMat);
      LoadFromFile(FileName);
    }

    // Copy constructor
    public CObject3D(CObject3D CopyFrom)
    {
      Assign(CopyFrom);
    }

    public void Assign(CObject3D CopyFrom)
    {
      ObjectMat = CopyFrom.ObjectMat;

      VertexTable = new TVertex[CopyFrom.VertexTable.Length];
      Array.Copy(CopyFrom.VertexTable, VertexTable, VertexTable.Length);

      FaceTable = new TFace3D[CopyFrom.FaceTable.Length];
      Array.Copy(CopyFrom.FaceTable, FaceTable, FaceTable.Length);

      ObjectMat = CopyFrom.ObjectMat;
      CombinedMatrix = CopyFrom.CombinedMatrix;

      Children = CopyFrom.Children;
      ChildrenAxes = CopyFrom.ChildrenAxes;
      Father = CopyFrom.Father;
      OriginalAxisPointA = CopyFrom.OriginalAxisPointA;
      OriginalAxisPointB = CopyFrom.OriginalAxisPointB;
    }

    // Load the object with data from a 3D file
    private void LoadFromFile(string FileName)
    {
      CIniReader IniReader = new CIniReader(FileName);

      if (IniReader.FindSection("Header"))
      {
        // Verify that it is a 3D object ini file
        if (IniReader.GetStringValue("Signature", "") != "3D File Format")
          throw new EObject3D("File \"" + FileName + "\" is not a 3D ini file!");

        // Get the number of vertices and faces
        int VertexNum = IniReader.GetIntValue("VertexNum", -1);
        if (VertexNum == -1)
          throw new EObject3D("File \"" + FileName + "\" does not define the vertices number");

        int FaceNum = IniReader.GetIntValue("FaceNum", -1);
        if (FaceNum == -1)
          throw new EObject3D("File \"" + FileName + "\" does not define the faces number");

        // Allocate internal tables
        VertexTable = new TVertex[VertexNum];
        FaceTable = new TFace3D[FaceNum];
      }
      else
        throw new EObject3D("Header cannot be found in file \"" + FileName + "\"");

      // Read vertices
      if (IniReader.FindSection("Vertex"))
      {
        for (int i = 0; i < VertexTable.Length; i++)
        {
          string CurrKey, CurrValue;
          if (!IniReader.GetNextEntry(out CurrKey, out CurrValue))
            throw new EObject3D("Vertex not found in file \"" + FileName + "\"");

          string[] SplittedStr = CurrValue.Split(',');

          if (SplittedStr.Length != 3)
            throw new EObject3D("Invalid vertex definition in file \"" + FileName + "\"");

          VertexTable[i].X = (float)Convert.ToDouble(SplittedStr[0]);
          VertexTable[i].Y = (float)Convert.ToDouble(SplittedStr[1]);
          VertexTable[i].Z = (float)Convert.ToDouble(SplittedStr[2]);
        }
      }
      else
        throw new EObject3D("Vertices definitions cannot be found in file \"" + FileName + "\"");

      // Read faces
      if (IniReader.FindSection("Faces"))
      {
        for (int i = 0; i < FaceTable.Length; i++)
        {
          string CurrKey, CurrValue;
          if (!IniReader.GetNextEntry(out CurrKey, out CurrValue))
            throw new EObject3D("Face not found in file \"" + FileName + "\"");

          string[] SplittedStr = CurrValue.Split(',');

          if (SplittedStr.Length != 7)
            throw new EObject3D("Invalid face definition in file \"" + FileName + "\"");

          FaceTable[i].AIndex = Convert.ToInt32(SplittedStr[0]);
          FaceTable[i].BIndex = Convert.ToInt32(SplittedStr[1]);
          FaceTable[i].CIndex = Convert.ToInt32(SplittedStr[2]);

          FaceTable[i].Normal.X = (float)Convert.ToDouble(SplittedStr[3]);
          FaceTable[i].Normal.Y = (float)Convert.ToDouble(SplittedStr[4]);
          FaceTable[i].Normal.Z = (float)Convert.ToDouble(SplittedStr[5]);

          // Break into R,G,B and initialize the color struct
          int RGBColor = Convert.ToInt32(SplittedStr[6]);
          Color TmpColor = Color.FromArgb(RGBColor);
          
          FaceTable[i].Color = TmpColor;
        }
      }
      else
        throw new EObject3D("Faces definitions cannot be found in file \"" + FileName + "\"");
    }

    public void SetMatrix(TMat4x4 Mat)
    {
      ObjectMat = Mat;

      // Transform myself
      CombinedMatrix = ObjectMat.MulMat(GetFatherMat());

      TransformAxis(ObjectMat);

      // Transform all child objects
      for (int i = 0; i < Children.Count; i++)
        ((CObject3D)Children[i]).SetMatrix(((CObject3D)Children[i]).ObjectMat);
    }

    public TMat4x4 GetMatrix()
    {
      return CombinedMatrix;
    }

    public TVertex TransformVertex(TVertex v)
    {
      return CombinedMatrix.MulVertex(v);
    }

    public void SetPos(float X, float Y, float Z)
    {
      CurrentPos.Set(X, Y, Z);

      TMat4x4 TransMat = TMat4x4.ZeroMat();
      TransMat.SetTrans(X, Y, Z);
      SetMatrix(TransMat);
    }

    public void SetPos(TVertex NewPos)
    {
      CurrentPos = NewPos;

      TMat4x4 TransMat = TMat4x4.ZeroMat();
      TransMat.SetTrans(NewPos.X, NewPos.Y, NewPos.Z);
      SetMatrix(TransMat);
    }

    public TVertex GetPos()
    {
      return CurrentPos;
    }

    public float GetXPos()
    {
      return CurrentPos.X;
    }

    public float GetYPos()
    {
      return CurrentPos.Y;
    }

    public float GetZPos()
    {
      return CurrentPos.Z;
    }

		public TVertex GetRot()
		{
			return CurrentRot;
		}

		public float GetXRot()
		{
			return CurrentRot.X;
		}

		public float GetYRot()
		{
			return CurrentRot.Y;
		}

		public float GetZRot()
		{
			return CurrentRot.Z;
		}

    public void SetPosAndAngle(float X, float Y, float Z, float A, float B, float C)
    {
      TMat4x4 TransMat = TMat4x4.ZeroMat();
      TMat4x4 RotMatX = TMat4x4.ZeroMat();
      TMat4x4 RotMatY = TMat4x4.ZeroMat();
      TMat4x4 RotMatZ = TMat4x4.ZeroMat();

      CurrentPos.Set(X, Y, Z);
			CurrentRot.Set(A, B, C);

      TransMat.SetTrans(X, Y, Z);
      RotMatX.SetRotateX(A);
      RotMatY.SetRotateY(B);
      RotMatZ.SetRotateZ(C);
        
      SetMatrix(RotMatX.MulMat(RotMatY).MulMat(RotMatZ).MulMat(TransMat));
    }

    public void SetOrientation(int NewOrientation)
    {
      Orientation = NewOrientation;
    }

    public void SetOrientationOffset(int NewOrientationOffset)
    {
      OrientationOffset = NewOrientationOffset;
    }

    public int GetOrientation()
    {
      return Orientation;
    }

    public TVertex FindNewPosXZ(float DeltaStep)
    {
      float AngleInRad = Math3D.Deg2Rad(Orientation);

      TVertex Result = new TVertex(CurrentPos.X + (float)Math.Sin(AngleInRad) * DeltaStep,
                                   CurrentPos.Y,
                                   CurrentPos.Z + (float)Math.Cos(AngleInRad) * DeltaStep);
      return Result;
    }

    public void GotoNewPosXZ(TVertex NewPos)
    {
      TMat4x4 TransMat = TMat4x4.ZeroMat();
      TMat4x4 RotMatY = TMat4x4.ZeroMat();

      CurrentPos = NewPos;

      TransMat.SetTrans(NewPos.X, NewPos.Y, NewPos.Z);
      RotMatY.SetRotateY(Math3D.Deg2Rad(Orientation + OrientationOffset));

      SetMatrix(RotMatY.MulMat(TransMat));
    }

    private TMat4x4 GetFatherMat()
    {
      if (Father != null)
        return Father.CombinedMatrix;

      return TMat4x4.UnitMat();
    }

    private void TransformAxis(TMat4x4 Mat)
    {
      TransformedAxisPointA = Mat.MulVertex(OriginalAxisPointA);
      TransformedAxisPointB = Mat.MulVertex(OriginalAxisPointB);
    }

    public void AddChildObject(CObject3D ChildObj, TVertex AxisPointA, TVertex AxisPointB)
    {
      Children.Add(ChildObj);
      ChildObj.Father = this;
      ChildObj.OriginalAxisPointA = AxisPointA;
      ChildObj.TransformedAxisPointA = AxisPointA;
      ChildObj.OriginalAxisPointB = AxisPointB;
      ChildObj.TransformedAxisPointB = AxisPointB;
    }

    public void RotateOnAxis(float Angle)
    {
      TMat4x4 ArbitraryMat = TMat4x4.ZeroMat();

      ArbitraryMat.SetArbitraryRot(TransformedAxisPointA, TransformedAxisPointB, Angle);
      SetMatrix(ArbitraryMat);
    }

    public void SetNewFacesColor(int NewColor)
    {
      Color C = Color.FromArgb(NewColor);
      
      for (int i = 0; i < FaceTable.Length; i++)
        FaceTable[i].Color = C;
    }

    private void CalculatePolygonFactors(TVertex Normal, TVertex PointOnPlane, float PerspFactor,out float M,out float N,out float K)
    {
      float A, B, C, D;

      A = Normal.X;
      B = Normal.Y;
      C = Normal.Z;
      D = Math3D.DotProduct(Normal, PointOnPlane);

      if (D != 0)
      {
        M = A / (D * PerspFactor);
        N = B / (D * PerspFactor);
        K = C / D;
      }
      else
      {
        M = 0;
        N = 0;
        K = 0;
      }
    }

    private bool TestForBackClipping(TVertex V1, TVertex V2, TVertex V3)
    {
      return !((V1.Z < 0) && (V2.Z < 0) && (V3.Z < 0));
    }

    private TMat4x4 GetTransformMatrix(CRenderContext Context)
    {
      return CombinedMatrix.MulMat(Context.GetViewMat());
    }

    private void RenderChildObjects(CRenderContext Context)
    {
      for(int i = 0; i < Children.Count; i++)
        ((CObject3D)Children[i]).Render(Context);
    }

    byte ClipColorChn(int x, float LightLevel)
    {
      int v = (int)(x * LightLevel + 0.5);

      if (v > 255)
        return 255;

      return (byte)v;
    }

    // Perform rendering on a specific context
    public void Render(CRenderContext Context)
    {  
      if(!Visible)
        return;

      // Calculate transform matrix
      TMat4x4 Mat = GetTransformMatrix(Context);

      // Calculate the inverse matrix
      TMat4x4 InverseMat = Mat.FindInverseMat();

      // Clear translation effect
      InverseMat.ClearTranslation();

      // Calculate the viewer position in object coordinates
      TVertex ViewDirection = Context.GetViewDirection();

      TVertex ViewerPosInObjCoords = InverseMat.MulVertex(ViewDirection);

      float PerspectiveFactor = Context.GetPerspectiveFactor();

      Color IlluminatedFaceColor;

      foreach(TFace3D Face in FaceTable)
      {
        bool FaceVisible;
        float FaceDotProd = 0;

        if(!Context.IsBackfaceCullingMode())
          // No backface culling, the face is always visible
          FaceVisible = true;
        else
        {
          // Do backface culling
          FaceDotProd  = Math3D.DotProduct(ViewerPosInObjCoords,Face.Normal);

          // Perspective mode
          if(Context.IsPerspectiveMode())
            // Remember if the face is visible
            FaceVisible = (FaceDotProd > THRESHOLD_FOR_CULLING);
          else
            FaceVisible = (FaceDotProd > 0);
        }
  
        if(FaceVisible)
        {
          // Get current face vertices and transform to world coordinates
          TVertex[] FaceVertex = new TVertex[3] {Mat.MulVertex(VertexTable[Face.AIndex]),
                                                 Mat.MulVertex(VertexTable[Face.BIndex]),
                                                 Mat.MulVertex(VertexTable[Face.CIndex])};

          Point[] ScreenCoords = new Point[3];

          if(TestForBackClipping(FaceVertex[0],FaceVertex[1],FaceVertex[2]))
          {
            bool PreventFaceDraw = false;

            // Perspective mode
            if(Context.IsPerspectiveMode())
            {              
              // Transform the from world coordinates to screen coordinates
              for(int i = 0; i < 3; i++)
              {
                if(Math.Abs(FaceVertex[i].Z) < POLYGON_Z_MIN_VALUE)
                {
                  PreventFaceDraw = true;
                  break;
                }

                ScreenCoords[i].X = (int)(FaceVertex[i].X * PerspectiveFactor / FaceVertex[i].Z);
                ScreenCoords[i].Y = (int)(FaceVertex[i].Y * PerspectiveFactor / FaceVertex[i].Z);

                if((Math.Abs(ScreenCoords[i].X) > POLYGON_COORD_MAX_VALUE) || (Math.Abs(ScreenCoords[i].Y) > POLYGON_COORD_MAX_VALUE))
                {
                  PreventFaceDraw = true;
                  break;
                }
              }
            } 
            else
            // Orthogonal projection
            {
              PreventFaceDraw = false;

              // Transform the from world coordinates to screen coordinates
              for(int i =0; i < 3; i++)
              {                
                ScreenCoords[i].X = (int)(FaceVertex[i].X / ORTHO_PROJECTION_SCALING);
                ScreenCoords[i].Y = (int)(FaceVertex[i].Y / ORTHO_PROJECTION_SCALING);
              }
            }

            if(!PreventFaceDraw)
            {
              T2DTriangle ScreenTriangle = new T2DTriangle(ScreenCoords[0],ScreenCoords[1],ScreenCoords[2]);

              if(Context.IsWireFrameMode())
              {                
                Context.DrawTriangle(ScreenTriangle);
              }
              else
              // Filled triangles mode
              {
                float LightLevel = FaceDotProd + MIN_LIGHT_LEVEL;

                // Clip to maximum light level
                if(LightLevel > 1.0)
                  LightLevel = 1.0f;

                // Calculate face color
                int ARGBColor = ClipColorChn(Face.Color.R,LightLevel);
                ARGBColor |= ClipColorChn(Face.Color.G,LightLevel) << 8;
                ARGBColor |= ClipColorChn(Face.Color.B,LightLevel) << 16;

                // Set maximum alpha channel value
                ARGBColor = (int)((uint)ARGBColor | 0xff000000);

                IlluminatedFaceColor = Color.FromArgb(ARGBColor);

                // Calculate factors for the polygon filling routine
                TVertex FaceNormal = Math3D.CalcFaceNormal(FaceVertex[0], FaceVertex[1], FaceVertex[2]);

                float M, N, K;
                CalculatePolygonFactors(FaceNormal, FaceVertex[1], PerspectiveFactor, out M, out N, out K);
                TriangleFiller.DrawFilledTriangle(ScreenTriangle, Context, IlluminatedFaceColor, M, N, K);
              }
            }
          }
        }    
      }

      RenderChildObjects(Context);
    }
  }
}
