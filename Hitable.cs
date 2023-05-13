namespace RayTracingInOneWeekend;

struct HitRecord
{
    public float T;
    public Vector3d PointOfIntersection;
    public Vector3d Normal;
    //public Material Material;
}

abstract class Hitable
{
    public abstract bool Hit(Ray r, float tMin, float tMax, ref HitRecord record);
}

class HitableList : Hitable
{
    private readonly Hitable[] _hitables;

    public HitableList(Hitable[] hitables)
    {
        _hitables = hitables;
    }

    public override bool Hit(Ray r, float tMin, float tMax, ref HitRecord record)
    {
        var hitAnything = false;
        var closestSoFar = tMax;

        foreach (var t in _hitables)
        {
            if (!t.Hit(r, tMin, closestSoFar, ref record))
                continue;
                
            hitAnything = true;
            closestSoFar = record.T;
        }

        return hitAnything;
    }
}