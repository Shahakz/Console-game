// 3D matrix class and 3D math 

using System;

namespace Engine3D
{
  // 4 x 4 matrix type
  public struct TMat4x4
  {
    private float m00, m01, m02, m03;
    private float m10, m11, m12, m13;
    private float m20, m21, m22, m23;
    private float m30, m31, m32, m33;

    // Special function for initializing a matrix with zeroes
    public static TMat4x4 ZeroMat()
    {      
      TMat4x4 Result;
      Result.m00 = Result.m01 = Result.m02 = Result.m03 = 0;
      Result.m10 = Result.m11 = Result.m12 = Result.m13 = 0;
      Result.m20 = Result.m21 = Result.m22 = Result.m23 = 0;
      Result.m30 = Result.m31 = Result.m32 = Result.m33 = 0;
      return Result;
    }

    // Return a unit matrix
    public static TMat4x4 UnitMat()
    {
      TMat4x4 Result;
      Result.m01 = Result.m02 = Result.m03 = 0;
      Result.m10 = Result.m12 = Result.m13 = 0;
      Result.m20 = Result.m21 = Result.m23 = 0;
      Result.m30 = Result.m31 = Result.m32 = 0;
      Result.m00 = Result.m11 = Result.m22 = Result.m33 = 1;
      return Result;
    }

    // Initialize the matrix with unit mat
    public void SetUnit()
    {
      this = UnitMat();
    }

    public void SetTrans(float x, float y, float z)
    {
      SetUnit();
      m30 = x;
      m31 = y;
      m32 = z;
    }

    public void SetRotateX(float angle)
    {
      SetUnit();
      m11 = (float)Math.Cos(angle);
      m22 = m11;
      m12 = (float)Math.Sin(angle);
      m21 = -m12;
    }

    public void SetRotateY(float angle)
    {
      SetUnit();
      m00 = (float)Math.Cos(angle);
      m22 = m00;
      m20 = (float)Math.Sin(angle);
      m02 = -m20;
    }

    public void SetRotateZ(float angle)
    {
      SetUnit();
      m00 = (float)Math.Cos(angle);
      m11 = m00;
      m01 = (float)Math.Sin(angle);
      m10 = -m01;
    }

    public void SetScale(float sx,float sy,float sz)
    {
      SetUnit();
      m00 = sx;
      m11 = sy;
      m22 = sz;
    }

    public void SetXYMirror()
    {
      SetUnit();
      m22 = -1;
    }

    public void SetXZMirror()
    {
      SetUnit();
      m11 = -1;
    }

    public void SetYZMirror()
    {
      SetUnit();
      m00 = -1;
    }

    // Set rotation matrix around arbitrary axis
    public void SetArbitraryRot(TVertex P1, TVertex P2,float Angle)
    {
      TMat4x4 MTra = ZeroMat(), MRo_X = ZeroMat(), MRo_Y = ZeroMat(), MRo_Z = ZeroMat(), TempM = ZeroMat();
      float D;

      // Find the direction cosines of the arbitrary axis P1-->P2 .
      // The direction cosines will be in C 
      TVertex C = Math3D.CalcNormalizedVec(P1,P2);

      D = (float)Math.Sqrt(Math3D.Sqr(C.Y) + Math3D.Sqr(C.Z));

      // Special case for the X axis
      if(D == 0)
      {
        MTra.SetTrans(-P1.X, -P1.Y, -P1.Z);
        MRo_X.SetRotateX(Angle);
        
        TempM = MTra.MulMat(MRo_X);

        MTra.SetTrans(P1.X,P1.Y,P1.Z);

        this = TempM.MulMat(MTra);
      } 
      else
      {
        MTra.SetTrans(-P1.X, -P1.Y, -P1.Z);

        // Prepare matrix rotation about axis X with angle Alfa Cos(Alfa) = C.z / D Sin(Alfa) = C.y / D }
        MRo_X.SetUnit();
        MRo_X.m11 = C.Z / D ;
        MRo_X.m22 = MRo_X.m11;
        MRo_X.m12 = C.Y / D;
        MRo_X.m21 = -MRo_X.m12;

        // prepare matrix rotation about axis Y with angle Beta Cos(Beta) =  D     Sin(Beta) = -C.x 
        MRo_Y.SetUnit();
        MRo_Y.m00 = D;
        MRo_Y.m22 = MRo_Y.m00;
        MRo_Y.m02 = C.X;
        MRo_Y.m20 = -MRo_Y.m02;

        TMat4x4 M;

        // M= Trans * Rot about axis X * Rot about axis Y
        TempM = MTra.MulMat(MRo_X);
        M = TempM.MulMat(MRo_Y);

        // prepare matrix rotation about axis Z with angle Angle
        MRo_Z.SetRotateZ(Angle);

        // TempM= Trans * Rot axis X * Rot axis Y * Rot about axis Z by angle Angle
        TempM = M.MulMat(MRo_Z);

        // Find inverse Y matrix
        MRo_Y.m00 = D;
        MRo_Y.m22 = D;
        MRo_Y.m02 = -C.X;
        MRo_Y.m20 = C.X;

        M = TempM.MulMat(MRo_Y);

        // Find inverse x matrix
        MRo_X.m11 = C.Z / D ;
        MRo_X.m22 = MRo_X.m11;
        MRo_X.m21 = C.Y / D;
        MRo_X.m12 = -MRo_X.m21;

        TempM = M.MulMat(MRo_X);

        // Find inverse translation matrix
        MTra.SetTrans(P1.X,P1.Y,P1.Z);

        this = TempM.MulMat(MTra);
      }
    }

