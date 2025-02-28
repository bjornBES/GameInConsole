

using GameInConsoleEngine.Engine;

namespace GameInConsole.Engine.Tools
{
    public static class Input
    {
        static ConsoleEngine Engine;
        public static void UpdateAll(ConsoleEngine engine)
        {
            Engine = engine;
        }
    }
}