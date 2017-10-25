using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Engine3D;
//using System.IO;

namespace CSharp3DEngine
{

  public partial class Form1 : Form
  {
    enum TBallState { bsMove, bsMoveToTheSide };

    const int NUM_STAIRS = 9;
    
    const int STAIR_LENGTH =150;
    const int STAIR_WIDTH = 150;
    const int STAIR_HEIGHT = 30;

    const int STAIRS_START_X = 0;
    const int STAIRS_START_Y =-60;
    const int STAIRS_START_Z = -250;

    const int STAIR_Z_TO_DISAPPEAR = -420;

    const float STAIRS_Y_FACTOR = 0.15f;

    const int NUM_SPIRALS = 5;

    const int SPIRAL_RADIUS = 15;
    const int SPIRAL_HEIGHT = 15;
    const int SPIRAL_MOVE_STEP = 2;

    const int START_SPIRAL_Z = STAIRS_START_Z - STAIR_WIDTH;
   
    const int BALL_RADIUS = 15;

    const int BALL_START_X_POS = 0;
    const int BALL_START_Y_POS = STAIRS_START_Y + STAIR_HEIGHT + BALL_RADIUS;
    const int BALL_START_Z_POS = STAIRS_START_Z;

    const int BALL_MOVE_STEP = 1;
                          
    const int BALL_SIDE_MOVE_SPEED = 20;
    const int BALL_SIDE_MOVE_LIMIT = 600;
    
    const int MOVE_DELTA = 1;
    const int MOVE_Z = -3;
    const int TURN_ANGLE  =  20;
    const int FALLING_ANGLE = 10;

    private TBallState BallState;
    private int BallSideMoveDirection;

    private int CurrentMoveX, CurrentMoveZ;
    private int FirstVisibleStair;

  //  private int CurrentRotateDelta;

    private int IndexCurrentStairBallOn;

    CObject3D Ball;
    CObject3D Diamond;

    CObject3D[] Stairs = new CObject3D[NUM_STAIRS];

    CObject3D[] Spirals = new CObject3D[NUM_SPIRALS];
  
    int[] IndexStairSpiralOn = new int[NUM_SPIRALS];
  
    int[] SpiralAngle = new int[NUM_SPIRALS];
    bool[] SpiralDirection = new bool[NUM_SPIRALS]; 
 
    CRenderContext RenderContext;
    Graphics FormCanvas;

    Random rnd = new Random();


    public Form1()
    {
      InitializeComponent();

      Ball = new CObject3D("Ball.ini");

      Stairs[0] = new CObject3D("Floor.ini");

      for (int i = 1; i < NUM_STAIRS; i++)
          Stairs[i] = new CObject3D(Stairs[0]);

      Spirals[0] = new CObject3D("Spiral.ini");

      for (int i = 1; i < NUM_SPIRALS; i++)
          Spirals[i] = new CObject3D(Spirals[0]);


      Diamond = new CObject3D("Diamond.ini");         
  
      RenderContext = new CRenderContext(ClientSize.Width, ClientSize.Height);
      FormCanvas = CreateGraphics();

      RenderContext.SetViewerPosition(0, -80, 500);
   
      Initialize();

      timer1.Enabled = true;
    }
    
  
    private void Initialize()
    {
      InitBall();
      PrepareStairs();
      PrepareSpirals();
    }

    private void InitBall()
    {
        CurrentMoveX = 0; // CurrentRotateDelta = 0;
      IndexCurrentStairBallOn = 0;
      BallSideMoveDirection = 0;

      BallState = TBallState.bsMove;
      Ball.SetPosAndAngle(BALL_START_X_POS, BALL_START_Y_POS, BALL_START_Z_POS, 0, 0, 0);

    }

    private void PrepareStairs()
    {
      FirstVisibleStair = 0;
      Stairs[0].SetPos(STAIRS_START_X, STAIRS_START_Y, STAIRS_START_Z);

      for (int i = 1; i < NUM_STAIRS; i++)
          Stairs[i].SetPos(Stairs[i - 1].GetXPos(), Stairs[i - 1].GetYPos() - STAIR_HEIGHT,
                           Stairs[i - 1].GetZPos() + STAIR_WIDTH);

      for (int i = 0; i < NUM_STAIRS; i++)
        Stairs[i].Visible = true;

      Stairs[0].SetNewFacesColor(CObject3D.ConvertRGBToInt(250, 0, 0));
      Stairs[NUM_STAIRS - 1].SetNewFacesColor(CObject3D.ConvertRGBToInt(250, 0, 0));

      Diamond.SetPos(Stairs[NUM_STAIRS - 1].GetPos());
    }

