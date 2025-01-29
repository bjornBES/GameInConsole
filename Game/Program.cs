using GameInConsoleEngine.Resources;
using GameInConsoleEngine.Engine;
using GameInConsoleEngine.Engine.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class Program : ConsoleGame
{
    private static void Main(string[] args)
    {
        new Program().Construct(200, 800, 4, 8);
    }
    
    public override void Start()
    {
        TargetFramerate = 15;

        Resources.Load("./Font.flf", "FFont");
        Resources.Load("./Image1.png", "testImage");
    }
    int color = 0;
    int placement = 0;
    int[] indexes;
    Image<Rgba32> image;
    public override void Render(float daltatime)
    {
        Engine.ClearBuffer();

        Engine.PrintImage(new GameInConsoleEngine.Engine.Point(0,1), indexes, image);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(0, 0), $"Daltatime = {daltatime}", 15);
        /*
        Engine.Fill(new Point(0, 0), new Point(placement * 3, placement), color);
        string figletFontPath = Resources.GetEntry("FFont").ResourcePath;
        FigletFont font = FigletFont.Load(figletFontPath);
        Engine.WriteFiglet(new Point(0, 0), "Hello world", font, 1);
        Engine.WriteText(new Point(50, 0), $"{(int)GetFramerate()}FPS", 7);
        Engine.WriteText(new Point(60, 0), $"color == {color}", 7);
        if (placement == 30)
        {
            color++;
            placement = 0;
        }
        placement++;
         */
        Engine.DisplayBuffer();
    }
    public override void Update(float daltatime)
    {
        if (indexes != null)
        {
            if (placement % 16 == 0)
            {
                for (int i = 0; i < indexes.Length; i++)
                {
                    indexes[i]++;
                }
                placement = 1;
            }
            else
            {
                placement++;
            }
        }
        else
        {
            string path = Resources.GetEntry("testImage").ResourcePath;
            image = ImageTools.GetImageBitmap(path);
            GameInConsoleEngine.Engine.Color[] colors = ImageTools.GetColors(image);
            indexes = Engine.GetColorIndex(colors);
        }
    }
}