using System.Collections.Generic;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        public static T[] ArrayAdd<T>(T[] arr, T add)
        {
            T[] ret = new T[arr.Length + 1];
            for (int i = 0; i < arr.Length; i++)
                ret[i] = arr[i];
            ret[arr.Length] = add;
            return ret;
        }

        public static void ShuffleArray<T>(T[] arr, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = UnityEngine.Random.Range(0, arr.Length);
                T tmp = arr[rnd];
                arr[rnd] = arr[0];
                arr[0] = tmp;
            }
        }
        public static void ShuffleArray<T>(T[] arr, int iterations, System.Random random)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = random.Next(0, arr.Length);
                T tmp = arr[rnd];
                arr[rnd] = arr[0];
                arr[0] = tmp;
            }
        }

        public static void ShuffleList<T>(List<T> list, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = UnityEngine.Random.Range(0, list.Count);
                T tmp = list[rnd];
                list[rnd] = list[0];
                list[0] = tmp;
            }
        }

        // Return random element from array
        public static T GetRandom<T>(T[] array) => array[UnityEngine.Random.Range(0, array.Length)];

        public static T[] RemoveDuplicates<T>(T[] arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                    list.Add(t);
            }
            return list.ToArray();
        }

        public static List<T> RemoveDuplicates<T>(List<T> arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                    list.Add(t);
            }
            return list;
        }
    }
}