    private void PrepareSpirals()
    {
      int StairNum;

      bool[] SpiralOnStair = new bool[NUM_STAIRS];

      for (int i=0; i< NUM_STAIRS; i++)
         SpiralOnStair[i] = false;

      for (int i = 0; i < NUM_SPIRALS; i++)
      {
        do
          //choose stair, that have no spiral on it, to put new spiral  
          StairNum = rnd.Next(1, NUM_STAIRS - 1);
        while (SpiralOnStair[StairNum]);

        SpiralOnStair[StairNum] = true;
        IndexStairSpiralOn[i] = StairNum;

        SpiralAngle[i] = TURN_ANGLE * i;

        //put the spiral on the stair
        TVertex spiralPlace = new TVertex(0,0,0);
        spiralPlace.X = rnd.Next(-STAIR_LENGTH / 2 + BALL_RADIUS, STAIR_LENGTH / 2 - BALL_RADIUS);
        spiralPlace.Y = Stairs[StairNum].GetYPos() + STAIR_HEIGHT;
        spiralPlace.Z = Stairs[StairNum].GetZPos() + 20;

        Spirals[i].SetPos(spiralPlace);
        Spirals[i].Visible = true;
      }

      for (int i = 0; i < NUM_SPIRALS; i++)   
          //choose random direction for each visible spiral. true = go right  false = go left
          SpiralDirection[i] = rnd.Next(2) == 0;
        
    }

    private void RefreshDisplay()
    {             
      RenderContext.StartRender();
        
      Ball.Render(RenderContext);

      Diamond.Render(RenderContext);

      for (int i = 0; i < NUM_STAIRS; i++)
        Stairs[i].Render(RenderContext);

      for (int i=0; i< NUM_SPIRALS; i++)
        Spirals[i].Render(RenderContext);

      RenderContext.EndRender(FormCanvas);
        
    }

    private void quitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (FirstVisibleStair == NUM_STAIRS - 1)
      {
        timer1.Enabled = false;
        MessageBox.Show(" Here is the diamond you wanted ");
      }
      else
      {
        ObjectsMoveForward();
        BallCollineSpiral();

        switch (BallState)
        {
          case TBallState.bsMove: DoMoveBall();
              break;

          case TBallState.bsMoveToTheSide: DoMoveBallToSide();
              break;
        }
      }

      RefreshDisplay();
    }

    private void ObjectsMoveForward()
    {

      for (int i = FirstVisibleStair; i < NUM_STAIRS; i++)
      {
        Stairs[i].SetPos(Stairs[i].GetXPos(), Stairs[i].GetYPos() + STAIRS_Y_FACTOR, Stairs[i].GetZPos() - MOVE_DELTA);
        BallToNextStair();
      }

      if (Stairs[FirstVisibleStair].GetZPos() <= STAIR_Z_TO_DISAPPEAR)
      {
        Stairs[FirstVisibleStair].Visible = false;

        UnVisibleSpiral(FirstVisibleStair);
        FirstVisibleStair++;
      }

      MoveSpirals();

      Ball.SetPos(Ball.GetXPos(), Ball.GetYPos() + STAIRS_Y_FACTOR, Ball.GetZPos());
      Diamond.SetPos(Stairs[NUM_STAIRS - 1].GetPos());
    }

    private void UnVisibleSpiral(int indexStair)
    {
      for (int i=0; i< NUM_SPIRALS; i++)
         if (IndexStairSpiralOn[i] == indexStair)
         {
             Spirals[i].Visible = false;
             return;
         }
    }

    private void BallToNextStair()
    {
      int CurrentStairBallOn = IndexCurrentStairBallOn;
      IndexCurrentStairBallOn = ChangeCurrentStairBallOn();

      if (IndexCurrentStairBallOn > CurrentStairBallOn)
      {
         TVertex ballPos = new TVertex(Ball.GetXPos(), Ball.GetYPos(), Ball.GetZPos());
         ballPos.Y = Stairs[IndexCurrentStairBallOn].GetYPos() + STAIR_HEIGHT + BALL_RADIUS;
         ballPos.Z += 12;

         Ball.SetPos(ballPos.X, ballPos.Y, ballPos.Z);
      }
    }

    private int ChangeCurrentStairBallOn()
    {
        if (Ball.GetZPos() > (Stairs[IndexCurrentStairBallOn].GetZPos() + STAIR_WIDTH / 2))
            return (IndexCurrentStairBallOn + 1);

        return IndexCurrentStairBallOn;
    }

