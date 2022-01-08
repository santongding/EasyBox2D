using UnityEngine;

namespace ReducedBox2D
{
    public struct Mat22
    {
        public Mat22(float angle)
        {
            float c = Mathf.Cos(angle), s = Mathf.Sin(angle);

            col1.x = c;
            col2.x = -s;
            col1.y = s;
            col2.y = c;
        }

        Mat22(Vector2 c1, Vector2 c2)
        {
            col1 = c1;
            col2 = c2;
        }

        public Mat22 Transpose

            => new Mat22(new Vector2(col1.x, col2.x), new Vector2(col1.y, col2.y));


        public Mat22 Invert
        {
            get
            {
                {
                    float a = col1.x, b = col2.x, c = col1.y, d = col2.y;
                    Mat22 B;
                    float det = a * d - b * c;
                    Debug.Assert(det != 0.0f);
                    det = 1.0f / det;
                    B.col1.x = det * d;
                    B.col2.x = -det * b;
                    B.col1.y = -det * c;
                    B.col2.y = det * a;
                    return B;
                }
            }
        }

        public Mat22 Abs
        {
            get { return new Mat22(col1.Abs(), col2.Abs()); }
        }

        public Vector2 col1, col2;

        public static Mat22 operator +(Mat22 b, Mat22 c)
        {
            return new Mat22(b.col1 + c.col1, b.col2 + c.col2);
        }

        public static Vector2 operator *(Mat22 A, Vector2 v)
        {
            return new Vector2(A.col1.x * v.x + A.col2.x * v.y, A.col1.y * v.x + A.col2.y * v.y);
        }

        public static Mat22 operator *(Mat22 A, Mat22 B)
        {
            return new Mat22(A * B.col1, A * B.col2);
        }
    };

    public static class MathUtil
    {
        public static Vector2 Abs(this Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        public static float Cross(this Vector2 v1, Vector2 v2)
        {
            return Vector3.Cross(v1, v2).z;
        }


        public static Vector2 Cross(this float r, Vector2 v)
        {
            return new Vector2(-v.y * r, v.x * r);
        }

        public static Vector2 Cross(this Vector2 v, float r)
        {
            return new Vector2(r * v.y, -r * v.x);
        }

        public static Vector2 Rotate(this Vector2 v, float r)
        {
            var c = Mathf.Cos(r);
            var s = Mathf.Sin(r);
            return new Vector2(v.x * c - v.y * s, v.x * s + v.y * s);
        }

        public static bool Equals(this float f1, float f2)
        {
            return Mathf.Abs(f1 - f2) < 1e-4f;
        }

        public static bool Equals(this Vector2 v1, Vector2 v2)
        {
            return v1.x.Equals(v2.x) && v1.y.Equals(v2.y);
        }
    }
}