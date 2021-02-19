using System.Collections.Generic;
using UnityEngine;

namespace _0G.Legacy
{
    public static class RandomEx
    {
        public static bool Chance(float probability)
        {
            if (probability <= 0) return false;
            float rf = Random.Range(0f, 1f);
            return rf <= probability;
        }

        public static T Pick<T>(params T[] options)
        {
            int ri = Random.Range(0, options.Length);
            return options[ri];
        }

        public static T Pick<T>(List<T> options)
        {
            int ri = Random.Range(0, options.Count);
            return options[ri];
        }
    }
}