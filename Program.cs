using System;

namespace RayTracingInOneWeekend
{
    internal class Program
    {
        static Random _rng = RandomGenerator.Rng;
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
                Ray scattered = new Ray();
                Vector3d attenuation = new Vector3d();
                if (rec.Material.Scatter(r, rec, ref attenuation, ref scattered))
                {
                    return attenuation * GetColor(scattered, world, depth - 1);
                }
                return new Vector3d(0, 0, 0);
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
            var material_ground = new Lambertian(new Vector3d(0.8f, 0.8f, 0.0f));
            var material_center = new Lambertian(new Vector3d(0.1f, 0.2f, 0.5f));
            var material_left = new Dielectric(1.5f);
            var material_right = new Metal(new Vector3d(0.8f, 0.6f, 0.2f), 0.0f);

            HitableList world = new HitableList(new Hitable[] {
                new Sphere(new Vector3d( 0.0f, -100.5f, -1.0f), 100.0f, material_ground),
                new Sphere(new Vector3d( 0.0f,    0.0f, -1.0f),   0.5f, material_center),
                new Sphere(new Vector3d(-1.0f,    0.0f, -1.0f),   0.5f, material_left),
                new Sphere(new Vector3d(-1.0f,    0.0f, -1.0f), -0.45f, material_left),
                new Sphere(new Vector3d( 1.0f,    0.0f, -1.0f),   0.5f, material_right)
            });

            // Camera
            var lookFrom = new Vector3d(3, 3, 2);
            var lookAt = new Vector3d(0, 0, -1);
            var vup = new Vector3d(0, 1, 0);
            var dist_to_focus = (lookFrom - lookAt).Magnitude;

            var camera = new Camera(lookFrom, lookAt, vup, 20, Convert.ToSingle(aspect_ratio), 2.0f, dist_to_focus);

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