using System.Drawing;

namespace GameInConsole.Game.Dialogsystem
{
    public static class DialogSystem
    {
        public static void Test()
        {
            string filePath = "";
            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                Image image = Image.FromStream(stream);
                Bitmap bitmap = new Bitmap(image);
                Console.WriteLine($"Image Dimensions: {bitmap.Width}x{bitmap.Height}");

                // Loop through some pixels to extract color data
                for (int y = 0; y < bitmap.Height; y += 10) // Adjust step size as needed
                {
                    for (int x = 0; x < bitmap.Width; x += 10)
                    {
                        Color pixelColor = bitmap.GetPixel(x, y);
                        Console.WriteLine($"Pixel at ({x},{y}): R={pixelColor.R}, G={pixelColor.G}, B={pixelColor.B}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Image file not found.");
            }
        }
    }

    public class DialogEntry
    {

    }
}
