using System;
namespace RayTracingInOneWeekend;

class Camera
{
    private readonly Vector3d _lowerLeftCorner;
    private readonly Vector3d _horizontal;
    private readonly Vector3d _vertical;
    private readonly Vector3d _origin;
    private readonly Vector3d _u;
    private readonly Vector3d _v;
    private readonly double _lensRadius;
    private readonly Random _rng = new();
    private static readonly Vector3d Size = new(1, 1, 0);

    // verticalFieldOfViewDegrees is top to bottom in degrees.
    public Camera(Vector3d lookFrom, Vector3d lookAt, Vector3d viewUp, float verticalFieldOfViewDegrees, float aspectRatio, float aperture, float focusDistance)
    {
        _lensRadius = aperture / 2;
        var theta = verticalFieldOfViewDegrees * Math.PI / 180;
        var halfHeight = Convert.ToSingle(Math.Tan(theta / 2));
        var halfWidth = aspectRatio * halfHeight;

        _origin = lookFrom;
        Vector3d _w = new Vector3d(lookFrom - lookAt).Normalize();
        _u = new Vector3d(Vector3d.Cross(viewUp, _w)).Normalize();
        _v = Vector3d.Cross(_w, _u);

        _lowerLeftCorner = _origin - halfWidth * focusDistance * _u - halfHeight * focusDistance * _v - focusDistance * _w;
        _horizontal = 2 * halfWidth * focusDistance * _u;
        _vertical = 2 * halfHeight * focusDistance * _v;
    }

    public Ray GetRay(float s, float t)
    {
        var rayDirection = Convert.ToSingle(_lensRadius) * Vector3d.Random(RandomGenerator.Rng);
        var offset = _u * rayDirection.x + _v * rayDirection.y;
        return new Ray(_origin + offset, _lowerLeftCorner + s * _horizontal + t * _vertical - _origin - offset);
    }
}