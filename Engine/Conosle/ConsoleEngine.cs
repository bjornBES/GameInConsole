using System.Data;
using System.Text;
using GameInConsoleEngine.Engine.Tools;
using GameInConsoleEngine.Resource;
using SDL2;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameInConsoleEngine.Engine
{
    /// <summary>
    /// Class for Drawing to a console window.
    /// </summary>
    public class ConsoleEngine
    {
        const int MAX_COLOR = 256;

        public IntPtr window = IntPtr.Zero;
        public IntPtr renderer = IntPtr.Zero;

        /// <summary> The active color palette. </summary> <see cref="Color"/>
        public Color[] Palette { get; private set; }

        /// <summary> The current size of the font. </summary> <see cref="Point"/>
        public Point FontSize { get; private set; }

        /// <summary> The dimensions of the window in characters. </summary> <see cref="Point"/>
        public Point WindowSize { get; private set; }

        /*private char[,] CharBuffer { get; set; }
        private int[,] ColorBuffer { get; set; }
        private int[,] BackgroundBuffer { get; set; }*/
        private Glyph[,] GlyphBuffer { get; set; }
        private int Background { get; set; }
        private ConsoleBuffer ConsoleBuffer { get; set; }
        private bool IsBorderless { get; set; }
        private SDL.SDL_KeyboardEvent keyboardEvent;
        private SDL.SDL_MouseWheelEvent mouseWheelEvent;
        private SDL.SDL_MouseButtonEvent mouseButtonEvent;
        private SDL.SDL_MouseMotionEvent mouseMotionEvent;
        public bool Exit { get; private set; } = false;
        public bool RunEvents = false;


        /// <summary> Creates a new ConsoleEngine. </summary>
        /// <param name="width">Target window width.</param>
        /// <param name="height">Target window height.</param>
        /// <param name="fontW">Target font width.</param>
        /// <param name="fontH">Target font height.</param>
        public ConsoleEngine(int width, int height, int fontW, int fontH, IntPtr _window, IntPtr _renderer)
        {
            if (width < 1 || height < 1)
                throw new ArgumentOutOfRangeException();
            if (fontW < 1 || fontH < 1)
                throw new ArgumentOutOfRangeException();

            renderer = _renderer;
            window = _window;

            SDL.SDL_SetWindowTitle(window, "test");
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255); // black color
            SDL.SDL_RenderClear(renderer);
            SDL.SDL_RenderPresent(renderer);


            // NativeMethods.ConsoleCursorInfo consoleCursorInfo = new NativeMethods.ConsoleCursorInfo()
            // {
            //     bVisible = false,
            //     dwSize = 100,
            // };

            // NativeMethods.SetConsoleTitle("test");
            // unsafe
            // {
            //     NativeMethods.SetConsoleCursorInfo(stdOutputHandle, (IntPtr)(&consoleCursorInfo));
            // }

            //sets console location to 0,0 to try and avoid the error where the console is to big
            // NativeMethods.SetWindowPos(consoleHandle, 0, 0, 0, 0, 0, 0x0046);


            ConsoleBuffer = new ConsoleBuffer(width, height);

            WindowSize = new Point(width, height);
            FontSize = new Point(fontW, fontH);

            // CharBuffer = new char[width, height];
            // ColorBuffer = new int[width, height];
            // BackgroundBuffer = new int[width, height];

            GlyphBuffer = new Glyph[width, height];
            for (int y = 0; y < GlyphBuffer.GetLength(1); y++)
                for (int x = 0; x < GlyphBuffer.GetLength(0); x++)
                    GlyphBuffer[x, y] = new Glyph();

            // NativeMethods.Coord size = new NativeMethods.Coord((short)width, (short)height);
            // NativeMethods.SetConsoleScreenBufferSize(stdOutputHandle, size);

            // NativeMethods.SmallRect windowsize = new NativeMethods.SmallRect(0, 0, (short)(width - 1), (short)(height - 1));
            // NativeMethods.SetConsoleWindowInfo(stdOutputHandle, true, ref windowsize);

            // here we make it so we can have more then 16 colors
            // if (NativeMethods.GetConsoleMode(stdOutputHandle, out uint mode))
            // {
            //     mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            //     NativeMethods.SetConsoleMode(stdOutputHandle, mode);
            // }

            SetBackground(0);
            SetPalette(Palettes.vgaColors);

            RunEvents = true;
            Thread eventThread = new Thread(new ThreadStart(getEvents));
            eventThread.Start();

            // NativeMethods.SetConsoleMode(stdInputHandle, 0x0080);

            // ConsoleFont.SetFont(stdOutputHandle, (short)fontW, (short)fontH);
        }

        public void getEvents()
        {
            while (RunEvents)
            {
                if (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_KEYUP:
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            keyboardEvent = e.key;
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
                            mouseMotionEvent = e.motion;
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                            mouseWheelEvent = e.wheel;
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            mouseButtonEvent = e.button;
                            break;
                        case SDL.SDL_EventType.SDL_QUIT:
                            RunEvents = false;
                            Exit = true;
                            break;
                    }
                }
            }
        }

        public int[] GetColorIndex(Color[] colors)
        {
            List<int> ints = new List<int>();
            for (int i = 0; i < colors.Length; i++)
            {
                Color color = colors[i];
                for (int colorMargin = 25; colorMargin < 255; colorMargin += 5)
                {
                    int index = color.NearToColor(Palette, colorMargin);
                    if (index != -1)
                    {
                        ints.Add(index);
                        break;
                    }
                }
            }
            return ints.ToArray();
        }
        public void PrintImage(Point startPoint, int[] image, int imageWidth, int imageHeight)
        {
            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    int index = y * imageWidth + x;
                    Point point = new Point(x, y) + startPoint;

                    if (point.Y == imageHeight)
                    {
                        return;
                    }
                    if (point.X == imageWidth)
                    {
                        break;
                    }

                    if (image.Length == index)
                    {
                        return;
                    }
                    SetPixel(point, image[index]);
                }
            }
        }
        public void PrintImage(Point startPoint, int[] imageData, Image<Rgba32> image)
        {
            DateTime dateTimeNow = DateTime.Now;
            PrintImage(startPoint, imageData, image.Width, image.Height);
            double diff = (DateTime.Now - dateTimeNow).TotalSeconds;
        }
        // Rita
        public void SetPixel(Point selectedPoint, int color, char character)
        {
            SetPixel(selectedPoint, color, Background, character);
        }

        //new Draw method, which supports background
        public void SetPixel(Point selectedPoint, int fgColor, int bgColor, char character)
        {
            if (selectedPoint.X >= GlyphBuffer.GetLength(0) || selectedPoint.Y >= GlyphBuffer.GetLength(1)
                || selectedPoint.X < 0 || selectedPoint.Y < 0)
                return;

            /*CharBuffer[selectedPoint.X, selectedPoint.Y] = character;
            ColorBuffer[selectedPoint.X, selectedPoint.Y] = fgColor;
            BackgroundBuffer[selectedPoint.X, selectedPoint.Y] = bgColor;*/
            GlyphBuffer[selectedPoint.X, selectedPoint.Y].set(character, Palette[fgColor], Palette[bgColor]);
        }

        /// <summary>
        /// returns gylfh at point given
        /// </summary>
        /// <param name="selectedPoint"></param>
        /// <returns></returns>
        public Glyph? PixelAt(Point selectedPoint)
        {
            if (selectedPoint.X > 0 && selectedPoint.X < GlyphBuffer.GetLength(0) && selectedPoint.Y > 0 && selectedPoint.Y < GlyphBuffer.GetLength(1))
                return GlyphBuffer[selectedPoint.X, selectedPoint.Y];
            else
                return null;
        }


        /// <summary> Sets the console's color palette </summary>
        /// <param name="colors"></param>
        /// <exception cref="ArgumentException"/> <exception cref="ArgumentNullException"/>
        public void SetPalette(Color[] colors)
        {
            if (colors.Length > MAX_COLOR)
                throw new ArgumentException($"Windows command prompt only support {MAX_COLOR} colors.");
            Palette = colors ?? throw new ArgumentNullException();


            unsafe
            {
                SDL.SDL_Palette* palette = (SDL.SDL_Palette*)SDL.SDL_AllocPalette(MAX_COLOR).ToPointer();
                (*palette).refcount = colors.Length;
                (*palette).version = 0;
                (*palette).ncolors = colors.Length;
                SDL.SDL_Color[] SDLcolors = new SDL.SDL_Color[colors.Length];
                for (int i = 0; i < colors.Length; i++)
                {
                    SDL.SDL_Color c = new SDL.SDL_Color();
                    c.r = (byte)colors[i].R;
                    c.g = (byte)colors[i].G;
                    c.b = (byte)colors[i].B;
                    c.a = 255;
                    SDLcolors[i] = c;
                }
                if (SDL.SDL_SetPaletteColors((IntPtr)palette, SDLcolors, 0, SDLcolors.Length) != 0)
                {
                    Console.WriteLine($"Error Setting the patette{Environment.NewLine}{SDL.SDL_GetError()}");
                    SDL.SDL_FreePalette((IntPtr)palette);
                    Environment.Exit(1);
                }
                SDL.SDL_FreePalette((IntPtr)palette);
            }

        }

        public void SetFont(string resourceKeyImage, string resourceKeyFontHeight, string resourceKeyFontWidth, string resourceKeyCharsPerRow, string resourceKeyCharsPerCol)
        {
            string fontPath = Resources.GetEntry(resourceKeyImage).value;
            int fontHeight = int.Parse(Resources.GetEntry(resourceKeyFontHeight).value);
            int fontWidth = int.Parse(Resources.GetEntry(resourceKeyFontWidth).value);
            int charsPerRow = int.Parse(Resources.GetEntry(resourceKeyCharsPerRow).value);
            int charsPerCol = int.Parse(Resources.GetEntry(resourceKeyCharsPerCol).value);
            SetFont(fontPath, fontHeight, fontWidth, charsPerRow, charsPerCol);
        }
        public void SetFont(string fontPath, int fontHeight, int fontWidth, int charsPerRow, int charsPerCol)
        {
            if (File.Exists(fontPath))
            {
                FontTools.GetImageBitmap(fontPath, fontWidth, fontHeight, charsPerRow, charsPerCol);
            }
        }

        /// <summary> Sets the console's background color to one in the active palette. </summary>
        /// <param name="color">Index of background color in palette.</param>
        public void SetBackground(int color = 0)
        {
            if (color > MAX_COLOR || color < 0)
                throw new IndexOutOfRangeException();
            Background = color;
        }

        public void SetTitle(string title)
        {
            SDL.SDL_SetWindowTitle(window, title);
        }

        /// <summary>Gets Background</summary>
        /// <returns>Returns the background</returns>
        public int GetBackground()
        {
            return Background;
        }

        /// <summary> Clears the screenbuffer. </summary>
        public void ClearBuffer()
        {
            DateTime dateTimeNow = DateTime.Now;
            /*Array.Clear(CharBuffer, 0, CharBuffer.Length);
            Array.Clear(ColorBuffer, 0, ColorBuffer.Length);
            Array.Clear(BackgroundBuffer, 0, BackgroundBuffer.Length);*/
            for (int y = 0; y < GlyphBuffer.GetLength(1); y++)
                for (int x = 0; x < GlyphBuffer.GetLength(0); x++)
                    GlyphBuffer[x, y] = new Glyph();
            double diff = (DateTime.Now - dateTimeNow).TotalSeconds;
        }

        /// <summary> Blits the screenbuffer to the Console window. </summary>
        public void DisplayBuffer()
        {
            ConsoleBuffer.SetBuffer(GlyphBuffer, Palette[Background]);
            ConsoleBuffer.Blit(renderer, Palette[Background]);
        }

        #region Primitives

        /// <summary> Draws a single pixel to the screenbuffer. calls new method with Background as the bgColor </summary>
        /// <param name="v">The Point that should be drawn to.</param>
        /// <param name="color">The color index.</param>
        /// <param name="c">The character that should be drawn with.</param>
        public void SetPixel(Point v, int color, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            SetPixel(v, color, Background, (char)c);
        }

        /// <summary> Overloaded Method Draws a single pixel to the screenbuffer with custom bgColor. </summary>
        /// <param name="v">The Point that should be drawn to.</param>
        /// <param name="fgColor">The foreground color index.</param>
        /// <param name="bgColor">The background color index.</param>
        /// <param name="c">The character that should be drawn with.</param>
        public void SetPixel(Point v, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            SetPixel(v, fgColor, bgColor, (char)c);
        }

        /// <summary> Draws a frame using boxdrawing symbols, calls new method with Background as the bgColor. </summary>
        /// <param name="pos">Top Left corner of box.</param>
        /// <param name="end">Bottom Right corner of box.</param>
        /// <param name="color">The specified color index.</param>
        public void Frame(Point pos, Point end, int color)
        {
            Frame(pos, end, color, Background);
        }

        /// <summary> Draws a frame using boxdrawing symbols. </summary>
        /// <param name="pos">Top Left corner of box.</param>
        /// <param name="end">Bottom Right corner of box.</param>
        /// <param name="fgColor">The specified color index.</param>
        /// <param name="bgColor">The specified background color index.</param>
        public void Frame(Point pos, Point end, int fgColor, int bgColor)
        {
            for (int i = 1; i < end.X - pos.X; i++)
            {
                SetPixel(new Point(pos.X + i, pos.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_H);
                SetPixel(new Point(pos.X + i, end.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_H);
            }

            for (int i = 1; i < end.Y - pos.Y; i++)
            {
                SetPixel(new Point(pos.X, pos.Y + i), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_V);
                SetPixel(new Point(end.X, pos.Y + i), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_V);
            }

            SetPixel(new Point(pos.X, pos.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_DR);
            SetPixel(new Point(end.X, pos.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_DL);
            SetPixel(new Point(pos.X, end.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_UR);
            SetPixel(new Point(end.X, end.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_UL);
        }

        /// <summary> Writes plain text to the buffer, calls new method with Background as the bgColor. </summary>
        /// <param name="pos">The position to write to.</param>
        /// <param name="text">String to write.</param>
        /// <param name="color">Specified color index to write with.</param>
        public void WriteText(Point pos, string text, int color)
        {
            DateTime dateTimeNow = DateTime.Now;
            WriteText(pos, text, color, Background);
            double diff = (DateTime.Now - dateTimeNow).TotalMilliseconds;
        }

        /// <summary> Writes plain text to the buffer. </summary>
        /// <param name="pos">The position to write to.</param>
        /// <param name="text">String to write.</param>
        /// <param name="fgColor">Specified color index to write with.</param>
        /// <param name="bgColor">Specified background color index to write with.</param>
        public void WriteText(Point pos, string text, int fgColor, int bgColor)
        {
            DateTime dateTimeNow = DateTime.Now;
            for (int i = 0; i < text.Length; i++)
            {
                Point point = new Point((pos.X + i) * FontTools.fontWidth, pos.Y * FontTools.fontHeight);
                WriteText(point, text[i], fgColor, bgColor);
            }
            double diff = (DateTime.Now - dateTimeNow).TotalSeconds;
        }

        /// <summary> Writes plain text to the buffer. </summary>
        /// <param name="pos">The position to write to.</param>
        /// <param name="c">Char to write.</param>
        /// <param name="fgColor">Specified color index to write with.</param>
        /// <param name="bgColor">Specified background color index to write with.</param>
        public void WriteText(Point pos, char c, int fgColor, int bgColor)
        {
            DateTime dateTimeNow = DateTime.Now;
            int charIndex = (byte)c;
            Color[] colors = FontTools.GetChar(charIndex);
            for (int y = 0; y < FontTools.fontHeight; y++)
            {
                for (int x = 0; x < FontTools.fontWidth; x++)
                {
                    int i = y * FontTools.fontWidth + x;
                    Color color = colors[i];
                    char cToPrint = c;
                    if (color.AllColor > 0) // have a color
                    {
                        color = Palette[fgColor];
                    }
                    else // background
                    {
                        cToPrint = '\0';
                    }
                    int colorIndex = color.NearToColor(Palette, 25);
                    SetPixel(pos + new Point(x, y), colorIndex, bgColor, cToPrint);
                }
            }
            double diff = (DateTime.Now - dateTimeNow).TotalMilliseconds;
        }

        /// <summary>  Writes text to the buffer in a FIGlet font, calls new method with Background as the bgColor. </summary>
        /// <param name="pos">The Top left corner of the text.</param>
        /// <param name="text">String to write.</param>
        /// <param name="font">FIGLET font to write with.</param>
        /// <param name="color">Specified color index to write with.</param>
        /// <see cref="FigletFont"/>
        public void WriteFiglet(Point pos, string text, FigletFont font, int color)
        {
            WriteFiglet(pos, text, font, color, Background);
        }

        /// <summary>  Writes text to the buffer in a FIGlet font. </summary>
        /// <param name="pos">The Top left corner of the text.</param>
        /// <param name="text">String to write.</param>
        /// <param name="font">FIGLET font to write with.</param>
        /// <param name="fgColor">Specified color index to write with.</param>
        /// <param name="bgColor">Specified background color index to write with.</param>
        /// <see cref="FigletFont"/>
        public void WriteFiglet(Point pos, string text, FigletFont Ffont, int fgColor, int bgColor)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (Encoding.UTF8.GetByteCount(text) != text.Length)
                throw new ArgumentException("String contains non-ascii characters");

            int sWidth = FigletFont.GetStringWidth(Ffont, text);

            for (int line = 1; line <= Ffont.Height; line++)
            {
                int runningWidthTotal = 0;

                for (int c = 0; c < text.Length; c++)
                {
                    char character = text[c];
                    string fragment = FigletFont.GetCharacter(Ffont, character, line);
                    for (int f = 0; f < fragment.Length; f++)
                    {
                        if (fragment[f] != ' ')
                        {
                            Point point = new Point((pos.X + runningWidthTotal + f) * FontTools.fontWidth, (pos.Y + line - 1) * FontTools.fontHeight);
                            WriteText(point, fragment[f], fgColor, bgColor);
                        }
                    }
                    runningWidthTotal += fragment.Length;
                }
            }
        }

        /// <summary> Draws an Arc, calls new method with Background as the bgColor. </summary>
        /// <param name="pos">Center of Arc.</param>
        /// <param name="radius">Radius of Arc.</param>
        /// <param name="color">Specified color index.</param>
        /// <param name="arc">angle in degrees, 360 if not specified.</param>
        /// <param name="c">Character to use.</param>
        public void Arc(Point pos, int radius, int color, int arc = 360, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            Arc(pos, radius, color, Background, arc, c);
        }

        /// <summary> Draws an Arc. </summary>
        /// <param name="pos">Center of Arc.</param>
        /// <param name="radius">Radius of Arc.</param>
        /// <param name="fgColor">Specified color index.</param>
        /// <param name="bgColor">Specified background color index.</param>
        /// <param name="arc">angle in degrees, 360 if not specified.</param>
        /// <param name="c">Character to use.</param>
        public void Arc(Point pos, int radius, int fgColor, int bgColor, int arc = 360, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            for (int a = 0; a < arc; a++)
            {
                int x = (int)(radius * Math.Cos(a / 57.29577f));
                int y = (int)(radius * Math.Sin(a / 57.29577f));

                Point v = new Point(pos.X + x, pos.Y + y);
                SetPixel(v, fgColor, bgColor, ConsoleCharacter.Full);
            }
        }

        /// <summary> Draws a filled Arc, calls new method with Background as the bgColor </summary>
        /// <param name="pos">Center of Arc.</param>
        /// <param name="radius">Radius of Arc.</param>
        /// <param name="start">Start angle in degrees.</param>
        /// <param name="arc">End angle in degrees.</param>
        /// <param name="color">Specified color index.</param>
        /// <param name="c">Character to use.</param>
        public void SemiCircle(Point pos, int radius, int start, int arc, int color, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            SemiCircle(pos, radius, start, arc, color, Background, c);
        }

        /// <summary> Draws a filled Arc. </summary>
        /// <param name="pos">Center of Arc.</param>
        /// <param name="radius">Radius of Arc.</param>
        /// <param name="start">Start angle in degrees.</param>
        /// <param name="arc">End angle in degrees.</param>
        /// <param name="fgColor">Specified color index.</param>
        /// <param name="bgColor">Specified background color index.</param>
        /// <param name="c">Character to use.</param>
        public void SemiCircle(Point pos, int radius, int start, int arc, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            for (int a = start; a > -arc + start; a--)
            {
                for (int r = 0; r < radius + 1; r++)
                {
                    int x = (int)(r * Math.Cos(a / 57.29577f));
                    int y = (int)(r * Math.Sin(a / 57.29577f));

                    Point v = new Point(pos.X + x, pos.Y + y);
                    SetPixel(v, fgColor, bgColor, c);
                }
            }
        }

        // Bresenhams Line Algorithm
        // https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
        /// <summary> Draws a line from start to end. (Bresenhams Line), calls overloaded method with background as bgColor </summary>
        /// <param name="start">Point to draw line from.</param>
        /// <param name="end">Point to end line at.</param>
        /// <param name="color">Color to draw with.</param>
        /// <param name="c">Character to use.</param>
        public void Line(Point start, Point end, int color, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            Line(start, end, color, Background, c);
        }

        // Bresenhams Line Algorithm
        // https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
        /// <summary> Draws a line from start to end. (Bresenhams Line) </summary>
        /// <param name="start">Point to draw line from.</param>
        /// <param name="end">Point to end line at.</param>
        /// <param name="fgColor">Color to draw with.</param>
        /// <param name="bgColor">Color to draw the background with.</param>
        /// <param name="c">Character to use.</param>
        public void Line(Point start, Point end, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            Point delta = end - start;
            Point da = Point.Zero, db = Point.Zero;
            if (delta.X < 0)
                da.X = -1;
            else if (delta.X > 0)
                da.X = 1;
            if (delta.Y < 0)
                da.Y = -1;
            else if (delta.Y > 0)
                da.Y = 1;
            if (delta.X < 0)
                db.X = -1;
            else if (delta.X > 0)
                db.X = 1;
            int longest = Math.Abs(delta.X);
            int shortest = Math.Abs(delta.Y);

            if (!(longest > shortest))
            {
                longest = Math.Abs(delta.Y);
                shortest = Math.Abs(delta.X);
                if (delta.Y < 0)
                    db.Y = -1;
                else if (delta.Y > 0)
                    db.Y = 1;
                db.X = 0;
            }

            int numerator = longest >> 1;
            Point p = new Point(start.X, start.Y);
            for (int i = 0; i <= longest; i++)
            {
                SetPixel(p, fgColor, bgColor, c);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    p += da;
                }
                else
                {
                    p += db;
                }
            }
        }

        /// <summary> Draws a Rectangle, calls overloaded method with background as bgColor  </summary>
        /// <param name="pos">Top Left corner of rectangle.</param>
        /// <param name="end">Bottom Right corner of rectangle.</param>
        /// <param name="color">Color to draw with.</param>
        /// <param name="c">Character to use.</param>
        public void Rectangle(Point pos, Point end, int color, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            Rectangle(pos, end, color, Background, c);
        }

        /// <summary> Draws a Rectangle. </summary>
        /// <param name="pos">Top Left corner of rectangle.</param>
        /// <param name="end">Bottom Right corner of rectangle.</param>
        /// <param name="fgColor">Color to draw with.</param>
        /// <param name="bgColor">Color to draw to the background with.</param>
        /// <param name="c">Character to use.</param>
        public void Rectangle(Point pos, Point end, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            for (int i = 0; i < end.X - pos.X; i++)
            {
                SetPixel(new Point(pos.X + i, pos.Y), fgColor, bgColor, c);
                SetPixel(new Point(pos.X + i, end.Y), fgColor, bgColor, c);
            }

            for (int i = 0; i < end.Y - pos.Y + 1; i++)
            {
                SetPixel(new Point(pos.X, pos.Y + i), fgColor, bgColor, c);
                SetPixel(new Point(end.X, pos.Y + i), fgColor, bgColor, c);
            }
        }

        /// <summary> Draws a Rectangle and fills it, calls overloaded method with background as bgColor </summary>
        /// <param name="a">Top Left corner of rectangle.</param>
        /// <param name="b">Bottom Right corner of rectangle.</param>
        /// <param name="color">Color to draw with.</param>
        /// <param name="c">Character to use.</param>
        public void Fill(Point a, Point b, int color, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            Fill(a, b, color, Background, c);
        }

        /// <summary> Draws a Rectangle and fills it. </summary>
        /// <param name="a">Top Left corner of rectangle.</param>
        /// <param name="b">Bottom Right corner of rectangle.</param>
        /// <param name="fgColor">Color to draw with.</param>
        /// <param name="bgColor">Color to draw the background with.</param>
        /// <param name="c">Character to use.</param>
        public void Fill(Point a, Point b, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            for (int y = a.Y; y < b.Y; y++)
            {
                for (int x = a.X; x < b.X; x++)
                {
                    SetPixel(new Point(x, y), fgColor, bgColor, c);
                }
            }
        }

        /// <summary> Draws a grid, calls overloaded method with background as bgColor </summary>
        /// <param name="a">Top Left corner of grid.</param>
        /// <param name="b">Bottom Right corner of grid.</param>
        /// <param name="spacing">the spacing until next line</param>
        /// <param name="color">Color to draw with.</param>
        /// <param name="c">Character to use.</param>
        public void Grid(Point a, Point b, int spacing, int color, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            Grid(a, b, spacing, color, Background, c);
        }

        /// <summary> Draws a grid. </summary>
        /// <param name="a">Top Left corner of grid.</param>
        /// <param name="b">Bottom Right corner of grid.</param>
        /// <param name="spacing">the spacing until next line</param>
        /// <param name="fgColor">Color to draw with.</param>
        /// <param name="bgColor">Color to draw the background with.</param>
        /// <param name="c">Character to use.</param>
        public void Grid(Point a, Point b, int spacing, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
        {
            for (int y = a.Y; y < b.Y / spacing; y++)
            {
                Line(new Point(a.X, y * spacing), new Point(b.X, y * spacing), fgColor, bgColor, c);
            }
            for (int x = a.X; x < b.X / spacing; x++)
            {
                Line(new Point(x * spacing, a.Y), new Point(x * spacing, b.Y), fgColor, bgColor, c);
            }
        }

        /// <summary> Draws a Triangle, calls overloaded method with background as bgColor </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="c">Point C.</param>
        /// <param name="color">Color to draw with.</param>
        /// <param name="character">Character to use.</param>
        public void Triangle(Point a, Point b, Point c, int color, ConsoleCharacter character = ConsoleCharacter.Full)
        {
            Triangle(a, b, c, color, Background, character);
        }

        /// <summary> Draws a Triangle. </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="c">Point C.</param>
        /// <param name="fgColor">Color to draw with.</param>
        /// <param name="bgColor">Color to draw to the background with.</param>
        /// <param name="character">Character to use.</param>
        public void Triangle(Point a, Point b, Point c, int fgColor, int bgColor, ConsoleCharacter character = ConsoleCharacter.Full)
        {
            Line(a, b, fgColor, bgColor, character);
            Line(b, c, fgColor, bgColor, character);
            Line(c, a, fgColor, bgColor, character);
        }

        // Bresenhams Triangle Algorithm

        /// <summary> Draws a Triangle and fills it, calls overloaded method with background as bgColor </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="c">Point C.</param>
        /// <param name="color">Color to draw with.</param>
        /// <param name="character">Character to use.</param>
        public void FillTriangle(Point a, Point b, Point c, int color, ConsoleCharacter character = ConsoleCharacter.Full)
        {
            FillTriangle(a, b, c, color, Background, character);
        }

        /// <summary> Draws a Triangle and fills it. </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="c">Point C.</param>
        /// <param name="fgColor">Color to draw with.</param>
        /// <param name="bgColor">Color to draw to the background with.</param>
        /// <param name="character">Character to use.</param>
        public void FillTriangle(Point a, Point b, Point c, int fgColor, int bgColor, ConsoleCharacter character = ConsoleCharacter.Full)
        {
            Point min = new Point(Math.Min(Math.Min(a.X, b.X), c.X), Math.Min(Math.Min(a.Y, b.Y), c.Y));
            Point max = new Point(Math.Max(Math.Max(a.X, b.X), c.X), Math.Max(Math.Max(a.Y, b.Y), c.Y));

            Point p = new Point();
            for (p.Y = min.Y; p.Y < max.Y; p.Y++)
            {
                for (p.X = min.X; p.X < max.X; p.X++)
                {
                    int w0 = Orient(b, c, p);
                    int w1 = Orient(c, a, p);
                    int w2 = Orient(a, b, p);

                    if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                        SetPixel(p, fgColor, bgColor, character);
                }
            }
        }

        private int Orient(Point a, Point b, Point c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        #endregion Primitives

        // Input

        /// <summary>Checks to see if the console is in focus </summary>
        /// <returns>True if Console is in focus</returns>
        private bool ConsoleFocused()
        {
            // TODO
            //return NativeMethods.GetConsoleWindow() == NativeMethods.GetForegroundWindow();
            return false;
        }

        /// <summary> Checks if specified key is pressed. </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if key is pressed</returns>
        public bool GetKey(ConsoleKey key)
        {
            if (keyboardEvent.type == SDL.SDL_EventType.SDL_KEYUP)
            {
                SDL.SDL_Scancode keycode = keyboardEvent.keysym.scancode;
                ConsoleKey consoleKey = SDL_KeycodeToConsoleKey(keycode);
                if (consoleKey == key)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        /// <summary> Checks if specified keyCode is pressed. </summary>
        /// <param name="virtualkeyCode">keycode to check</param>
        /// <returns>True if key is pressed</returns>
        public bool GetKey(int virtualkeyCode)
        {
            return GetKey((ConsoleKey)virtualkeyCode);
        }

        /// <summary> Checks if specified key is pressed down. </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if key is down</returns>
        public bool GetKeyDown(ConsoleKey key)
        {
            if (keyboardEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                SDL.SDL_Scancode keycode = keyboardEvent.keysym.scancode;
                ConsoleKey consoleKey = SDL_KeycodeToConsoleKey(keycode);
                if (consoleKey == key)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        /// <summary> Checks if specified keyCode is pressed down. </summary>
        /// <param name="virtualkeyCode">keycode to check</param>
        /// <returns>True if key is down</returns>
        public bool GetKeyDown(int virtualkeyCode)
        {
            return GetKeyDown((ConsoleKey)virtualkeyCode);
        }

        /// <summary> Checks if left mouse button is pressed down. </summary>
        /// <returns>True if left mouse button is down</returns>
        public bool GetMouseLeft()
        {
            if (mouseButtonEvent.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
            {
                if (mouseButtonEvent.button == SDL.SDL_BUTTON_LEFT)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        /// <summary> Checks if right mouse button is pressed down. </summary>
        /// <returns>True if right mouse button is down</returns>
        public bool GetMouseRight()
        {
            if (mouseButtonEvent.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
            {
                if (mouseButtonEvent.button == SDL.SDL_BUTTON_RIGHT)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        /// <summary> Checks if middle mouse button is pressed down. </summary>
        /// <returns>True if middle mouse button is down</returns>
        public bool GetMouseMiddle()
        {
            if (mouseButtonEvent.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
            {
                if (mouseButtonEvent.button == SDL.SDL_BUTTON_MIDDLE)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        /// <summary> Gets the mouse position. </summary>
        /// <returns>The mouse's position in character-space.</returns>
        public Point GetMousePos()
        {
            Point point = new Point(mouseMotionEvent.x, mouseMotionEvent.y);
            return new Point(Utility.Clamp(point.X, 0, WindowSize.X - 1), Utility.Clamp(point.Y, 0, WindowSize.Y - 1));
        }

        private ConsoleKey SDL_KeycodeToConsoleKey(SDL.SDL_Scancode keycode)
        {
            switch (keycode)
            {
                case SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN:
                    return ConsoleKey.None;
                case SDL.SDL_Scancode.SDL_SCANCODE_A:
                case SDL.SDL_Scancode.SDL_SCANCODE_B:
                case SDL.SDL_Scancode.SDL_SCANCODE_C:
                case SDL.SDL_Scancode.SDL_SCANCODE_D:
                case SDL.SDL_Scancode.SDL_SCANCODE_E:
                case SDL.SDL_Scancode.SDL_SCANCODE_F:
                case SDL.SDL_Scancode.SDL_SCANCODE_G:
                case SDL.SDL_Scancode.SDL_SCANCODE_H:
                case SDL.SDL_Scancode.SDL_SCANCODE_I:
                case SDL.SDL_Scancode.SDL_SCANCODE_J:
                case SDL.SDL_Scancode.SDL_SCANCODE_K:
                case SDL.SDL_Scancode.SDL_SCANCODE_L:
                case SDL.SDL_Scancode.SDL_SCANCODE_M:
                case SDL.SDL_Scancode.SDL_SCANCODE_N:
                case SDL.SDL_Scancode.SDL_SCANCODE_O:
                case SDL.SDL_Scancode.SDL_SCANCODE_P:
                case SDL.SDL_Scancode.SDL_SCANCODE_Q:
                case SDL.SDL_Scancode.SDL_SCANCODE_R:
                case SDL.SDL_Scancode.SDL_SCANCODE_S:
                case SDL.SDL_Scancode.SDL_SCANCODE_T:
                case SDL.SDL_Scancode.SDL_SCANCODE_U:
                case SDL.SDL_Scancode.SDL_SCANCODE_V:
                case SDL.SDL_Scancode.SDL_SCANCODE_W:
                case SDL.SDL_Scancode.SDL_SCANCODE_X:
                case SDL.SDL_Scancode.SDL_SCANCODE_Y:
                case SDL.SDL_Scancode.SDL_SCANCODE_Z:
                    return (ConsoleKey)(((ushort)keycode) + 61);
                case SDL.SDL_Scancode.SDL_SCANCODE_1:
                case SDL.SDL_Scancode.SDL_SCANCODE_2:
                case SDL.SDL_Scancode.SDL_SCANCODE_3:
                case SDL.SDL_Scancode.SDL_SCANCODE_4:
                case SDL.SDL_Scancode.SDL_SCANCODE_5:
                case SDL.SDL_Scancode.SDL_SCANCODE_6:
                case SDL.SDL_Scancode.SDL_SCANCODE_7:
                case SDL.SDL_Scancode.SDL_SCANCODE_8:
                case SDL.SDL_Scancode.SDL_SCANCODE_9:
                    return (ConsoleKey)(((ushort)keycode) + 19);
                case SDL.SDL_Scancode.SDL_SCANCODE_0:
                    return ConsoleKey.D0;
                case SDL.SDL_Scancode.SDL_SCANCODE_RETURN:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_ENTER:
                    return ConsoleKey.Enter;
                case SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE:
                    return ConsoleKey.Escape;
                case SDL.SDL_Scancode.SDL_SCANCODE_BACKSPACE:
                    return ConsoleKey.Backspace;
                case SDL.SDL_Scancode.SDL_SCANCODE_TAB:
                    return ConsoleKey.Tab;
                case SDL.SDL_Scancode.SDL_SCANCODE_SPACE:
                    return ConsoleKey.Spacebar;
                case SDL.SDL_Scancode.SDL_SCANCODE_MINUS:
                    return ConsoleKey.OemMinus;
                case SDL.SDL_Scancode.SDL_SCANCODE_BACKSLASH:
                case SDL.SDL_Scancode.SDL_SCANCODE_SLASH:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_DIVIDE:
                    return ConsoleKey.Divide;
                case SDL.SDL_Scancode.SDL_SCANCODE_COMMA:
                    return ConsoleKey.OemComma;
                case SDL.SDL_Scancode.SDL_SCANCODE_PERIOD:
                    return ConsoleKey.OemPeriod;
                case SDL.SDL_Scancode.SDL_SCANCODE_F1:
                case SDL.SDL_Scancode.SDL_SCANCODE_F2:
                case SDL.SDL_Scancode.SDL_SCANCODE_F3:
                case SDL.SDL_Scancode.SDL_SCANCODE_F4:
                case SDL.SDL_Scancode.SDL_SCANCODE_F5:
                case SDL.SDL_Scancode.SDL_SCANCODE_F6:
                case SDL.SDL_Scancode.SDL_SCANCODE_F7:
                case SDL.SDL_Scancode.SDL_SCANCODE_F8:
                case SDL.SDL_Scancode.SDL_SCANCODE_F9:
                case SDL.SDL_Scancode.SDL_SCANCODE_F10:
                case SDL.SDL_Scancode.SDL_SCANCODE_F11:
                case SDL.SDL_Scancode.SDL_SCANCODE_F12:
                    return (ConsoleKey)(((ushort)keycode) + 54);
                case SDL.SDL_Scancode.SDL_SCANCODE_PRINTSCREEN:
                    return ConsoleKey.PrintScreen;
                case SDL.SDL_Scancode.SDL_SCANCODE_PAUSE:
                    return ConsoleKey.Pause;
                case SDL.SDL_Scancode.SDL_SCANCODE_INSERT:
                    return ConsoleKey.Insert;
                case SDL.SDL_Scancode.SDL_SCANCODE_HOME:
                    return ConsoleKey.Home;
                case SDL.SDL_Scancode.SDL_SCANCODE_PAGEUP:
                    return ConsoleKey.PageUp;
                case SDL.SDL_Scancode.SDL_SCANCODE_DELETE:
                    return ConsoleKey.Delete;
                case SDL.SDL_Scancode.SDL_SCANCODE_END:
                    return ConsoleKey.End;
                case SDL.SDL_Scancode.SDL_SCANCODE_PAGEDOWN:
                    return ConsoleKey.PageDown;
                case SDL.SDL_Scancode.SDL_SCANCODE_RIGHT:
                    return ConsoleKey.RightArrow;
                case SDL.SDL_Scancode.SDL_SCANCODE_LEFT:
                    return ConsoleKey.LeftArrow;
                case SDL.SDL_Scancode.SDL_SCANCODE_DOWN:
                    return ConsoleKey.DownArrow;
                case SDL.SDL_Scancode.SDL_SCANCODE_UP:
                    return ConsoleKey.UpArrow;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY:
                    return ConsoleKey.Multiply;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MINUS:
                    return ConsoleKey.Subtract;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_PLUS:
                    return ConsoleKey.OemPlus;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_1:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_2:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_3:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_4:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_5:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_6:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_7:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_8:
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_9:
                    return (ConsoleKey)(((ushort)keycode) - 40);
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_0:
                    return ConsoleKey.D0;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_PERIOD:
                    return ConsoleKey.OemPeriod;
                case SDL.SDL_Scancode.SDL_SCANCODE_APPLICATION:
                    return ConsoleKey.Applications;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_COMMA:
                    return ConsoleKey.OemComma;
            }
            return ConsoleKey.None;
        }
    }
}