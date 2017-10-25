// 3D math and basic types

using System;

namespace Engine3D
{
    // 3D vertex type
    public struct TVertex
    {
        public float X, Y, Z;

        // Constructor
        public TVertex(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public void Set(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    class Math3D
    {
        // Radian to degrees conversion
        public static float Rad2Deg(float R)
        {
            return (float)(R * (180.0 / Math.PI));
        }

        // Degrees to radian conversion
        public static float Deg2Rad(float D)
        {
            return (float)(D * (Math.PI / 180.0));
        }

        public static float Sqr(float x)
        {
            return x * x;
        }

        // Return a normalized vector
        public static TVertex CalcNormalizedVec(TVertex p1, TVertex p2)
        {
            float Mag;
            TVertex Result;

            Mag = (float)Math.Sqrt(Sqr(p1.X - p2.X) + Sqr(p1.Y - p2.Y) + Sqr(p1.Z - p2.Z));
            Result.X = (p2.X - p1.X) / Mag;
            Result.Y = (p2.Y - p1.Y) / Mag;
            Result.Z = (p2.Z - p1.Z) / Mag;

            return Result;
        }

        // The distance from point P1 to P2
        public static float PointDistance(TVertex P1, TVertex P2)
        {
            return (float)Math.Sqrt(Sqr(P1.X - P2.X) + Sqr(P1.Y - P2.Y) + Sqr(P1.Z - P2.Z));
        }

        public static float DotProduct(TVertex P1, TVertex P2)
        {
            return P1.X * P2.X + P1.Y * P2.Y + P1.Z * P2.Z;
        }

        public static TVertex CrossProduct(TVertex P1, TVertex P2)
        {
            TVertex Result = new TVertex(P1.Y * P2.Z - P1.Z * P2.Y,
                                         P1.Z * P2.X - P1.X * P2.Z,
                                         P1.X * P2.Y - P1.Y * P2.X);
            return Result;
        }

        public static TVertex CalcFaceNormal(TVertex P1, TVertex P2, TVertex P3)
        {
            P2.X = P2.X - P1.X;
            P2.Y = P2.Y - P1.Y;
            P2.Z = P2.Z - P1.Z;

            P3.X = P3.X - P1.X;
            P3.Y = P3.Y - P1.Y;
            P3.Z = P3.Z - P1.Z;

            return CrossProduct(P2, P3);
        }
    }
}




