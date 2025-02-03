using GameInConsoleEngine.Resource;
using GameInConsoleEngine.Engine;
using GameInConsoleEngine.Engine.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Net.Http.Headers;
using GameInConsole.Game.Dialogsystem;

public class Program : ConsoleGame
{
    private static void Main(string[] args)
    {
        new Program().Construct(800, 800, 4, 8);
    }

    public override void Start()
    {
        EngineVariabels.Start();
        TargetFramerate = 15;

        Resources.Load("./Resources/Font.flf", "FFont");
        Resources.Load("./Resources/Image1.png", "testImage");
        Resources.Load("./Resources/ConsoleFont.png", "consoleFont");

        Engine.SetFont("./Resources/ConsoleFont.png", 16, 8, 16, 16);

        AddThreads(EngineVariabels.Player);

        for (int Y = 0; Y < Engine.WindowSize.Y / 16; Y++)
        {
            testStr.Add("".PadLeft(Engine.WindowSize.X, 'E'));
        }
    }
    int placement = 0;
    int[] indexes;
    Image<Rgba32> image;
    FigletFont font;
    List<string> testStr = new List<string>();
    public override void Render(float daltatime)
    {
DateTime dateTimeNow = DateTime.Now;
        Engine.ClearBuffer();

        for (int i = 0; i < testStr.Count; i++)
        {
            DateTime dateTimeLoop = DateTime.Now;
            GameInConsoleEngine.Engine.Point point = new GameInConsoleEngine.Engine.Point(0,i);
            Engine.WriteText(point, testStr[i], 15);
            double loopDiff = (DateTime.Now - dateTimeLoop).TotalMilliseconds;
            Console.WriteLine($"{i} @ {loopDiff:n2}");
        }
        
        /*
        DialogEntry entry = new DialogEntry(NPC.Player, "Embrace the journey, not just the destination. Every step, every challenge, shapes who you become. Keep moving forward with purpose and passion.");
        DialogSystem.DisplayDialog(entry, Engine);

        Engine.PrintImage(new GameInConsoleEngine.Engine.Point(0, 1), indexes, image);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(0, 0), $"Daltatime = {daltatime:n2}", 15);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(0, 2), $"FPS = {GetFramerate():n2}", 15);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(0, 10), $"UpdateTime = {upadteTime:n2}", 15, 1);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(0, 11), $"ThreadTime = {upadteThreadTime:n2}", 15);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(0, 12), $"renderTime = {renderTime:n2}", 15);
        
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(0, 14), "H", 15, 0);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(1, 14), "E", 15, 1);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(2, 14), "L", 15, 2);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(3, 14), "L", 15, 3);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(4, 14), "O", 15, 4);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(5, 14), " ", 15, 0);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(6, 14), "W", 15, 5);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(7, 14), "O", 15, 6);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(8, 14), "R", 15, 7);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(9, 14), "L", 15, 8);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(10, 14), "D", 15, 9);
        Engine.WriteText(new GameInConsoleEngine.Engine.Point(11, 14), "!", 15, 10);
        */

        Engine.DisplayBuffer();
        double diff = (DateTime.Now - dateTimeNow).TotalSeconds;
    }
    public override void Update(float daltatime)
    {
        if (font == null)
        {
            string figletFontPath = Resources.GetEntry("FFont").value;
            font = FigletFont.Load(figletFontPath);
        }

        if (indexes != null)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i]++;
            }
        }
        else
        {
            string path = Resources.GetEntry("testImage").value;
            image = ImageTools.GetImageBitmap(path);
            GameInConsoleEngine.Engine.Color[] colors = ImageTools.GetColors(image);
            indexes = Engine.GetColorIndex(colors);
        }
    }
}
