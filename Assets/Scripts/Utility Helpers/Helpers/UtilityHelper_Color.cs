using System;
using UnityEngine;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        // Returns 00-FF, value 0->255
        public static string Dec_to_Hex(int value) => value.ToString("X2");

        // Returns 0-255
        public static int Hex_to_Dec(string hex) => Convert.ToInt32(hex, 16);

        // Returns a hex string based on a number between 0->1
        public static string Dec01_to_Hex(float value) => Dec_to_Hex((int)Mathf.Round(value * 255f));

        // Returns a float between 0->1
        public static float Hex_to_Dec01(string hex) => Hex_to_Dec(hex) / 255f;

        // Get Hex Color FF00FF
        public static string GetStringFromColor(Color color)
        {
            string red = Dec01_to_Hex(color.r);
            string green = Dec01_to_Hex(color.g);
            string blue = Dec01_to_Hex(color.b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColorWithAlpha(Color color)
        {
            string alpha = Dec01_to_Hex(color.a);
            return GetStringFromColor(color) + alpha;
        }

        // Sets out values to Hex String 'FF'
        public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha)
        {
            red = Dec01_to_Hex(color.r);
            green = Dec01_to_Hex(color.g);
            blue = Dec01_to_Hex(color.b);
            alpha = Dec01_to_Hex(color.a);
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(float r, float g, float b)
        {
            string red = Dec01_to_Hex(r);
            string green = Dec01_to_Hex(g);
            string blue = Dec01_to_Hex(b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColor(float r, float g, float b, float a)
        {
            string alpha = Dec01_to_Hex(a);
            return GetStringFromColor(r, g, b) + alpha;
        }

        // Get Color from Hex string FF00FFAA
        public static Color GetColorFromString(string color)
        {
            float red = Hex_to_Dec01(color.Substring(0, 2));
            float green = Hex_to_Dec01(color.Substring(2, 2));
            float blue = Hex_to_Dec01(color.Substring(4, 2));
            float alpha = 1f;
            if (color.Length >= 8)
            {
                // Color string contains alpha
                alpha = Hex_to_Dec01(color.Substring(6, 2));
            }
            return new Color(red, green, blue, alpha);
        }

        // Return a color going from Red to Yellow to Green, like a heat map
        public static Color GetRedGreenColor(float value)
        {
            float r = 0f;
            float g = 0f;
            if (value <= .5f)
            {
                r = 1f;
                g = value * 2f;
            }
            else
            {
                g = 1f;
                r = 1f - (value - .5f) * 2f;
            }
            return new Color(r, g, 0f, 1f);
        }


        public static Color GetRandomColor() =>
            new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);

        private static int sequencialColorIndex = -1;
        private static Color[] sequencialColors = new[] {
            GetColorFromString("26a6d5"),
            GetColorFromString("41d344"),
            GetColorFromString("e6e843"),
            GetColorFromString("e89543"),
            GetColorFromString("0f6ad0"),//("d34141"),
		    GetColorFromString("b35db6"),
            GetColorFromString("c45947"),
            GetColorFromString("9447c4"),
            GetColorFromString("4756c4"),
        };

        public static void ResetSequencialColors() => sequencialColorIndex = -1;

        public static Color GetSequencialColor()
        {
            sequencialColorIndex = (sequencialColorIndex + 1) % sequencialColors.Length;
            return sequencialColors[sequencialColorIndex];
        }

        public static Color GetColor255(float red, float green, float blue, float alpha = 255f) =>
            new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);


        // Take two color arrays (pixels) and merge them
        public static void MergeColorArrays(Color[] baseArray, Color[] overlay)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (overlay[i].a > 0)
                {
                    // Not empty color
                    if (overlay[i].a >= 1)
                        // Fully replace
                        baseArray[i] = overlay[i];
                    else
                    {
                        // Interpolate colors
                        float alpha = overlay[i].a;
                        baseArray[i].r += (overlay[i].r - baseArray[i].r) * alpha;
                        baseArray[i].g += (overlay[i].g - baseArray[i].g) * alpha;
                        baseArray[i].b += (overlay[i].b - baseArray[i].b) * alpha;
                        baseArray[i].a += overlay[i].a;
                    }
                }
            }
        }

        // Replace color in baseArray with replaceArray if baseArray[i] != ignoreColor
        public static void ReplaceColorArrays(Color[] baseArray, Color[] replaceArray, Color ignoreColor)
        {
            for (int i = 0; i < baseArray.Length; i++)
                if (baseArray[i] != ignoreColor)
                    baseArray[i] = replaceArray[i];
        }

        public static void MaskColorArrays(Color[] baseArray, Color[] mask)
        {
            for (int i = 0; i < baseArray.Length; i++)
                if (baseArray[i].a > 0f)
                    baseArray[i].a = mask[i].a;
        }

        public static void TintColorArray(Color[] baseArray, Color tint)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                // Apply tint
                baseArray[i].r = tint.r * baseArray[i].r;
                baseArray[i].g = tint.g * baseArray[i].g;
                baseArray[i].b = tint.b * baseArray[i].b;
            }
        }
        public static void TintColorArrayInsideMask(Color[] baseArray, Color tint, Color[] mask)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (mask[i].a > 0)
                {
                    // Apply tint
                    Color baseColor = baseArray[i];
                    Color fullyTintedColor = tint * baseColor;
                    float interpolateAmount = mask[i].a;
                    baseArray[i].r = baseColor.r + (fullyTintedColor.r - baseColor.r) * interpolateAmount;
                    baseArray[i].g = baseColor.g + (fullyTintedColor.g - baseColor.g) * interpolateAmount;
                    baseArray[i].b = baseColor.b + (fullyTintedColor.b - baseColor.b) * interpolateAmount;
                }
            }
        }

        public static Color TintColor(Color baseColor, Color tint)
        {
            // Apply tint
            baseColor.r = tint.r * baseColor.r;
            baseColor.g = tint.g * baseColor.g;
            baseColor.b = tint.b * baseColor.b;

            return baseColor;
        }

        public static bool IsColorSimilar255(Color colorA, Color colorB, int maxDiff) =>
            IsColorSimilar(colorA, colorB, maxDiff / 255f);

        public static bool IsColorSimilar(Color colorA, Color colorB, float maxDiff)
        {
            float rDiff = Mathf.Abs(colorA.r - colorB.r);
            float gDiff = Mathf.Abs(colorA.g - colorB.g);
            float bDiff = Mathf.Abs(colorA.b - colorB.b);
            float aDiff = Mathf.Abs(colorA.a - colorB.a);

            float totalDiff = rDiff + gDiff + bDiff + aDiff;
            return totalDiff < maxDiff;
        }

        public static float GetColorDifference(Color colorA, Color colorB)
        {
            float rDiff = Mathf.Abs(colorA.r - colorB.r);
            float gDiff = Mathf.Abs(colorA.g - colorB.g);
            float bDiff = Mathf.Abs(colorA.b - colorB.b);
            float aDiff = Mathf.Abs(colorA.a - colorB.a);

            float totalDiff = rDiff + gDiff + bDiff + aDiff;
            return totalDiff;
        }
    }
}