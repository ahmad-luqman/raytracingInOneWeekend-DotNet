namespace RayTracingInOneWeekend
{
    class Ray
    {
        //public Ray() { }
        public Ray(Vector3d ori, Vector3d dir) { origin = ori; direction = dir; }
        public Ray(Ray r) { origin = r.Origin; direction = r.Direction; }
        public Vector3d point_at_parameter(float t) { return origin + direction * t; }

        public Vector3d Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        public Vector3d Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        private Vector3d origin;
        private Vector3d direction;
    }
}