    public TMat4x4 MulMat(TMat4x4 MatToMul)
    {
      TMat4x4 Result;
     
      Result.m00 = m00 * MatToMul.m00 + m01 * MatToMul.m10 + m02 * MatToMul.m20 + m03 * MatToMul.m30;
      Result.m01 = m00 * MatToMul.m01 + m01 * MatToMul.m11 + m02 * MatToMul.m21 + m03 * MatToMul.m31;
      Result.m02 = m00 * MatToMul.m02 + m01 * MatToMul.m12 + m02 * MatToMul.m22 + m03 * MatToMul.m32;
      Result.m03 = m00 * MatToMul.m03 + m01 * MatToMul.m13 + m02 * MatToMul.m23 + m03 * MatToMul.m33;

      Result.m10 = m10 * MatToMul.m00 + m11 * MatToMul.m10 + m12 * MatToMul.m20 + m13 * MatToMul.m30;
      Result.m11 = m10 * MatToMul.m01 + m11 * MatToMul.m11 + m12 * MatToMul.m21 + m13 * MatToMul.m31;
      Result.m12 = m10 * MatToMul.m02 + m11 * MatToMul.m12 + m12 * MatToMul.m22 + m13 * MatToMul.m32;
      Result.m13 = m10 * MatToMul.m03 + m11 * MatToMul.m13 + m12 * MatToMul.m23 + m13 * MatToMul.m33;

      Result.m20 = m20 * MatToMul.m00 + m21 * MatToMul.m10 + m22 * MatToMul.m20 + m23 * MatToMul.m30;
      Result.m21 = m20 * MatToMul.m01 + m21 * MatToMul.m11 + m22 * MatToMul.m21 + m23 * MatToMul.m31;
      Result.m22 = m20 * MatToMul.m02 + m21 * MatToMul.m12 + m22 * MatToMul.m22 + m23 * MatToMul.m32;
      Result.m23 = m20 * MatToMul.m03 + m21 * MatToMul.m13 + m22 * MatToMul.m23 + m23 * MatToMul.m33;

      Result.m30 = m30 * MatToMul.m00 + m31 * MatToMul.m10 + m32 * MatToMul.m20 + m33 * MatToMul.m30;
      Result.m31 = m30 * MatToMul.m01 + m31 * MatToMul.m11 + m32 * MatToMul.m21 + m33 * MatToMul.m31;
      Result.m32 = m30 * MatToMul.m02 + m31 * MatToMul.m12 + m32 * MatToMul.m22 + m33 * MatToMul.m32;
      Result.m33 = m30 * MatToMul.m03 + m31 * MatToMul.m13 + m32 * MatToMul.m23 + m33 * MatToMul.m33;

      return Result;
    }

    public TVertex MulVertex(TVertex v)
    {
      TVertex Result;

      Result.X = v.X * m00 + v.Y * m10 + v.Z * m20 + m30;
      Result.Y = v.X * m01 + v.Y * m11 + v.Z * m21 + m31;
      Result.Z = v.X * m02 + v.Y * m12 + v.Z * m22 + m32;

      return Result;
    }

    private float[,] ConvertToArray()
    {
      return new float[4, 4] {{m00,m01,m02,m03},
                              {m10,m11,m12,m13},
                              {m20,m21,m22,m23},
                              {m30,m31,m32,m33}};
    }

