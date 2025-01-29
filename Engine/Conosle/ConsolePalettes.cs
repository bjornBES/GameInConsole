namespace GameInConsoleEngine.Engine
{
    /// <summary> Represents prebuilt palettes. </summary>
    public static class Palettes
    {
        /// <summary> Pico8 palette. </summary>
        public static Color[] Pico8 { get; set; } = new Color[16] {
            new Color(0,    0,     0),
            new Color(29,   43,    83),
            new Color(126,  37,    83),
            new Color(0,    135,   81),
            new Color(171,  82,    54),
            new Color(95,   87,    79),
            new Color(194,  195,   199),
            new Color(255,  241,   232),
            new Color(255,  0,     77),
            new Color(255,  163,   0),
            new Color(255,  236,   39),
            new Color(0,    228,   54),
            new Color(41,   173,   255),
            new Color(131,  118,   156),
            new Color(255,  119,   168),
            new Color(255,  204,   170),
        };

        public static Color[] vgaColors = new Color[256]
        {
            // 0-15: Standard VGA colors
            new Color(0, 0, 0),       // 0: Black
            new Color(0, 0, 170),     // 1: Blue
            new Color(0, 170, 0),     // 2: Green
            new Color(0, 170, 170),   // 3: Cyan
            new Color(170, 0, 0),     // 4: Red
            new Color(170, 0, 170),   // 5: Magenta
            new Color(170, 85, 0),    // 6: Brown/Yellow
            new Color(170, 170, 170), // 7: Light Gray
            new Color(85, 85, 85),    // 8: Dark Gray
            new Color(85, 85, 255),   // 9: Light Blue
            new Color(85, 255, 85),   // 10: Light Green
            new Color(85, 255, 255),  // 11: Light Cyan
            new Color(255, 85, 85),   // 12: Light Red
            new Color(255, 85, 255),  // 13: Light Magenta
            new Color(255, 255, 85),  // 14: Light Yellow
            new Color(255, 255, 255), // 15: White
        
            // 16-231: Extended VGA colors (6x6x6 RGB cube)
            // r, g, b = 0, 63, 127, 191, 255
            new Color(0, 0, 0), new Color(0, 0, 95), new Color(0, 0, 135), new Color(0, 0, 175),
            new Color(0, 0, 215), new Color(0, 0, 255), new Color(0, 95, 0), new Color(0, 95, 95),
            new Color(0, 95, 135), new Color(0, 95, 175), new Color(0, 95, 215), new Color(0, 95, 255),
            new Color(0, 135, 0), new Color(0, 135, 95), new Color(0, 135, 135), new Color(0, 135, 175),
            new Color(0, 135, 215), new Color(0, 135, 255), new Color(0, 175, 0), new Color(0, 175, 95),
            new Color(0, 175, 135), new Color(0, 175, 175), new Color(0, 175, 215), new Color(0, 175, 255),
            new Color(0, 215, 0), new Color(0, 215, 95), new Color(0, 215, 135), new Color(0, 215, 175),
            new Color(0, 215, 215), new Color(0, 215, 255), new Color(0, 255, 0), new Color(0, 255, 95),
            new Color(0, 255, 135), new Color(0, 255, 175), new Color(0, 255, 215), new Color(0, 255, 255),
            new Color(95, 0, 0), new Color(95, 0, 95), new Color(95, 0, 135), new Color(95, 0, 175),
            new Color(95, 0, 215), new Color(95, 0, 255), new Color(95, 95, 0), new Color(95, 95, 95),
            new Color(95, 95, 135), new Color(95, 95, 175), new Color(95, 95, 215), new Color(95, 95, 255),
            new Color(95, 135, 0), new Color(95, 135, 95), new Color(95, 135, 135), new Color(95, 135, 175),
            new Color(95, 135, 215), new Color(95, 135, 255), new Color(95, 175, 0), new Color(95, 175, 95),
            new Color(95, 175, 135), new Color(95, 175, 175), new Color(95, 175, 215), new Color(95, 175, 255),
            new Color(95, 215, 0), new Color(95, 215, 95), new Color(95, 215, 135), new Color(95, 215, 175),
            new Color(95, 215, 215), new Color(95, 215, 255), new Color(95, 255, 0), new Color(95, 255, 95),
            new Color(95, 255, 135), new Color(95, 255, 175), new Color(95, 255, 215), new Color(95, 255, 255),
            new Color(135, 0, 0), new Color(135, 0, 95), new Color(135, 0, 135), new Color(135, 0, 175),
            new Color(135, 0, 215), new Color(135, 0, 255), new Color(135, 95, 0), new Color(135, 95, 95),
            new Color(135, 95, 135), new Color(135, 95, 175), new Color(135, 95, 215), new Color(135, 95, 255),
            new Color(135, 135, 0), new Color(135, 135, 95), new Color(135, 135, 135), new Color(135, 135, 175),
            new Color(135, 135, 215), new Color(135, 135, 255), new Color(135, 175, 0), new Color(135, 175, 95),
            new Color(135, 175, 135), new Color(135, 175, 175), new Color(135, 175, 215), new Color(135, 175, 255),
            new Color(135, 215, 0), new Color(135, 215, 95), new Color(135, 215, 135), new Color(135, 215, 175),
            new Color(135, 215, 215), new Color(135, 215, 255), new Color(135, 255, 0), new Color(135, 255, 95),
            new Color(135, 255, 135), new Color(135, 255, 175), new Color(135, 255, 215), new Color(135, 255, 255),
            new Color(175, 0, 0), new Color(175, 0, 95), new Color(175, 0, 135), new Color(175, 0, 175),
            new Color(175, 0, 215), new Color(175, 0, 255), new Color(175, 95, 0), new Color(175, 95, 95),
            new Color(175, 95, 135), new Color(175, 95, 175), new Color(175, 95, 215), new Color(175, 95, 255),
            new Color(175, 135, 0), new Color(175, 135, 95), new Color(175, 135, 135), new Color(175, 135, 175),
            new Color(175, 135, 215), new Color(175, 135, 255), new Color(175, 175, 0), new Color(175, 175, 95),
            new Color(175, 175, 135), new Color(175, 175, 175), new Color(175, 175, 215), new Color(175, 175, 255),
            new Color(175, 215, 0), new Color(175, 215, 95), new Color(175, 215, 135), new Color(175, 215, 175),
            new Color(175, 215, 215), new Color(175, 215, 255), new Color(175, 255, 0), new Color(175, 255, 95),
            new Color(175, 255, 135), new Color(175, 255, 175), new Color(175, 255, 215), new Color(175, 255, 255),
            new Color(215, 0, 0), new Color(215, 0, 95), new Color(215, 0, 135), new Color(215, 0, 175),
            new Color(215, 0, 215), new Color(215, 0, 255), new Color(215, 95, 0), new Color(215, 95, 95),
            new Color(215, 95, 135), new Color(215, 95, 175), new Color(215, 95, 215), new Color(215, 95, 255),
            new Color(215, 135, 0), new Color(215, 135, 95), new Color(215, 135, 135), new Color(215, 135, 175),
            new Color(215, 135, 215), new Color(215, 135, 255), new Color(215, 175, 0), new Color(215, 175, 95),
            new Color(215, 175, 135), new Color(215, 175, 175), new Color(215, 175, 215), new Color(215, 175, 255),
            new Color(215, 215, 0), new Color(215, 215, 95), new Color(215, 215, 135), new Color(215, 215, 175),
            new Color(215, 215, 215), new Color(215, 215, 255), new Color(215, 255, 0), new Color(215, 255, 95),
            new Color(215, 255, 135), new Color(215, 255, 175), new Color(215, 255, 215), new Color(215, 255, 255),
            new Color(255, 0, 0), new Color(255, 0, 95), new Color(255, 0, 135), new Color(255, 0, 175),
            new Color(255, 0, 215), new Color(255, 0, 255), new Color(255, 95, 0), new Color(255, 95, 95),
            new Color(255, 95, 135), new Color(255, 95, 175), new Color(255, 95, 215), new Color(255, 95, 255),
            new Color(255, 135, 0), new Color(255, 135, 95), new Color(255, 135, 135), new Color(255, 135, 175),
            new Color(255, 135, 215), new Color(255, 135, 255), new Color(255, 175, 0), new Color(255, 175, 95),
            new Color(255, 175, 135), new Color(255, 175, 175), new Color(255, 175, 215), new Color(255, 175, 255),
            new Color(255, 215, 0), new Color(255, 215, 95), new Color(255, 215, 135), new Color(255, 215, 175),
            new Color(255, 215, 215), new Color(255, 215, 255), new Color(255, 255, 0), new Color(255, 255, 95),
            new Color(255, 255, 135), new Color(255, 255, 175), new Color(255, 255, 215), new Color(255, 255, 255),
        
            // 232-255: Grayscale colors
            new Color(8, 8, 8), new Color(18, 18, 18), new Color(28, 28, 28), new Color(38, 38, 38),
            new Color(48, 48, 48), new Color(58, 58, 58), new Color(68, 68, 68), new Color(78, 78, 78),
            new Color(88, 88, 88), new Color(98, 98, 98), new Color(108, 108, 108), new Color(118, 118, 118),
            new Color(128, 128, 128), new Color(138, 138, 138), new Color(148, 148, 148), new Color(158, 158, 158),
            new Color(168, 168, 168), new Color(178, 178, 178), new Color(188, 188, 188), new Color(198, 198, 198),
            new Color(208, 208, 208), new Color(218, 218, 218), new Color(228, 228, 228), new Color(238, 238, 238)
        };

        /// <summary> default windows console palette. </summary>
        public static Color[] Default { get; set; } = new Color[16] {
            new Color(12,   12,     12),			// Black
			new Color(0,    55,     218),			// DarkBlue
			new Color(19,   161,    14),			// DarkGreen
			new Color(58,   150,    221),			// DarkCyan
			new Color(197,  15,     31),			// DarkRed
			new Color(136,  23,     152),			// DarkMagenta
			new Color(193,  156,    0),				// DarkYellow
			new Color(204,  204,    204),			// Gray
			new Color(118,  118,    118),			// DarkGray
			new Color(59,   120,    255),			// Blue
			new Color(22,   192,    12),			// Green
			new Color(97,   214,    214),			// Cyan
			new Color(231,  72,     86),			// Red
			new Color(180,  0,      158),			// Magenta
			new Color(249,  241,    165),			// Yellow
			new Color(242,  242,    242),			// White
		};


        /// <summary> Color constants for ease of use ex: Palettes.BlUE</summary>
        public static readonly int NULL = -1;
        public static readonly int BLACK = 0;
        public static readonly int DARK_BLUE = 1;
        public static readonly int DARK_GREEN = 2;
        public static readonly int DARK_CYAN = 3;
        public static readonly int DARK_RED = 4;
        public static readonly int DARK_MAGENTA = 5;
        public static readonly int DARK_YELLOW = 6;
        public static readonly int GRAY = 7;
        public static readonly int DARK_GRAY = 8;
        public static readonly int BLUE = 9;
        public static readonly int GREEN = 10;
        public static readonly int CYAN = 11;
        public static readonly int RED = 12;
        public static readonly int MAGENTA = 13;
        public static readonly int YELLOW = 14;
        public static readonly int WHITE = 15;

        private static string[] COLOR_NAME = new string[]
        {
            "Black","Dark Blue","Dark Green","Dark Cyan","Dark Red","Dark Magenta","Dark Yellow","Gray","Dark Gray","Blue","Green","Cyan","Red","Magenta","Yellow","White"
        };

        /// <summary>toString function, which returns the string name of the color</summary>
        /// <param name="colorPosition">position in array</param>
        /// <returns>the name of the color in the palette array</returns>
        public static string ColorName(int colorPosition)
        {
            if (colorPosition < 0 || colorPosition > COLOR_NAME.Length)
                return "null";
            return COLOR_NAME[colorPosition];
        }


    }
}
