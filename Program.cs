using System;

namespace RayTracingInOneWeekend
{
    internal class Program
    {
        static float Clamp(float x, float min, float max)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }
        
        static Vector3d GetColor(Ray r, Hitable world)
        {
            HitRecord rec = new HitRecord();

            if (world.Hit(r, 0, float.MaxValue, ref rec))
            {
                return 0.5f * (rec.Normal + new Vector3d(1, 1, 1));
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
            r *= scale;
            g *= scale;
            b *= scale;

            // Write the clamped [0,255] value of each color component.
            Console.WriteLine($"{(int)(256 * Clamp(r, 0.0f, 0.999f))} {(int)(256 * Clamp(g, 0.0f, 0.999f))} {(int)(256 * Clamp(b, 0.0f, 0.999f))}");
        }

        static void Main(string[] args)
        {
            Random rng = new();
            // Image

            const double aspect_ratio = 16.0 / 9.0;
            const int image_width = 400;
            int image_height = (int)(image_width / aspect_ratio);
            int samples_per_pixel = 100;

            // World

            HitableList world = new HitableList(new Hitable[] {
                new Sphere(new Vector3d(0, 0, -1), 0.5f),
                new Sphere(new Vector3d(0, -100.5f, -1), 100)
            });

            // Camera
            float viewport_height = 2.0f;
            float focal_length = 1.0f;

            Vector3d origin = new Vector3d(0, 0, 0);

            var camera = new Camera(
                origin,
                viewport_height,
                Convert.ToSingle(aspect_ratio),
                focal_length);

            // Render

            Console.Write($"P3\n{image_width} {image_height}\n255\n");

            for (int j = image_height-1; j >= 0; --j) {
                Console.Error.WriteLine($"Scanlines remaining: {j}");
                Console.Error.Flush();
                for (int i = 0; i < image_width; ++i) {
                    var pixel_color = new Vector3d(0, 0, 0);
                    for (int s = 0; s < samples_per_pixel; ++s)
                    {
                        float u = (float)(i + rng.NextDouble()) / (image_width - 1);
                        float v = (float)(j + rng.NextDouble()) / (image_height - 1);
                        Ray r = camera.GetRay(u, v);
                        pixel_color += GetColor(r, world);
                    }

                    WriteColor(pixel_color, samples_per_pixel);
                }
            }
            Console.Error.WriteLine("Done.");
        }
    }
}