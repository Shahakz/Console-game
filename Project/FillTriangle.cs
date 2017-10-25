// 3D object class

using System;
using System.Drawing;

namespace Engine3D
{
  // 2D triangle type
  public struct T2DTriangle
  {
    public Point Corner1;
    public Point Corner2;
    public Point Corner3;

    // Constructor
    public T2DTriangle(Point C1,Point C2,Point C3)
    {
      Corner1 = C1;
      Corner2 = C2;
      Corner3 = C3;
    }
  }

  class TriangleFiller
  {
    // Describes the beginning and ending X coordinates of a single horizontal line
    struct THLine 
    {
      public int XStart,XEnd;

      THLine(int Start,int End)
      {
        XStart = Start;
        XEnd = End;
      }
    }

    // Advances an index by one vertex forward through the vertex list, wrapping at the end of the list, return the new vertex index
    private static int IndexForward(int Index)
    {
      return (Index + 1) % 3;
    }

    // Advances an index by one vertex backward through the vertex list, wrapping at the start of the list, return the new vertex index
    private static int IndexBackward(int Index)
    {
      return (Index - 1 + 3) % 3;
    }

    // Scan converts an edge from (X1,Y1) to (X2,Y2), not including the
    // point at (X2,Y2). This avoids overlapping the end of one line with
    // the start of the next, and causes the bottom scan line of the
    // polygon not to be drawn. If SkipFirst !!= 0, the point at (X1,Y1)
    // isn't drawn. For each scan line, the pixel closest to the scanned
    // line without being to the left of the scanned line is chosen}
    private static void ScanEdge(int X1,int Y1,int X2,int Y2,int SetXStart,int SkipFirst,ref int EdgePointIndex,
                                 THLine[] HLines,int HalfHeight)
    {
      // Calculate X and Y lengths of the line and the inverse slope
      int DeltaX = X2 - X1;

      // guard against 0-length and horizontal edges
      int DeltaY = Y2 - Y1;
      if(DeltaY <= 0) 
        return;

      float InverseSlope = (float)DeltaX / (float)DeltaY;

      // Store the X coordinate of the pixel closest to but not to the
      // left of the line for each Y coordinate between Y1 and Y2, not
      // including Y2 and also not including Y1 if SkipFirst <> 0}
      int WorkingEdgePointIndex = EdgePointIndex;

      int StartY = Y1 + SkipFirst;
      int EndY = Y2 - 1;

      for(int Y = StartY; Y <= EndY; Y++)
      {
        // Store the X coordinate in the appropriate edge list
        if(SetXStart == 1)
          HLines[WorkingEdgePointIndex].XStart = X1 + (int)((Y - Y1) * InverseSlope + 1.0);
        else
          HLines[WorkingEdgePointIndex].XEnd = X1 + (int)((Y-Y1) * InverseSlope + 1.0) + 1;

        WorkingEdgePointIndex++;
      }

      // advance caller's ptr
      EdgePointIndex = WorkingEdgePointIndex;
    }

    // Advances the index by one vertex either forward or backward through the vertex list, wrapping at either end of the list
    private static int IndexMove(int Index, int Direction, int MaxLength)
    {
      if(Direction > 0)
        return (Index + 1) % MaxLength;
      
      return (Index - 1 + MaxLength) % MaxLength;
    }

    private static void DrawHorizontalLineList(CRenderContext Context,THLine[] HLines,int YStart,int ARGBColor, float M, float N, float K)
    {
      int HalfWidth = Context.HalfWidth;
      int HalfHeight = Context.HalfHeight;

      for(int i = 0; i < HLines.Length; i++)
      {
        int XLineStart = HLines[i].XStart;
        int XLineEnd = HLines[i].XEnd;
        int y = i + YStart;
        int YLine = HalfHeight - y;

        // Draw only lines that in the screen Y range
        if((YLine >= 0) && (YLine < Context.Height))
        {
          // Convert logical 2D coordinates to absolute 2D screen coordinates
          int ScreenXStart = HalfWidth + XLineStart;
          int ScreenXEnd = HalfWidth + XLineEnd;

          // Check if the line is outside the screen
          if((ScreenXStart >= Context.Width) || (ScreenXEnd < 0))
            continue;

          // Clip the horizontal line
          if(ScreenXStart < 0)
            XLineStart = -HalfWidth;

          if(ScreenXEnd >= Context.Width)
            XLineEnd = HalfWidth - 1;

          unsafe
          {
            // Get pointers to the line start
            int* PixelPtr = (int*)Context.GetLinePtr(YLine) + HalfWidth + XLineStart;

            float[] ZBuffer = Context.GetZBuffer();
            int PosInZBuff = YLine * Context.Width + HalfWidth + XLineStart;

            for (int x = XLineStart; x <= XLineEnd; x++)
            {
              float OneDivZ = M * x + N * y + K;

              if (OneDivZ > ZBuffer[PosInZBuff])
              {
                *PixelPtr = ARGBColor;
                ZBuffer[PosInZBuff] = OneDivZ;
              }

              PixelPtr++;
              PosInZBuff++;
            }
          }
        }
      }
    }

