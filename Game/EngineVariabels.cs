using GameInConsole.Game.PlayerSystem;
using GameInConsole.Game.Dialogsystem;
using System.Net.Http.Headers;

public class EngineVariabels
{
    public static string PlayerName = "";
    public static Player Player = new Player();


    public static Dictionary<NPC, NPCEntry> NPCs = new Dictionary<NPC,NPCEntry>();

    public static void Start()
    {
        NPCs.Add(NPC.Player, new NPCEntry(PlayerName, ""));
    }
}