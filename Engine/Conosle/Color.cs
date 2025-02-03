using SDL2;

namespace GameInConsoleEngine.Engine
{
	/// <summary> Represents an RGB color. </summary>
	public class Color {

		public static Color defaultColor = new Color(0, 0, 0);

		/// <summary> Red component. </summary>
		public uint R { get; set; }
		/// <summary> Green component. </summary>
		public uint G { get; set; }
		/// <summary> Bkue component. </summary>
		public uint B { get; set; }

		public int AllColor { get { return (int)((R + G + B) / 3); } }

		/// <summary> Creates a new Color from rgb. </summary>
		public Color(int r, int g, int b) {
			this.R = (uint)r;
			this.G = (uint)g;
			this.B = (uint)b;
		}
		public SDL.SDL_Color GetSDLColor()
		{
			return new SDL.SDL_Color() { r = (byte)R, g = (byte)G, b = (byte)B, a = 255 };
		}
		public int NearToColor(Color[] colors, int colorMargin)
		{
			for (int i = 0; i < colors.Length; i++)
            {
				Color color = colors[i];
                // Compute the Euclidean distance between the target color and the current color in the group
                int distance = (int)Math.Sqrt(
                    (color.R - R) +
                    (color.G - G) +
                    (color.B - B)
                );

                // If the distance is within the margin, return true
                if (distance <= colorMargin)
                {
                    return i;
                }
            }

            // If no color in the group is within the margin, return false
            return -1;
        }
		public static Color operator +(Color color, int n)
		{
			color.R += (uint)n;
			color.G += (uint)n;
			color.B += (uint)n;
			return color;
		}
        public override string ToString()
        {
			return $"{{{R},{G},{B}}}";
        }
    }
}
