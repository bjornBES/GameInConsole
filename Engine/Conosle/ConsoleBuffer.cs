using System.IO;
using GameInConsole.Engine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using StbImageSharp;
using StbImageWriteSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using Raylib_cs;
using System.Reflection.Emit;
using System.ComponentModel;

namespace GameInConsoleEngine.Engine
{
    struct charPixelInfo
    {
        public string str;
        public Point Point;
        public Raylib_cs.Color fgColor;
        public Raylib_cs.Color bgColor;
    }
    class ConsoleBuffer
    {
        string tempImgFile;
        readonly int width, height;
        byte[] PixelInfo;
        List<charPixelInfo> CharPixelInfo;

        public ConsoleBuffer(int w, int he)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "GameInConsole";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += Path.DirectorySeparatorChar + "Tmp";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += Path.DirectorySeparatorChar;

            tempImgFile = path + Path.GetTempFileName();
            width = w;
            height = he;
            PixelInfo = new byte[w * he * 4];
            CharPixelInfo = new List<charPixelInfo>(w * he);
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
        public void SetBuffer(Glyph[,] GlyphBuffer, Rgba32 defualtBackground)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = x + width * y;
                    Glyph glyph = GlyphBuffer[x, y];
                    if (EngineConfig.UseTextBasedGraphics)
                    {
                        Rgba32 fgColor = glyph.foreGround;
                        Rgba32 bgColor = glyph.backGround;
                        if (pixelIndex > 0)
                        {
                            charPixelInfo last = CharPixelInfo[^1];
                            if (last.str[0] == glyph.c && 
                                last.fgColor.ToRGBA32() == fgColor &&
                                last.bgColor.ToRGBA32() == bgColor)
                            {
                                charPixelInfo updated = last;
                                updated.str += glyph.c;
                                CharPixelInfo[^1] = updated;
                                continue;
                            }
                        }
                        {
                            charPixelInfo charPixel = new charPixelInfo()
                            { 
                                Point = new Point(x, y),
                                str = glyph.c.ToString(),
                                fgColor = fgColor.GetColor(),
                                bgColor = bgColor.GetColor()
                            };
                            CharPixelInfo.Add(charPixel);
                        }
                    }
                    else
                    {
                        byte[] colorBytes = glyph.foreGround.GetBytes();
                        PixelInfo[pixelIndex] = colorBytes[0];
                        PixelInfo[pixelIndex] = colorBytes[1];
                        PixelInfo[pixelIndex] = colorBytes[2];
                        PixelInfo[pixelIndex] = colorBytes[3];
                        File.WriteAllBytes(tempImgFile, PixelInfo);
                    }
                }
            }
        }

        public void Blit(Rgba32 bgColor, bool Running)
        {
            if (Running == false)
            {
                return;
            }

            DateTime dateTimeNow = DateTime.Now;

            if (EngineConfig.UseTextBasedGraphics)
            {
                for (int i = 0; i < CharPixelInfo.Count; i++)
                {
                    charPixelInfo charPixel = CharPixelInfo[i];
                    Raylib.DrawText(charPixel.str, charPixel.Point.X, charPixel.Point.Y, 8, charPixel.fgColor);
                }
            }
            else
            {
                Raylib_cs.Image img = Raylib.LoadImage(tempImgFile);
                Texture2D tex = Raylib.LoadTextureFromImage(img);
                Raylib.DrawTexture(tex, 0, 0, bgColor.GetColor());
            }

            double diff = (DateTime.Now - dateTimeNow).TotalMilliseconds;
        }
        public void Exit()
        {
            File.Delete(tempImgFile);
        }
    }
}