    private void MoveSpirals()
    {
      TVertex SpiralPos = new TVertex(0, 0, 0);

      for (int i=0; i< NUM_SPIRALS; i++)
        if (Spirals[i].Visible)
        {
           SpiralPos = Spirals[i].GetPos();

           if (SpiralDirection[i]) //spiral go right
           {
              SpiralPos.X += SPIRAL_MOVE_STEP;
              if (SpiralPos.X > STAIR_LENGTH / 2 - SPIRAL_RADIUS) //spiral go left
                 SpiralDirection[i] = false;
           }
           else //spiral go left
           {
             SpiralPos.X -= SPIRAL_MOVE_STEP;
             if (SpiralPos.X < -STAIR_LENGTH / 2 + SPIRAL_RADIUS) //spiral go right
                SpiralDirection[i] = true;
           }

           SpiralPos.Y += STAIRS_Y_FACTOR;
           SpiralPos.Z -= MOVE_DELTA;

           SpiralAngle[i] = (SpiralAngle[i] + TURN_ANGLE) % 360;
           int direction = 1;
           if (i % 2 == 1)
              direction = -1;

           Spirals[i].SetPosAndAngle(SpiralPos.X, SpiralPos.Y, SpiralPos.Z, 0, Math3D.Deg2Rad(direction * SpiralAngle[i]), 0);          
        }//for
    }

    private void BallCollineSpiral()
    {
      for (int i=0; i< NUM_SPIRALS; i++)
        if (Spirals[i].Visible)
        {
          float dist = (float)Math.Sqrt(Math3D.Sqr(Spirals[i].GetXPos() - Ball.GetXPos()) + Math3D.Sqr(Spirals[i].GetZPos() - Ball.GetZPos()));
          if (dist < (SPIRAL_RADIUS + BALL_RADIUS))
          {
            BallState = TBallState.bsMoveToTheSide;
            if (Ball.GetXPos() > Spirals[i].GetXPos())
                BallSideMoveDirection = 1;
            else
                BallSideMoveDirection = -1;
          }
        }
    }

    private void DoMoveBall()
    {
      if (CurrentMoveZ != 0 || CurrentMoveX != 0)
      {
        Ball.SetPos(Ball.GetXPos() + CurrentMoveX, Ball.GetYPos(), Ball.GetZPos() + CurrentMoveZ);
        BallToNextStair();
      }
    }

    private void DoMoveBallToSide()
    {
        timer1.Enabled = false;
        MessageBox.Show(" You lost ");
    }

    private void wireFrameModeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      wireFrameModeToolStripMenuItem.Checked = !wireFrameModeToolStripMenuItem.Checked;
      RenderContext.SetWireFrameMode(wireFrameModeToolStripMenuItem.Checked);
      RefreshDisplay();
    }

    private void orthogonalModeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      orthogonalModeToolStripMenuItem.Checked = !orthogonalModeToolStripMenuItem.Checked;
      RenderContext.SetOrthogonalMode(orthogonalModeToolStripMenuItem.Checked);
      RefreshDisplay();
    }

    private void noBackfaceCullingToolStripMenuItem_Click(object sender, EventArgs e)
    {
      noBackfaceCullingToolStripMenuItem.Checked = !noBackfaceCullingToolStripMenuItem.Checked;
      RenderContext.SetBackfaceCullingMode(!noBackfaceCullingToolStripMenuItem.Checked);
      RefreshDisplay();
    }

    private void Form1_Paint(object sender, PaintEventArgs e)
    {
      RefreshDisplay();
    }

    private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Initialize();
      timer1.Enabled = true;
    }
 
    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
    
        case Keys.Up:
          CurrentMoveZ = MOVE_DELTA; 
          break;

        case Keys.Down:
          CurrentMoveZ = -MOVE_DELTA;
          break;

        case Keys.Left:
          {
              CurrentMoveX = -MOVE_DELTA;
             // CurrentRotateDelta = -TURN_ANGLE;
          }
          break;

        case Keys.Right:
          {
              CurrentMoveX = MOVE_DELTA;
            //  CurrentRotateDelta = TURN_ANGLE;
          }
          break;
      }
    }

    private void Form1_KeyUp(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up:
            case Keys.Down:
                CurrentMoveZ = 0;
            break;

            case Keys.Left:
            case Keys.Right:
            {
                CurrentMoveX = 0;
              //  CurrentRotateDelta = 0;
            }
            break;
      }
    }

  }// class
}



