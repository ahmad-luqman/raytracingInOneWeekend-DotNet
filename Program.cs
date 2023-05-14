using System;
using System.Collections.Generic;

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

        static HitableList RandomScene()
        {
            var world = new List<Hitable>();

            var ground_material = new Lambertian(new Vector3d(0.5f, 0.5f, 0.5f));
            world.Add(new Sphere(new Vector3d(0, -1000, 0), 1000, ground_material));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    float choose_mat = (float)_rng.NextDouble();
                    Vector3d center = new Vector3d(a + 0.9f * (float)_rng.NextDouble(), 0.2f, b + 0.9f * (float)_rng.NextDouble());

                    if ((center - new Vector3d(4, 0.2f, 0)).Magnitude > 0.9f)
                    {
                        Material sphere_material;

                        if (choose_mat < 0.8f)
                        {
                            // diffuse
                            var albedo = Vector3d.Random(RandomGenerator.Rng) * Vector3d.Random(RandomGenerator.Rng);
                            sphere_material = new Lambertian(albedo);
                            world.Add(new Sphere(center, 0.2f, sphere_material));
                        }
                        else if (choose_mat < 0.95f)
                        {
                            // metal
                            var albedo = Vector3d.Random(RandomGenerator.Rng, 0.5f, 1);
                            var fuzz = (float)_rng.NextDouble() * 0.5f;
                            sphere_material = new Metal(albedo, fuzz);
                            world.Add(new Sphere(center, 0.2f, sphere_material));
                        }
                        else
                        {
                            // glass
                            sphere_material = new Dielectric(1.5f);
                            world.Add(new Sphere(center, 0.2f, sphere_material));
                        }
                    }
                }
            }

            var material1 = new Dielectric(1.5f);
            world.Add(new Sphere(new Vector3d(0, 1, 0), 1.0f, material1));

            var material2 = new Lambertian(new Vector3d(0.4f, 0.2f, 0.1f));
            world.Add(new Sphere(new Vector3d(-4, 1, 0), 1.0f, material2));

            var material3 = new Metal(new Vector3d(0.7f, 0.6f, 0.5f), 0.0f);
            world.Add(new Sphere(new Vector3d(4, 1, 0), 1.0f, material3));

            return new HitableList(world.ToArray());
        }
        static void Main(string[] args)
        {
            // Image

            const double aspect_ratio = 3.0 / 2.0;
            const int image_width = 1200;
            int image_height = (int)(image_width / aspect_ratio);
            int samples_per_pixel = 5;
            int max_depth = 50;

            // World
            var world = RandomScene();

            // Camera
            var lookFrom = new Vector3d(13, 2, 3);
            var lookAt = new Vector3d(0, 0, 0);
            var vup = new Vector3d(0, 1, 0);
            var dist_to_focus = 10.0f;
            var aperture = 0.1f;

            var camera = new Camera(lookFrom, lookAt, vup, 20, Convert.ToSingle(aspect_ratio), aperture, dist_to_focus);

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