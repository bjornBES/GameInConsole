using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameInConsole.Engine;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameInConsoleEngine.Engine.Tools
{
    public class FontTools
    {
        private static Image<Rgba32> image;
        public static int fontWidth;
        public static int fontHeight;
        private static int charsPerRow;
        private static int charsPerCol;
        private static int TotalChars;

        private static Rgba32[] cachedData;
        private static Dictionary<int, Rgba32[]> cachedCharData = new Dictionary<int, Rgba32[]>();
        public static void GetImageBitmap(string filePath, int _fontWidth, int _fontHeight, int _charsPerRow, int _charsPerCol)
        {
            cachedData = null;
            fontHeight = _fontHeight;
            fontWidth = _fontWidth;
            charsPerRow = _charsPerRow;
            charsPerCol = _charsPerCol;

            TotalChars = (charsPerCol * charsPerRow) - 1;

            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                image = Image.Load<Rgba32>(stream);

                cacheData();
            }
            else
            {
                Console.WriteLine("Image file not found.");
            }
        }
        public static void cacheData()
        {
            cachedData = ImageTools.GetColors(image);
        }
        public static Rgba32[] getCacheData()
        {
            return cachedData;
        }
        public static void CacheAllChars(Rgba32[] colorPalette)
        {
            for (int i = 0; i < TotalChars; i++)
            {
                _ = GetChar(i, colorPalette);
            }
        }
        public static void CacheChars(Rgba32[] colorPalette, int startRange, int EndRange)
        {
            for (int i = startRange - 1; i < EndRange; i++)
            {
                _ = GetChar(i, colorPalette);
            }
        }
        public static Rgba32[] GetChar(int index, Rgba32[] colorPalette)
        {
            DateTime dateTimeNow = DateTime.Now;
            List<Rgba32> result = new List<Rgba32>();

            if (index < 0 || index > TotalChars)
            {
                index = Math.Clamp(index, 0, TotalChars);
            }
            if (cachedCharData.ContainsKey(index))
            {
                result = cachedCharData[index].ToList();
            }
            else
            {
                int totalPixelCol = charsPerCol * (fontWidth + 1);

                int colIndex = index % charsPerCol;
                int rowIndex = index / charsPerCol;

                int firstColPixel = colIndex * (fontWidth + 1);
                int firstRowPixel = rowIndex * (fontHeight + 1);

                double diff1 = (DateTime.Now - dateTimeNow).TotalMilliseconds;
                for (int y = 0; y < fontHeight; y++)
                {
                    int i = (firstRowPixel + y) * totalPixelCol + firstColPixel;
                    Rgba32[] colorsBuffer = cachedData[i..(i + fontWidth)];
                    for (int j = 0; j < colorsBuffer.Length; j++)
                    {
                        Rgba32 temp = colorPalette[colorsBuffer[j].NearToColor(colorPalette, 25)];
                        colorsBuffer[j] = temp;
                    }
                    result.AddRange(colorsBuffer);
                    //Console.WriteLine($"{i} ({x},{y}) {colors[i]}");
                }
                double diff3 = (DateTime.Now - dateTimeNow).TotalMilliseconds;
                double diff = (DateTime.Now - dateTimeNow).TotalMilliseconds;
                cachedCharData.Add(index, result.ToArray());
            }

            return result.ToArray();
        }
    }
}