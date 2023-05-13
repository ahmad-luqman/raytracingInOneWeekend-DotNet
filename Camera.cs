namespace RayTracingInOneWeekend;

class Camera
{
    private readonly Vector3d _origin;
    private readonly Vector3d _lowerLeftCorner;
    private readonly float _viewportHeight;
    private readonly float _aspectRatio;
    private readonly float _focalLength;

    public Camera(
        Vector3d origin,
        float viewportHeight,
        float aspectRatio,
        float focalLength)
    {
        _origin = origin;
        _viewportHeight = viewportHeight;
        _aspectRatio = aspectRatio;
        _focalLength = focalLength;

        var viewportWidth = _aspectRatio * _viewportHeight;
        var horizontal = new Vector3d(viewportWidth, 0, 0);
        var vertical = new Vector3d(0, _viewportHeight, 0);

        _lowerLeftCorner = _origin - horizontal / 2 - vertical / 2 - new Vector3d(0, 0, _focalLength);
    }

    public Ray GetRay(float u, float v)
    {
        return new Ray(
            _origin,
            _lowerLeftCorner + u * new Vector3d(_aspectRatio, 0, 0) + v * new Vector3d(0, 1, 0) - _origin);
    }
}