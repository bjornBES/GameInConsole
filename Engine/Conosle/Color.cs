using OpenTK.Mathematics;
using SDL2;
using SixLabors.ImageSharp.PixelFormats;

namespace GameInConsoleEngine.Engine
{
    /// <summary> Represents an RGB color. </summary>
    public class Color {

        public static Color defaultColor = new Color(0, 0, 0);

        /// <summary> Red component. </summary>
        public byte R { get; set; }
        /// <summary> Green component. </summary>
        public byte G { get; set; }
        /// <summary> Bkue component. </summary>
        public byte B { get; set; }

        public int AllColor { get { return (int)((R + G + B) / 3); } }

        /// <summary> Creates a new Color from rgb. </summary>
        public Color(int r, int g, int b) {
            this.R = (byte)r;
            this.G = (byte)g;
            this.B = (byte)b;
        }
        public Color4 GetGLColor()
        {
            return new Color4() { R = R, G = G, B = B, A = 255 };
        }
        public int NearToColor(Color[] colors, int colorMargin)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Color color = colors[i];
                // Compute the Euclidean distance between the target color and the current color in the group
                Color result = color - this;
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
        public static Color operator -(Color a, Color b)
        {
            Color result = new Color(0, 0, 0);
            result.R = (byte)(a.R - b.R);
            result.G = (byte)(a.G - b.G);
            result.B = (byte)(a.B - b.B);
            return result;
        }
        public static Color operator +(Color color, byte n)
        {
            color.R += n;
            color.G += n;
            color.B += n;
            return color;
        }
        public override string ToString()
        {
            return $"{{{R},{G},{B}}}";
        }
        public static implicit operator Color4(Color color)
        {
            return color.GetGLColor();
        }
        public static implicit operator Rgba32(Color color)
        {
            return new Rgba32(color.R, color.G, color.B);
        }
        public static implicit operator byte[](Color color)
        {
            return new byte[] { color.R, color.G, color.B, 255 };
        }
        public static explicit operator Color(Rgba32 rgba32)
        {
            return new Color(rgba32.R, rgba32.G, rgba32.B);
        }
    }
}