    // Draw a filled triangle on a given rendering context
    public static void DrawFilledTriangle(T2DTriangle Triangle, CRenderContext Context, Color FillColor, float M, float N, float K)
    {
      // Scan the list to find the top and bottom of the polygon
      int MinIndexL = 0;
      int MaxIndex = 0;
      int MinPoint_Y = Triangle.Corner1.Y;
      int MaxPoint_Y = MinPoint_Y;

      int CurrentIndex, PreviousIndex;

      // Create a temporary array for triangle corner points
      Point[] Points = new Point[3]{Triangle.Corner1,Triangle.Corner2,Triangle.Corner3};

      for(int i = 1; i < 3; i++)
      {
        if(Points[i].Y < MinPoint_Y)
        {
          // new top
          MinPoint_Y = Points[i].Y;
          MinIndexL = i;
        }
        else
          if(Points[i].Y > MaxPoint_Y)
          {
            // new bottom
            MaxPoint_Y = Points[i].Y;
            MaxIndex = i;
          }
      }

      if (MinPoint_Y == MaxPoint_Y)
        // Triangle is 0-height
        return;

      // Scan in ascending order to find the last top-edge point
      int MinIndexR = MinIndexL;
      while(Points[MinIndexR].Y == MinPoint_Y)
        MinIndexR = IndexForward(MinIndexR);

      // back up to last top-edge point
      MinIndexR = IndexBackward(MinIndexR);

      // Now scan in descending order to find the first top-edge point
      while(Points[MinIndexL].Y == MinPoint_Y)
        MinIndexL = IndexBackward(MinIndexL);

      // back up to first top-edge point
      MinIndexL = IndexForward(MinIndexL);

      // Figure out which direction through the vertex list from the top
      // vertex is the left edge and which is the right}
      int LeftEdgeDir = -1; // {assume left edge runs down thru vertex list

      int TopIsFlat;

      if(Points[MinIndexL].X != Points[MinIndexR].X)
        TopIsFlat = 1;
      else 
        TopIsFlat = 0;

      // If the top is flat, just see which of the ends is leftmost
      if(TopIsFlat == 1)
      {
        if(Points[MinIndexL].X > Points[MinIndexR].X)
        {
          LeftEdgeDir = 1;  // left edge runs up through vertex list
          int Temp = MinIndexL;   // swap the indices so MinIndexL
          MinIndexL = MinIndexR;   //points to the start of the left
          MinIndexR = Temp;      // edge, similarly for MinIndexR
        }
      }
      else
      {
        // Point to the downward end of the first line of each of the
        // two edges down from the top}
        int NextIndex = MinIndexR;
        NextIndex = IndexForward(NextIndex);
        PreviousIndex = MinIndexL;
        PreviousIndex = IndexBackward(PreviousIndex);

        // Calculate X and Y lengths from the top vertex to the end of
        // the first line down each edge; use those to compare slopes
        // and see which line is leftmost
        int DeltaXN = Points[NextIndex].X - Points[MinIndexL].X;
        int DeltaYN = Points[NextIndex].Y - Points[MinIndexL].Y;
        int DeltaXP = Points[PreviousIndex].X - Points[MinIndexL].X;
        int DeltaYP = Points[PreviousIndex].Y - Points[MinIndexL].Y;

        if((DeltaXN * DeltaYP - DeltaYN * DeltaXP) < 0)
        {
          LeftEdgeDir = 1;  // left edge runs up through vertex list
          int Temp = MinIndexL;   // swap the indices so MinIndexL
          MinIndexL = MinIndexR;  // points to the start of the left
          MinIndexR = Temp;  // edge, similarly for MinIndexR
        }
      }

      // Set the # of scan lines in the polygon, skipping the bottom edge
      // and also skipping the top vertex if the top isn't flat because
      // in that case the top vertex has a right edge component, and set
      // the top scan line to draw, which is likewise the second line of
      // the polygon unless the top is flat}
      int HLineListLength = MaxPoint_Y - MinPoint_Y - 1 + TopIsFlat;
      if(HLineListLength <= 0)
        // there's nothing to draw, so we're done
        return;

      THLine[] HLineList = new THLine[HLineListLength];

      int HLineListYStart = MinPoint_Y + 1 - TopIsFlat;

      // Scan the left edge and store the boundary points in the list
      // Initial pointer for storing scan converted left-edge coords
      int EdgePointPtr = 0;

      // Start from the top of the left edge
      CurrentIndex = MinIndexL;
      PreviousIndex = MinIndexL;

      // Skip the first point of the first line unless the top is flat;
      // if the top isn't flat, the top vertex is exactly on a right
      // edge and isn't drawn}
      int SkipFirst = 1 - TopIsFlat;

      // Scan convert each line in the left edge from top to bottom
      do
      {
        CurrentIndex = IndexMove(CurrentIndex,LeftEdgeDir,3);

        ScanEdge(Points[PreviousIndex].X, Points[PreviousIndex].Y,
                 Points[CurrentIndex].X,  Points[CurrentIndex].Y, 1,
                 SkipFirst, ref EdgePointPtr, HLineList, Context.HalfHeight);

        PreviousIndex = CurrentIndex;
        SkipFirst = 0;   // scan convert the first point from now on
      } while(CurrentIndex != MaxIndex);

      // Scan the right edge and store the boundary points in the list
      EdgePointPtr = 0;
      CurrentIndex = MinIndexR;
      PreviousIndex = MinIndexR;
      SkipFirst = 1 - TopIsFlat;

      // Scan convert the right edge, top to bottom. X coordinates are
      // adjusted 1 to the left, effectively causing scan conversion of
      // the nearest points to the left of but not exactly on the edge}
      do
      {
        CurrentIndex = IndexMove(CurrentIndex,-LeftEdgeDir,3);

        ScanEdge(Points[PreviousIndex].X - 1, Points[PreviousIndex].Y,
                 Points[CurrentIndex].X - 1,  Points[CurrentIndex].Y, 0,
                 SkipFirst, ref EdgePointPtr, HLineList, Context.HalfHeight);

        PreviousIndex = CurrentIndex;
        SkipFirst = 0;  // scan convert the first point from now on
      } while(CurrentIndex != MaxIndex);

      // {Draw the line list representing the scan converted polygon}
      DrawHorizontalLineList(Context, HLineList, HLineListYStart, FillColor.ToArgb(), M, N, K);
    }      
  }
}
