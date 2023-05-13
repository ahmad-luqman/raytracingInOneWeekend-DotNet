using System;

namespace RayTracingInOneWeekend;

class RandomGenerator
{
    private static readonly Random random = new Random(); 
    private static readonly object syncLock = new object(); 
    public static Random Rng
    {
        get
        {
            lock(syncLock)
            { // synchronize
                return random;
            }
        }
        
    }
}