    private void ConvertFromArray(float[,] MatArray)
    {
      m00 = MatArray[0, 0];
      m01 = MatArray[0, 1];
      m02 = MatArray[0, 2];
      m03 = MatArray[0, 3];

      m10 = MatArray[1, 0];
      m11 = MatArray[1, 1];
      m12 = MatArray[1, 2];
      m13 = MatArray[1, 3];

      m20 = MatArray[2, 0];
      m21 = MatArray[2, 1];
      m22 = MatArray[2, 2];
      m23 = MatArray[2, 3];

      m30 = MatArray[3, 0];
      m31 = MatArray[3, 1];
      m32 = MatArray[3, 2];
      m33 = MatArray[3, 3];
    }

    // Clear the translation in the matrix
    public void ClearTranslation()
    {
      m30 = m31 = m32 = 0;
    }

    public TMat4x4 FindInverseMat()
    {
      float Det;
      float[,] C = new float[4, 4];

      C[0,0] = m11 * (m22 * m33 - m23 * m32) -
             m12 * (m21 * m33 - m23 * m31) +
             m13 * (m21 * m32 - m22 * m31);

      C[0, 1] = -(m10 * (m22 * m33 - m23 * m32) -
             m12 * (m20 * m33 - m23 * m30) +
             m13 * (m20 * m32 - m22 * m30));

      C[0, 2] = m10 * (m21 * m33 - m23 * m31) -
             m11 * (m20 * m33 - m23 * m30) +
             m13 * (m20 * m31 - m21 * m30);

      C[0, 3] = 0;
      C[1, 0] = -(m01 * (m22 * m33 - m23 * m32) -
             m02 * (m21 * m33 - m23 * m31) +
             m03 * (m21 * m32 - m22 * m31));

      C[1, 1] = m00 * (m22 * m33 - m23 * m32) -
             m02 * (m20 * m33 - m23 * m30) +
             m03 * (m20 * m32 - m22 * m30);

      C[1, 2] = -(m00 * (m21 * m33 - m23 * m31) -
             m01 * (m20 * m33 - m23 * m30) +
             m03 * (m20 * m31 - m21 * m30));

      C[1, 3] = 0;
      C[2, 0] = m01 * (m12 * m33 - m13 * m32) -
             m02 * (m11 * m33 - m13 * m31) +
             m03 * (m11 * m32 - m12 * m31);

      C[2, 1] = -(m00 * (m12 * m33 - m13 * m32) -
             m02 * (m10 * m33 - m13 * m30) +
             m03 * (m10 * m32 - m12 * m30));

      C[2, 2] = m00 * (m11 * m33 - m13 * m31) -
             m01 * (m10 * m33 - m13 * m30) +
             m03 * (m10 * m31 - m11 * m30);

      C[2, 3] = 0;
      C[3, 0] = -(m01 * (m12 * m23 - m13 * m22) -
             m02 * (m11 * m23 - m13 * m21) +
             m03 * (m11 * m22 - m12 * m21));

      C[3, 1] = m00 * (m12 * m23 - m13 * m22) -
             m02 * (m10 * m23 - m13 * m20) +
             m03 * (m10 * m22 - m12 * m20);

      C[3, 2] = -(m00 * (m11 * m23 - m13 * m21) -
             m01 * (m10 * m23 - m13 * m20) +
             m03 * (m10 * m21 - m11 * m20));

      C[3, 3] = m00 * (m11 * m22 - m12 * m21) -
             m01 * (m10 * m22 - m12 * m20) +
             m02 * (m10 * m21 - m11 * m20);

      Det = 0;

      // Create a temporary array in order to ease calculations
      float[,] TmpM = ConvertToArray();      

      for(int i = 0; i < 4; i++)
        Det += TmpM[0,i] * C[0,i];

      float[,] Result = new float[4,4];

      for(int i = 0; i < 4; i++)
        for(int j = 0; j < 4; j++)
          Result[i, j] = C[j, i] / Det;

      Result[3,3] = 1;

      for (int i = 0; i < 4; i++)
      {
        Result[i, 3] = 0;
        Result[3, i] = 0;

        for (int j = 0; j < 4; j++)
          Result[3, i] = Result[3, i] + TmpM[3, j] * Result[j, i];

        Result[3, i] = -Result[3, i];
      }

      TMat4x4 InverseMat = ZeroMat();
      InverseMat.ConvertFromArray(Result);

      return InverseMat;
    }
  }
}
