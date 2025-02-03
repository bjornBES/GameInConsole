namespace GameInConsoleEngine.Engine
{
    public struct Glyph
    {
        public char c;
        public Color fg;
        public Color bg;

        public Glyph()
        {
            clear();
        }

        public void set(char c_, Color fg_, Color bg_) { c = c_; fg = fg_; bg = bg_; }

        public void clear() { c = (char)0; fg = Color.defaultColor; bg = Color.defaultColor; }
    }
}
