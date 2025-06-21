using GameInConsole.Engine.Tools;
using GameInConsoleEngine.Resource;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using System.Threading.Tasks;
using Raylib_cs;
using System.Diagnostics;
using GameInConsole.Engine;

namespace GameInConsoleEngine.Engine
{
    /// <summary>
    /// Abstract class to aid in Gamemaking.
    /// Implements an instance of the ConsoleEngine and has Looping methods.
    /// </summary>
    public abstract class ConsoleGame
    {
        public ConsoleEngine Engine { get; private set; }

        public int FrameCounter { get; set; }
        public int FrameTotal { get; private set; }
        public float DeltaTime { get; set; } = 0;
        public DateTime StartTime { get; private set; }
        private int _TargetFramerate { get; set; }
        public int TargetFramerate
        {
            get
            {
                return _TargetFramerate;
            }
            set
            {
                Raylib.SetTargetFPS(value);
                _TargetFramerate = value;
            }
        }

        private bool Running { get; set; }
        private List<ThreadMono> threads = new List<ThreadMono>(16);
        private double[] framerateSamples;

        public double updateTime;
        public double updateThreadTime;
        public double renderTime;

        /// <summary> Initializes the ConsoleGame. Creates the instance of a ConsoleEngine and starts the game loop. </summary>
        /// <param name="width">Width of the window.</param>
        /// <param name="height">Height of the window.</param>
        /// <param name="fontW">Width of the font.</param>
        /// <param name="fontH">´Height of the font.</param>
        /// <param name="m">Framerate mode to run at.</param>
        /// <see cref="FramerateMode"/> <see cref="ConsoleEngine"/>
        public void Construct(int width, int height, int fontW, int fontH)
        {
            Raylib.InitWindow(width * fontW, height * fontH, "Untitled Game");

            Raylib.SetTargetFPS(30);

            _TargetFramerate = 30;
            threads = new List<ThreadMono>();

            int sampleCount = _TargetFramerate;
            framerateSamples = new double[sampleCount];

            RunAwake();
            Engine = new ConsoleEngine(width, height, fontW, fontH);
            Running = true;

            StartEngine();
            RunMainLoop();

            while (Running)
            {

            }
        }

        void RunMainLoop()
        {
            while (Running && !Raylib.WindowShouldClose())
            {
                _ = EngineUpdate();
                _ = EngineRenderFrame();
                // await Task.Delay(1000 / _TargetFramerate);
            }
            Running = false;
            EngineUnload();
            Raylib.CloseWindow();
        }

        private async Task EngineRenderFrame()
        {
            Stopwatch sw = Stopwatch.StartNew();


            Engine.ClearBuffer();
            await RunRender(DeltaTime);
            Engine.DisplayBuffer(Running);

            sw.Stop();
            renderTime = sw.Elapsed.TotalMilliseconds;
        }

        private void EngineUnload()
        {
            Engine.RunEvents = false;
            if (threads != null)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    threads[i].Stop();
                }
                threads = null;
            }
            Resources.Clear();
            CleanUp();
        }

        async Task EngineUpdate()
        {
            Stopwatch sw = Stopwatch.StartNew();

            FrameCounter = (FrameCounter + 1) % _TargetFramerate;
            await RunUpdate(DeltaTime);

            sw.Stop();
            updateTime = sw.Elapsed.TotalMilliseconds;
            DeltaTime = (float)sw.Elapsed.TotalSeconds;
            
            framerateSamples[FrameCounter] = updateTime;

            FrameTotal++;
            Console.WriteLine($"updateTime:{updateTime:n2}, renderTime:{renderTime:n2}");
        }

        void StartEngine()
        {
            Start();
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Engine = Engine;
                threads[i].runStart();
            }
            StartTime = DateTime.Now;
        }

        void RunAwake()
        {
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].runAwake();
            }
            Awake();
        }
        private async Task RunUpdate(float deltaTime)
        {
            Stopwatch sw = Stopwatch.StartNew();

            Input.UpdateAll(Engine);
            var updateTasks = threads.Select(t => t.runUpdate()).ToList();
            await Task.WhenAll(updateTasks);

            Update(deltaTime);

            sw.Stop();
            updateThreadTime = sw.Elapsed.TotalMilliseconds;
        }

        private async Task RunRender(float deltaTime)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var renderTasks = threads.Select(t => t.runRender()).ToList();
            await Task.WhenAll(renderTasks);

            Render(deltaTime);

            sw.Stop();
            renderTime = sw.Elapsed.TotalMilliseconds;
        }

        public void AddThreads(ThreadMono threadMono)
        {
            threads.Add(threadMono);
        }

        /// <summary> Gets the current framerate the application is running at. </summary>
        /// <returns> Application Framerate. </returns>
        public double GetFramerate()
        {
            return 1000.0 / (framerateSamples.Sum() / _TargetFramerate);
        }

        public void SetEngineToText()
        {
            EngineConfig.UseTextBasedGraphics = true;
        }
        public void SetEngineToGraphics()
        {
            EngineConfig.UseTextBasedGraphics = false;
        }

        void CleanUp()
        {
        }

        /// <summary> Run once before the first fream, import Resources here. </summary>
        public abstract void Awake();
        /// <summary> Run once on Creating, import Resources here. </summary>
        public abstract void Start();
        /// <summary> Run every frame with rendering. Do math here. </summary>
        public abstract void Update(float daltatime);
        /// <summary> Run every frame after updating. Do drawing here. </summary>
        public abstract void Render(float daltatime);
    }

    
}
