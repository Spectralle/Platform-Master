using UnityEngine;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        // Return a number with milli dots, 1000000 -> 1.000.000
        public static string GetMilliDots(float n) => GetMilliDots((long)n);

        public static string GetMilliDots(long n)
        {
            string ret = n.ToString();
            for (int i = 1; i <= Mathf.Floor(ret.Length / 4); i++)
                ret = ret.Substring(0, ret.Length - i * 3 - (i - 1)) +
                    "." + ret.Substring(ret.Length - i * 3 - (i - 1));
            return ret;
        }


        // Return with milli dots and dollar sign
        public static string GetDollars(float n) => GetDollars((long)n);
        public static string GetDollars(long n)
        {
            if (n < 0)
                return "-$" + GetMilliDots(Mathf.Abs(n));
            else
                return "$" + GetMilliDots(n);
        }
    }
}