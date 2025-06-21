using GameInConsoleEngine.Engine;
using GameInConsoleEngine.Engine.Tools;
using GameInConsoleEngine.Resource;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameInConsole.Game.Dialogsystem
{
    public static class DialogSystem
    {
        static readonly GameInConsoleEngine.Engine.Point DialogTextPoint = new GameInConsoleEngine.Engine.Point(0, 40);
        static readonly GameInConsoleEngine.Engine.Point DialogNamePoint = new GameInConsoleEngine.Engine.Point(0, 41);
        public static void DisplayDialog(DialogEntry entry, ConsoleEngine engine)
        {
            //GetImage();
            // Image<Rgba32> image = GetImage(entry);
            string dialogString = entry.dialog;
            string NPCname = EngineVariabels.NPCs[entry.NPC].Name;

            string[] strings = formatString(dialogString, engine.WindowSize.X);

            for (int i = 0; i < strings.Length; i++)
            {
                GameInConsoleEngine.Engine.Point point = DialogTextPoint;
                point.X = (engine.WindowSize.X / 8 / 2) - (strings[i].Length / 2);

                point.Y += i;
                engine.WriteText(point, strings[i], 15);
            }
            engine.WriteText(DialogNamePoint, NPCname, 15);


            if (entry.NextEntry != null)
            {
                DisplayDialog(entry.NextEntry, engine);
            }
        }

        static string[] formatString(string dialogString, int widthInPixels)
        {
            int textWidth = widthInPixels / 16;
            if (dialogString.Length <= textWidth)
            {
                return [dialogString];
            }

            List<string> result = new List<string>();

            int index = 0;
            int count = 50;
            while (true)
            {
                if (index >= dialogString.Length)
                {
                    break;
                }

                if (index + count > dialogString.Length)
                {
                    count = dialogString.Length - index;
                }

                string str = dialogString.Substring(index, count);
                if (str.Length == textWidth)
                {
                    for (int i = str.Length - 1; i >= 0; i--)
                    {
                        if (str[i] == ' ')
                        {
                            result.Add(str[0..i]);
                            index += i + 1;
                            break;
                        }
                    }
                }
                else 
                {
                    index += str.Length;
                    result.Add(str);
                }
            }
            return result.ToArray();
        }
        /*
        static Image<Rgba32> GetImage(DialogEntry entry)
        {
            string key = EngineVariabels.NPCs[entry.NPC].ResourceKey;
            string NPCimgName = key + "_img";
            if (Resources.GetEntry(NPCimgName, out ResourceEntry resourceEntry))
            {
            }
            else
            {
                return ImageTools.GetImageBitmap(resourceEntry.value);
            }

        }
         */
    }

    public class DialogEntry
    {
        public NPC NPC;
        public string dialog;
        public DialogEntry NextEntry;
        public DialogEntry(NPC npc, string _dialog)
        {
            NPC = npc;
            dialog = _dialog;
            NextEntry = null;
        }
        public DialogEntry(NPC npc, string _dialog, DialogEntry nextEntry)
        {
            NPC = npc;
            dialog = _dialog;
            NextEntry = nextEntry;
        }
        public override string ToString()
        {
            return $"{EngineVariabels.NPCs[NPC].Name}: {dialog}";
        }
    }
    public enum NPC
    {
        Player,
    }
}
