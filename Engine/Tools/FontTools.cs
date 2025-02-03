using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static void GetImageBitmap(string filePath, int _fontWidth, int _fontHeight, int _charsPerRow, int _charsPerCol)
        {
            fontHeight = _fontHeight;
            fontWidth = _fontWidth;
            charsPerRow = _charsPerRow;
            charsPerCol = _charsPerCol;

            TotalChars = (charsPerCol * charsPerRow) - 1;

            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                image = Image.Load<Rgba32>(stream);
            }
            else
            {
                Console.WriteLine("Image file not found.");
            }
        }
        public static Color[] GetChar(int index)
        {
            if (index < 0 || index > TotalChars)
            {
                index = Math.Clamp(index, 0, TotalChars);
            }

            int totalPixelCol = charsPerCol * (fontWidth + 1);

            int colIndex = index % charsPerCol;
            int rowIndex = index / charsPerCol;

            int firstColPixel = colIndex * (fontWidth + 1);
            int firstRowPixel = rowIndex * (fontHeight + 1);

            List<Color> result = new List<Color>();

            Color[] colors = ImageTools.GetColors(image);
            for (int y = 0; y < fontHeight; y++)
            {
                int i = (firstRowPixel + y) * totalPixelCol + firstColPixel;
                // i..i + FontWidth
                Color[] colorsBuffer = colors[i..(i + fontWidth)];
                result.AddRange(colorsBuffer);
                //Console.WriteLine($"{i} ({x},{y}) {colors[i]}");
            }

            return result.ToArray();
        }
    }
}