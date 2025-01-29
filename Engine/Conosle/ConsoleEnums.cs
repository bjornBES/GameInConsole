namespace GameInConsoleEngine.Engine
{
    /// <summary> Enum for basic Unicodes. </summary>
    public enum ConsoleCharacter
    {
        Null = 0x0000,

        Full = 0x2588,
        Dark = 0x2593,
        Medium = 0x2592,
        Light = 0x2591,

        // box drawing syboler
        // ┌───────┐
        // │       │
        // │       │
        // └───────┘
        BoxDrawingL_H = 0x2500,
        BoxDrawingL_V = 0x2502,
        BoxDrawingL_DR = 0x250C,
        BoxDrawingL_DL = 0x2510,
        BoxDrawingL_UL = 0x2518,
        BoxDrawingL_UR = 0x2514,
    }

    /// <summary> Enum for Different Gameloop modes. </summary>
    public enum FramerateMode
    {
        /// <summary>Run at max speed, but no higher than TargetFramerate.</summary>
        MaxFps,
        /// <summary>Run at max speed.</summary>
        Unlimited
    }
}
