using UnityEngine;

namespace KRG
{
    public static class RandomEx
    {
        public static T Pick<T>(params T[] options)
        {
            int i = Random.Range(0, options.Length);
            return options[i];
        }
    }
}
