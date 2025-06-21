using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GameInConsoleEngine.Engine;
using SixLabors.ImageSharp.PixelFormats;

namespace GameInConsole.Engine
{
    public static class ArrayExtensions
    {
        public static T[] To2DArray<T>(this T[][] source)
        {
            int rows = source.Length;
            int cols = source[0].Length;
            T[] result = new T[rows * cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = i * cols + j;
                    result[index] = source[i][j];
                }
            }
            return result;
        }
    }
    public static class Rgba32Extensions
    {
        public static int AllColors(this Rgba32 source)
        {
            return ((source.R + source.G + source.B) / 3);
        }
        public static int NearToColor(this Rgba32 source, Rgba32[] colorPalette, int colorMargin)
        {
            for (int i = 0; i < colorPalette.Length; i++)
            {
                Rgba32 color = colorPalette[i];
                // Compute the Euclidean distance between the target color and the current color in the group
                Rgba32 result = subAbsColor(color, source);
                bool inDisRed = result.R <= colorMargin;
                bool inDisGreen = result.G <= colorMargin;
                bool inDisBlue = result.B <= colorMargin;

                // If the distance is within the margin, return true
                if (inDisRed && inDisGreen && inDisBlue)
                {
                    return i;
                }
            }

            // If no color in the group is within the margin, return false
            return -1;
        }
        public static byte[] GetBytes(this Rgba32 source)
        {
            return new byte[]
            {
                source.R,
                source.G,
                source.B,
                source.A,
            };
        }
        public static Raylib_cs.Color GetColor(this Rgba32 source)
        {
            return new Raylib_cs.Color(source.R, source.G, source.B, source.A);
        }

        public static Rgba32 subAbsColor(Rgba32 a, Rgba32 b)
        {
            Rgba32 result = new Rgba32(0, 0, 0);
            result.R = (byte)Math.Abs((a.R - b.R));
            result.G = (byte)Math.Abs((a.G - b.G));
            result.B = (byte)Math.Abs((a.B - b.B));
            return result;
        }
    }
    public static class Abgr32Extensions
    {
        public static int NearToColor(this Abgr32 source, Abgr32[] colorPalette, int colorMargin)
        {
            for (int i = 0; i < colorPalette.Length; i++)
            {
                Abgr32 color = colorPalette[i];
                // Compute the Euclidean distance between the target color and the current color in the group
                Abgr32 result = new Abgr32(color.Abgr - source.Abgr);
                int distance = (int)Math.Sqrt(result.R + result.G + result.B);

                // If the distance is within the margin, return true
                if (distance <= colorMargin)
                {
                    return i;
                }
            }

            // If no color in the group is within the margin, return false
            return -1;
        }
        public static byte[] GetBytes(this Abgr32 source)
        {
            return new byte[]
            {
                source.A,
                source.B,
                source.G,
                source.R,
            };
        }
    }
    public static class SixLaborsExtensions
    {
        public static Color[] FromRgba32ToColor(this Rgba32[] source)
        {
            Color[] result = new Color[source.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (Color)source[i];
            }
            return result;
        }
        public static Color[] FromRgba32ToColor(this Rgba32[][] source)
        {
            int rows = source.Length;
            int cols = source[0].Length;
            Color[] result = new Color[rows * cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = i * cols + j;
                    result[index] = (Color)source[i][j];
                }
            }
            return result;
        }
    }
    public static class ColorExtensions
    {
        public static Rgba32[] FromColorToRgba32(this Color[] source)
        {
            Rgba32[] result = new Rgba32[source.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (Rgba32)source[i];
            }
            return result;
        }
        public static Rgba32[] FromColorToRgba32(this Color[][] source)
        {
            int rows = source.Length;
            int cols = source[0].Length;
            Rgba32[] result = new Rgba32[rows * cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = i * cols + j;
                    result[index] = (Rgba32)source[i][j];
                }
            }
            return result;
        }
    }
    public static class RaylibColorExtensions
    {
        public static Rgba32 ToRGBA32(this Raylib_cs.Color color)
        {
            return new Rgba32(color.R, color.G, color.B, color.A);
        }
    }
}
