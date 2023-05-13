using System;

namespace RayTracingInOneWeekend
{
    internal class Program
    {
        static Random _rng = new();
        static float Clamp(float x, float min, float max)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }
        
        static Vector3d GetColor(Ray r, Hitable world, int depth)
        {
            HitRecord rec = new HitRecord();

            // If we've exceeded the ray bounce limit, no more light is gathered.
            if (depth <= 0)
            {
                return new Vector3d(0, 0, 0);
            }

            if (world.Hit(r, 0.001f, float.MaxValue, ref rec))
            {
                Vector3d target = rec.PointOfIntersection + Vector3d.RandomInHemisphere(_rng, rec.Normal);
                return 0.5f * GetColor(new Ray(rec.PointOfIntersection, target - rec.PointOfIntersection), world, depth - 1);
            }

            Vector3d unit_direction = r.Direction.Normalize();
            float t = 0.5f * (unit_direction.y + 1.0f);
            return (1.0f - t) * new Vector3d(1.0f, 1.0f, 1.0f) + t * new Vector3d(0.5f, 0.7f, 1.0f);
        }

        static void WriteColor(Vector3d pixel_color, int samples_per_pixel)
        {
            var r = pixel_color.x;
            var g = pixel_color.y;
            var b = pixel_color.z;

            // Divide the color by the number of samples.
            float scale = 1.0f / samples_per_pixel;
            r = (float)Math.Sqrt(scale * r);
            g = (float)Math.Sqrt(scale * g);
            b = (float)Math.Sqrt(scale * b);

            // Write the clamped [0,255] value of each color component.
            Console.WriteLine($"{(int)(256 * Clamp(r, 0.0f, 0.999f))} {(int)(256 * Clamp(g, 0.0f, 0.999f))} {(int)(256 * Clamp(b, 0.0f, 0.999f))}");
        }

        static void Main(string[] args)
        {
            // Image

            const double aspect_ratio = 16.0 / 9.0;
            const int image_width = 1200;
            int image_height = (int)(image_width / aspect_ratio);
            int samples_per_pixel = 100;
            int max_depth = 50;

            // World

            HitableList world = new HitableList(new Hitable[] {
                new Sphere(new Vector3d(0, 0, -1), 0.5f),
                new Sphere(new Vector3d(0, -100.5f, -1), 100)
            });

            // Camera
            var lookFrom = new Vector3d(3, 3, 2);
            var lookAt = new Vector3d(0, 0, -1);
            var vup = new Vector3d(0, 1, 0);
            var dist_to_focus = (lookFrom - lookAt).Magnitude;

            var camera = new Camera(lookFrom, lookAt, vup, 20, Convert.ToSingle(aspect_ratio), 0.1f, dist_to_focus);

            // Render

            Console.Write($"P3\n{image_width} {image_height}\n255\n");

            for (int j = image_height-1; j >= 0; --j) {
                Console.Error.WriteLine($"Scanlines remaining: {j}");
                Console.Error.Flush();
                for (int i = 0; i < image_width; ++i) {
                    var pixel_color = new Vector3d(0, 0, 0);
                    for (int s = 0; s < samples_per_pixel; ++s)
                    {
                        float u = (float)(i + _rng.NextDouble()) / (image_width - 1);
                        float v = (float)(j + _rng.NextDouble()) / (image_height - 1);
                        Ray r = camera.GetRay(u, v);
                        pixel_color += GetColor(r, world, max_depth);
                    }

                    WriteColor(pixel_color, samples_per_pixel);
                }
            }
            Console.Error.WriteLine("Done.");
        }
    }
}