using System;

namespace TrueBRChaos
{
    internal sealed class ChaosRandomHandler
    {
        internal    ChaosRandomHandler(int seed) { random = new Random(seed); }
        private     Random random;

        internal int Range(int min, int max, bool excludeMax = false)
        {
            (min, max) = FixMinMax(min, max);
            return random.Next(min, !excludeMax && max != int.MaxValue ? (max + 1) : (max));
        }

        internal float Range(float min, float max)
        {
            (min, max) = FixMinMax(min, max);
            int rand = random.Next(UnityEngine.Mathf.FloorToInt(min), UnityEngine.Mathf.FloorToInt(max));
            return min + ((float)random.NextDouble() * (max - min));
        }

        private (T, T) FixMinMax<T>(T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) > 0)
            {
                T temp = min;
                min = max;
                max = temp;
            }
            return (min, max);
        }
    }
}
