using SDL2;

namespace GameInConsoleEngine.Engine
{
    public class PixelInfo
    {
        public SDL.SDL_Point Point;
        public Color foreGroundColor;
        public Color backGroundColor;
        public char character;
    }
    class ConsoleBuffer
    {
        private PixelInfo[] PixelInfoBuffer { get; set; }
        private SDL.SDL_Point[] PointBuffer { get; set; }

        readonly int width, height;

        public ConsoleBuffer(int w, int he)
        {
            width = w;
            height = he;

            PointBuffer = new SDL.SDL_Point[width * height];
            PixelInfoBuffer = new PixelInfo[width * height];
        }
        /// <summary>
        /// Sets the buffer to values
        /// </summary>
        /// <param name="GlyphBuffer"></param>
        /// <param name="charBuffer"> array of chars which get added to the buffer</param>
        /// <param name="colorBuffer"> array of foreground(front)colors which get added to the buffer</param>
        /// <param name="background"> array of background colors which get added to the buffer</param>
        /// <param name="defualtBackground"> default color(may reduce fps?), this is the background color
        ///									null chars will get set to this default background</param>
        public void SetBuffer(Glyph[,] GlyphBuffer, Color defualtBackground)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = y * width + x;

                    if (GlyphBuffer[x, y].c == 0)
                    {
                        GlyphBuffer[x, y].bg = defualtBackground;
                    }

                    PixelInfoBuffer[i] = new PixelInfo();
                    PointBuffer[i] = new SDL.SDL_Point() { x = x, y = y };
                    PixelInfoBuffer[i].Point = new SDL.SDL_Point() { x = x, y = y };
                    PixelInfoBuffer[i].foreGroundColor = GlyphBuffer[x, y].fg;
                    PixelInfoBuffer[i].backGroundColor = GlyphBuffer[x, y].bg;
                    PixelInfoBuffer[i].character = GlyphBuffer[x, y].c;
                }
            }
        }

        public void Blit(IntPtr renderer, Color backGroundColor)
        {
            DateTime dateTimeNow = DateTime.Now;
            SDL.SDL_RenderClear(renderer);

            SDL.SDL_Rect rect = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = width,
                h = height
            };

            SDL.SDL_Color backGround ; //= backGroundColor.GetSDLColor();
            // SDL.SDL_SetRenderDrawColor(renderer, backGround.r, backGround.g, backGround.b, backGround.a);
            // SDL.SDL_RenderDrawRect(renderer, ref rect);

            Color colorBuffer = null;

            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    int i = y * height + x;
                    PixelInfo pixelInfo = PixelInfoBuffer[i];
                    SDL.SDL_Color foreGround = pixelInfo.foreGroundColor.GetSDLColor();

                    if (pixelInfo.character != 0)
                    {
                        if (colorBuffer == null || colorBuffer != pixelInfo.foreGroundColor)
                        {
                            SDL.SDL_SetRenderDrawColor(renderer, foreGround.r, foreGround.g, foreGround.b, foreGround.a);
                            colorBuffer = pixelInfo.foreGroundColor;
                        }
                    }
                    else
                    {
                        if (colorBuffer == null || colorBuffer != pixelInfo.backGroundColor)
                        {
                            backGround = pixelInfo.backGroundColor.GetSDLColor();
                            SDL.SDL_SetRenderDrawColor(renderer, backGround.r, backGround.g, backGround.b, backGround.a);
                            colorBuffer = pixelInfo.backGroundColor;
                        }
                    }
                    SDL.SDL_RenderDrawPoint(renderer, x, y);
                }
            }
            SDL.SDL_RenderPresent(renderer);
            double diff = (DateTime.Now - dateTimeNow).TotalSeconds;

            // NativeMethods.SmallRect rect = new NativeMethods.SmallRect() { Left = 0, Top = 0, Right = (short)width, Bottom = (short)height };

            // NativeMethods.WriteConsoleOutputW(h, CharInfoBuffer,
            //     new NativeMethods.Coord() { X = (short)width, Y = (short)height },
            //     new NativeMethods.Coord() { X = 0, Y = 0 }, ref rect);
        }
    }
}
