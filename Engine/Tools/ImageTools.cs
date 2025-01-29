using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameInConsoleEngine.Engine.Tools
{
    public static class ImageTools
    {
        public static Image<Rgba32> GetImageBitmap(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                Image<Rgba32> image = Image.Load<Rgba32>(stream);
                return image;
            }
            else
            {
                Console.WriteLine("Image file not found.");
            }
            return null;
        }
        public static Color[] GetColors(string filePath)
        {
            return GetColors(GetImageBitmap(filePath));
        }
        public static Color[] GetColors(Image<Rgba32> image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }
            Color[] colors = new Color[image.Height * image.Width];
            // Loop through some pixels to extract color data
            for (int y = 0; y < image.Height; y++) // Adjust step size as needed
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int index = y * image.Width + x;
                    Rgba32 pixelColor = image[x, y];
                    Color color = new Color(pixelColor.R, pixelColor.G, pixelColor.B);
                    colors[index] = color;
                }
            }
            return colors;
        }
    }
}
