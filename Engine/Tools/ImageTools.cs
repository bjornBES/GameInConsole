using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameInConsole.Engine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameInConsoleEngine.Engine.Tools
{
    public static class ImageTools
    {
        public static Rgba32[] CachedData;
        public static Image<Rgba32> GetImageBitmap(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                Image<Rgba32> image = Image.Load<Rgba32>(stream);
                CachedData = GetColors(image);
                return image;
            }
            else
            {
                Console.WriteLine("Image file not found.");
            }
            return null;
        }
        public static Rgba32[] GetColors(string filePath)
        {
            return GetColors(GetImageBitmap(filePath));
        }
        public static Rgba32[] GetColors(Image<Rgba32> image)
        {
            DateTime dateTimeNow = DateTime.Now;
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }
            Color[] colors = new Color[image.Height * image.Width];
            // Loop through some pixels to extract color data
            double diff1 = (DateTime.Now - dateTimeNow).TotalMilliseconds;

            Rgba32[] range = new Rgba32[image.Height * image.Width];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int i = y * image.Width + x;
                    range[i] = image[x, y];
                }
            }

            /*
             */
            double diff2 = (DateTime.Now - dateTimeNow).TotalMilliseconds;
            colors = range.FromRgba32ToColor();
            /*
            for (int i = 0; i < image.Height; i++) // Adjust step size as needed
            {
                Rgba32 pixelColor = range[i];
                colors[i] = (Color)pixelColor;
            }
             */
            double diff = (DateTime.Now - dateTimeNow).TotalMilliseconds;
            return range;
            // with: 13,7984 + 12,1046 + 11,4508 + 10,0908 + 9,5696 = 57,0142 / 5 = 11,40284
            // with out: 14,967 + 12,4198 + 11,1565 + 11,8495 + 9,7202 = 60,113 / 5 = 12,0226
        }
    }
}
