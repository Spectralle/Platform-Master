using UnityEngine;
using TMPro;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        public static string GetTimeHMS(float time, bool hours = true, bool minutes = true,
            bool seconds = true, bool milliseconds = true)
        {
            string h0, h1, m0, m1, s0, s1, ms0, ms1, ms2;
            GetTimeCharacterStrings(time, out h0, out h1, out m0, out m1, out s0, out s1, out ms0, out ms1, out ms2);
            string h = h0 + h1;
            string m = m0 + m1;
            string s = s0 + s1;
            string ms = ms0 + ms1 + ms2;

            if (hours)
            {
                if (minutes)
                {
                    if (seconds)
                    {
                        if (milliseconds)
                            return h + ":" + m + ":" + s + "." + ms;
                        else
                            return h + ":" + m + ":" + s;
                    }
                    else
                        return h + ":" + m;
                }
                else
                    return h;
            }
            else
            {
                if (minutes)
                {
                    if (seconds)
                    {
                        if (milliseconds)
                            return m + ":" + s + "." + ms;
                        else
                            return m + ":" + s;
                    }
                    else
                        return m;
                }
                else
                {
                    if (seconds)
                    {
                        if (milliseconds)
                            return s + "." + ms;
                        else
                            return s;
                    }
                    else
                        return ms;
                }
            }
        }

        public static void SetupTimeHMSTransform(Transform transform, float time)
        {
            string h0, h1, m0, m1, s0, s1, ms0, ms1, ms2;
            GetTimeCharacterStrings(time, out h0, out h1, out m0, out m1, out s0, out s1, out ms0, out ms1, out ms2);

            if (transform.Find("h0") != null && transform.Find("h0").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("h0").GetComponent<TextMeshProUGUI>().text = h0;

            if (transform.Find("h1") != null && transform.Find("h1").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("h1").GetComponent<TextMeshProUGUI>().text = h1;

            if (transform.Find("m0") != null && transform.Find("m0").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("m0").GetComponent<TextMeshProUGUI>().text = m0;

            if (transform.Find("m1") != null && transform.Find("m1").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("m1").GetComponent<TextMeshProUGUI>().text = m1;

            if (transform.Find("s0") != null && transform.Find("s0").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("s0").GetComponent<TextMeshProUGUI>().text = s0;

            if (transform.Find("s1") != null && transform.Find("s1").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("s1").GetComponent<TextMeshProUGUI>().text = s1;

            if (transform.Find("ms0") != null && transform.Find("ms0").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("ms0").GetComponent<TextMeshProUGUI>().text = ms0;

            if (transform.Find("ms1") != null && transform.Find("ms1").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("ms1").GetComponent<TextMeshProUGUI>().text = ms1;

            if (transform.Find("ms2") != null && transform.Find("ms2").GetComponent<TextMeshProUGUI>() != null)
                transform.Find("ms2").GetComponent<TextMeshProUGUI>().text = ms2;
        }

        public static void GetTimeHMS(float time, out int h, out int m, out int s, out int ms)
        {
            s = Mathf.FloorToInt(time);
            m = Mathf.FloorToInt(s / 60f);
            h = Mathf.FloorToInt((s / 60f) / 60f);
            s = s - m * 60;
            m = m - h * 60;
            ms = (int)((time - Mathf.FloorToInt(time)) * 1000);
        }

        public static void GetTimeCharacterStrings(float time,
            out string h0, out string h1,
            out string m0, out string m1,
            out string s0, out string s1,
            out string ms0, out string ms1, out string ms2)
        {
            int s = Mathf.FloorToInt(time);
            int m = Mathf.FloorToInt(s / 60f);
            int h = Mathf.FloorToInt((s / 60f) / 60f);
            s = s - m * 60;
            m = m - h * 60;
            int ms = (int)((time - Mathf.FloorToInt(time)) * 1000);

            if (h < 10)
            {
                h0 = "0";
                h1 = "" + h;
            }
            else
            {
                h0 = "" + Mathf.FloorToInt(h / 10f);
                h1 = "" + (h - Mathf.FloorToInt(h / 10f) * 10);
            }

            if (m < 10)
            {
                m0 = "0";
                m1 = "" + m;
            }
            else
            {
                m0 = "" + Mathf.FloorToInt(m / 10f);
                m1 = "" + (m - Mathf.FloorToInt(m / 10f) * 10);
            }

            if (s < 10)
            {
                s0 = "0";
                s1 = "" + s;
            }
            else
            {
                s0 = "" + Mathf.FloorToInt(s / 10f);
                s1 = "" + (s - Mathf.FloorToInt(s / 10f) * 10);
            }


            if (ms < 10)
            {
                ms0 = "0";
                ms1 = "0";
                ms2 = "" + ms;
            }
            else
            {
                // >= 10
                if (ms < 100)
                {
                    ms0 = "0";
                    ms1 = "" + Mathf.FloorToInt(ms / 10f);
                    ms2 = "" + (ms - Mathf.FloorToInt(ms / 10f) * 10);
                }
                else
                {
                    // >= 100
                    int _i_ms0 = Mathf.FloorToInt(ms / 100f);
                    int _i_ms1 = Mathf.FloorToInt(ms / 10f) - (_i_ms0 * 10);
                    int _i_ms2 = ms - (_i_ms1 * 10) - (_i_ms0 * 100);
                    ms0 = "" + _i_ms0;
                    ms1 = "" + _i_ms1;
                    ms2 = "" + _i_ms2;
                }
            }
        }

        public static void PrintTimeMilliseconds(float startTime, string prefix = "") =>
            Debug.Log(prefix + GetTimeMilliseconds(startTime));

        public static float GetTimeMilliseconds(float startTime) =>
            (Time.realtimeSinceStartup - startTime) * 1000f;

        public static string GetMonthName(int month)
        {
            switch (month)
            {
                default:
                case 0:
                    return "January";
                case 1:
                    return "February";
                case 2:
                    return "March";
                case 3:
                    return "April";
                case 4:
                    return "May";
                case 5:
                    return "June";
                case 6:
                    return "July";
                case 7:
                    return "August";
                case 8:
                    return "September";
                case 9:
                    return "October";
                case 10:
                    return "November";
                case 11:
                    return "December";
            }
        }

        public static string GetMonthNameShort(int month) => GetMonthName(month).Substring(0, 3);
    }
}