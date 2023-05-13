using System;
using System.Text;

namespace RayTracingInOneWeekend
{
    class Vector3d
    {
        private float[] e = new float[3];

        public Vector3d()
        {
        }

        public Vector3d(Vector3d v)
        {
            e[0] = v.e[0];
            e[1] = v.e[1];
            e[2] = v.e[2];
        }

        public Vector3d(float e0, float e1, float e2)
        {
            e[0] = e0;
            e[1] = e1;
            e[2] = e2;
        }

        public void Set(float e0, float e1, float e2)
        {
            e[0] = e0;
            e[1] = e1;
            e[2] = e2;
        }


        public float x
        {
            get { return e[0]; }
            set { e[0] = value; }
        }
        public float y
        {
            get { return e[1]; }
            set { e[1] = value; }
        }
        public float z
        {
            get { return e[2]; }
            set { e[2] = value; }
        }

        public float r
        {
            get { return e[0]; }
            set { e[0] = value; }
        }
        public float g
        {
            get { return e[1]; }
            set { e[1] = value; }
        }
        public float b
        {
            get { return e[2]; }
            set { e[2] = value; }
        }

        public float this[int i]
        {
            get { return e[i]; }
            set { e[i] = value; }
        }

        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3d operator *(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3d operator /(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static bool operator ==(Vector3d a, Vector3d b)
        {
            return (a.x == b.x && a.y == b.y && a.z == b.z);
        }

        public static bool operator !=(Vector3d a, Vector3d b)
        {
            return (a.x != b.x || a.y != b.y || a.z != b.z);
        }

        public static Vector3d operator +(Vector3d a, float b)
        {
            return new Vector3d(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3d operator -(Vector3d a, float b)
        {
            return new Vector3d(a.x - b, a.y - b, a.z - b);
        }

        public static Vector3d operator *(Vector3d a, float b)
        {
            return new Vector3d(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3d operator *(float b, Vector3d a)
        {
            return new Vector3d(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3d operator /(Vector3d a, float b)
        {
            return new Vector3d(a.x / b, a.y / b, a.z / b);
        }

        public readonly int dimension = 3;

        public float Magnitude
        {
            get { return (float)Math.Sqrt(Sqr_Magnitude); }
        }

        public float Sqr_Magnitude
        {
            get { return (x * x + y * y + z * z); }
        }

        public void normalized()
        {
            float length = Magnitude;
            e[0] /= length;
            e[1] /= length;
            e[2] /= length;
        }

        public Vector3d GetIntVec()
        {
            return new Vector3d((int)x, (int)y, (int)z);
        }

        public Vector3d Normalize()
        {
            return this / Magnitude;
        }

        public float Dot(Vector3d a)
        {
            return a.x * x + a.y * y + a.z * z;
        }

        public static float Dot(Vector3d a, Vector3d b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public Vector3d Cross(Vector3d a)
        {
            return new Vector3d(
                a.y * z - a.z * y,
                a.z * x - a.x * z,
                a.x * y - a.y * x);
        }

        public static Vector3d Cross(Vector3d a, Vector3d b)
        {
            return new Vector3d(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(r).Append(" ").Append(g).Append(" ").Append(b);
            return builder.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            Vector3d v = (Vector3d)obj;
            if (this == v)
                return true;
            return false;
        }

        public static Vector3d Random(Random rng)
        {
            return new Vector3d(
                (float)rng.NextDouble(),
                (float)rng.NextDouble(),
                (float)rng.NextDouble());
        }

        public static Vector3d Random(Random rng, float min, float max)
        {
            return new Vector3d(
                (float)rng.NextDouble() * (max - min) + min,
                (float)rng.NextDouble() * (max - min) + min,
                (float)rng.NextDouble() * (max - min) + min);
        }

        public static Vector3d RandomInUnitSphere(Random rng)
        {
            while (true)
            {
                Vector3d p = Random(rng, -1, 1);
                if (p.Sqr_Magnitude >= 1)
                    continue;
                return p;
            }
        }

        public static Vector3d RandomUnitVector(Random rng)
        {
            return RandomInUnitSphere(rng).Normalize();
        }

        public static Vector3d RandomInHemisphere(Random rng, Vector3d normal)
        {
            Vector3d in_unit_sphere = RandomInUnitSphere(rng);
            if (Dot(in_unit_sphere, normal) > 0.0f)
                return in_unit_sphere;
            else
                return -1 * in_unit_sphere;
        }
    }
}