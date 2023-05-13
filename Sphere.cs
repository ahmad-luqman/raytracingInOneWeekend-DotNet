using System;

namespace RayTracingInOneWeekend;

class Sphere : Hitable
{
    private readonly Vector3d _center;
    private readonly float _radius;

    public Sphere(Vector3d center, float radius)
    {
        _center = center;
        _radius = radius;
    }

    public override bool Hit(Ray r, float tMin, float tMax, ref HitRecord record)
    {
        var oc = r.Origin - _center;
        var a = r.Direction.Dot(r.Direction);
        var b = oc.Dot(r.Direction);
        var c = oc.Dot(oc) - _radius * _radius;
        var discriminant = b * b - a * c;

        if (discriminant > 0)
        {
            var temp = Convert.ToSingle(-b - Math.Sqrt(b * b - a * c)) / a;
            if (temp < tMax && temp > tMin)
            {
                record.T = temp;
                record.PointOfIntersection = r.point_at_parameter(record.T);
                record.Normal = (record.PointOfIntersection - _center) / _radius;
                return true;
            }

            temp = Convert.ToSingle(-b + Math.Sqrt(b * b - a * c)) / a;
            if (temp < tMax && temp > tMin)
            {
                record.T = temp;
                record.PointOfIntersection = r.point_at_parameter(record.T);
                record.Normal = (record.PointOfIntersection - _center) / _radius;
                return true;
            }
        }

        return false;
    }
}