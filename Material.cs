using System;
namespace RayTracingInOneWeekend;

abstract class Material
{
    public abstract bool Scatter(Ray r_in, HitRecord rec, ref Vector3d attenuation, ref Ray scattered);

    protected Vector3d Reflect(Vector3d v, Vector3d n)
    {
        return v - 2 * Vector3d.Dot(v, n) * n;
    }

    protected Vector3d Refract(Vector3d uv, Vector3d n, float etai_over_etat)
    {
        var cos_theta = Vector3d.Dot(-1 * uv, n);
        var r_out_perp = etai_over_etat * (uv + cos_theta * n);
        var r_out_parallel = -1 * Convert.ToSingle(Math.Sqrt(1.0 - r_out_perp.Magnitude)) * n;
        return r_out_parallel + r_out_perp;
    }
}

class Lambertian : Material
{
    private readonly Vector3d _albedo;

    public Lambertian(Vector3d albedo)
    {
        _albedo = albedo;
    }

    public override bool Scatter(Ray r_in, HitRecord rec, ref Vector3d attenuation, ref Ray scattered)
    {
        var scatter_direction = rec.Normal + Vector3d.RandomUnitVector(RandomGenerator.Rng);

        if (scatter_direction.NearZero())
            scatter_direction = rec.Normal;

        scattered = new Ray(rec.PointOfIntersection, scatter_direction);
        attenuation = _albedo;
        return true;
    }
}

class Metal : Material
{
    private readonly Vector3d _albedo;
    private readonly float _fuzz;

    public Metal(Vector3d albedo, float fuzz)
    {
        _albedo = albedo;
        _fuzz = fuzz < 1 ? fuzz : 1;
    }

    public override bool Scatter(Ray r_in, HitRecord rec, ref Vector3d attenuation, ref Ray scattered)
    {
        var reflected = Vector3d.Reflect(r_in.Direction.Normalize(), rec.Normal);
        scattered = new Ray(rec.PointOfIntersection, reflected + _fuzz * Vector3d.RandomInUnitSphere(RandomGenerator.Rng));
        attenuation = _albedo;
        return Vector3d.Dot(scattered.Direction, rec.Normal) > 0;
    }
}

class Dielectric : Material
{
    private readonly float _refractionIndex;

    public Dielectric(float refractionIndex)
    {
        _refractionIndex = refractionIndex;
    }

    public override bool Scatter(Ray r_in, HitRecord rec, ref Vector3d attenuation, ref Ray scattered)
    {
        attenuation = new Vector3d(1.0f, 1.0f, 1.0f);
        var refractionRatio = rec.FrontFace ? (1.0f / _refractionIndex) : _refractionIndex;

        var unitDirection = r_in.Direction.Normalize();
        var cosTheta = Convert.ToSingle(Math.Min(Vector3d.Dot(-1*unitDirection, rec.Normal), 1.0));
        var sinTheta = Convert.ToSingle(Math.Sqrt(1.0 - cosTheta * cosTheta));

        var cannotRefract = refractionRatio * sinTheta > 1.0;
        Vector3d direction;
        
        if(cannotRefract || Reflectance(cosTheta, refractionRatio) > RandomGenerator.Rng.NextDouble())
        {
            direction = Vector3d.Reflect(unitDirection, rec.Normal);
        }
        else
        {
            direction = Refract(unitDirection, rec.Normal, refractionRatio);
        }

        scattered = new Ray(rec.PointOfIntersection, direction);

        return true;
    }

    private static float Reflectance(float cosine, float refIdx)
    {
        // Use Schlick's approximation for reflectance.
        var r0 = (1 - refIdx) / (1 + refIdx);
        r0 = r0 * r0;
        return (float)(r0 + (1 - r0) * Math.Pow((1 - cosine), 5));
    }
}