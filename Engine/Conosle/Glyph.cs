using GameInConsole.Engine;
using OpenTK.Mathematics;
using SixLabors.ImageSharp.PixelFormats;

namespace GameInConsoleEngine.Engine
{
    public struct Glyph
    {
        public char c;
        public Rgba32 foreGround;
        public Rgba32 backGround;

        public Glyph(Rgba32 backGroundColor)
        {
            clear(backGroundColor);
        }

        public void set(char c_, Rgba32 fg_, Rgba32 bg_)
        {
            c = c_;
            foreGround = fg_;
            backGround = bg_;
        }

        public void clear(Rgba32 backGroundColor) 
        { 
            c = '\0';
            foreGround = backGround = new Rgba32(0);
        }
    
        public byte[] toBytes()
        {
            byte[] fgResult =
            {
                foreGround.R,
                foreGround.G,
                foreGround.B,
                foreGround.A,
            };
            byte[] bgResult =
            {
                backGround.R,
                backGround.G,
                backGround.B,
                backGround.A,
            };
            if (c == '\0')
            {
                return bgResult;
            }
            else
            {
                return fgResult;
            }
        }
        
        public static implicit operator Color4(Glyph glyph)
        {
            if (glyph.c == '\0')
            {
                return glyph.backGround.GetColor4();
            }
            else
            {
                return glyph.foreGround.GetColor4();
            }
        }
        public static implicit operator Rgba32(Glyph glyph)
        {
            if (glyph.c == '\0')
            {
                return glyph.backGround;
            }
            else
            {
                return glyph.foreGround;
            }
        }
        public static implicit operator byte[](Glyph glyph)
        {
            if (glyph.c == '\0')
            {
                return BitConverter.GetBytes(glyph.backGround.PackedValue);
            }
            else
            {
                return BitConverter.GetBytes(glyph.foreGround.PackedValue);
            }
        }
    }
}
