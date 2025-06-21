using GameInConsole.Engine.Tools;
using GameInConsoleEngine.Resource;
using SDL2;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace GameInConsoleEngine.Engine
{
    /// <summary>
    /// Abstract class to aid in Gamemaking.
    /// Implements an instance of the ConsoleEngine and has Looping methods.
    /// </summary>
    public abstract class ConsoleGame
    {
        /// <summary> Instance of a ConsoleEngine. </summary>
        public ConsoleEngine Engine { get; private set; }

        /// <summary> A counter representing the current unique frame we're at. </summary>
        public int FrameCounter { get; set; }

        /// <summary> A counter representing the total frames since launch</summary>
        public int FrameTotal { get; private set; }
        /// <summary> Factor for generating framerate-independent physics. time between last frame and current. </summary>
        public float DeltaTime { get; set; } = 0;

        /// <summary>The time the program started in DateTime, set after Create()</summary>
        public DateTime StartTime { get; private set; }

        /// <summary> The framerate the engine is trying to run at. </summary>
        public int TargetFramerate { get; set; }

        private bool Running { get; set; }

        private List<ThreadMono> threads = new List<ThreadMono>(16);

        private double[] framerateSamples;
        GameWindow window;

        public double upadteTime;
        public double upadteThreadTime;
        public double renderTime;
        bool doneRender;

        /// <summary> Initializes the ConsoleGame. Creates the instance of a ConsoleEngine and starts the game loop. </summary>
        /// <param name="width">Width of the window.</param>
        /// <param name="height">Height of the window.</param>
        /// <param name="fontW">Width of the font.</param>
        /// <param name="fontH">´Height of the font.</param>
        /// <param name="m">Framerate mode to run at.</param>
        /// <see cref="FramerateMode"/> <see cref="ConsoleEngine"/>
        public void Construct(int width, int height, int fontW, int fontH)
        {
            TargetFramerate = 30;
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(width, height),
                Title = "LearnOpenTK - Creating a Window",
                
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
                IsEventDriven = true,
            };
            GameWindowSettings gameWindowSettings = new GameWindowSettings()
            {
                UpdateFrequency = TargetFramerate
            };
            window = new GameWindow(gameWindowSettings, nativeWindowSettings);

            window.Load += StartEngine;
            window.UpdateFrame += EngineUpdate;
            window.Unload += EngineUnload;
            window.RenderFrame += EngineRenderFrame;
            window.FocusedChanged += EngineFocusChanged;

            threads = new List<ThreadMono>();

            int sampleCount = TargetFramerate;
            framerateSamples = new double[sampleCount];

            RunAwake();

            Engine = new ConsoleEngine(width, height, fontW, fontH, window);

            Running = true;

            window.Run();

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

        private void EngineFocusChanged(FocusedChangedEventArgs obj)
        {
            Engine.FocusChanged(obj.IsFocused);
            if (obj.IsFocused)
            {
                
            }
            else
            {

            }
        }

        private void EngineRenderFrame(FrameEventArgs obj)
        {
            if (!window.IsFocused)
            {
                return;
            }
            DateTime dateTimeNow = DateTime.Now;
            doneRender = false;
            Engine.ClearBuffer();
            double diff1 = (DateTime.Now - dateTimeNow).TotalMilliseconds;
            RunRender((float)obj.Time);
            double diff2 = (DateTime.Now - dateTimeNow).TotalMilliseconds;
            Engine.DisplayBuffer();
            double diff3 = (DateTime.Now - dateTimeNow).TotalMilliseconds;
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

        void EngineUpdate(FrameEventArgs args)
        {
            if (!window.IsFocused)
            {
                return;
            }
            DateTime lastTime = DateTime.UtcNow;
            window.UpdateFrequency = TargetFramerate;

            FrameCounter++;
            FrameCounter = FrameCounter % TargetFramerate;
            
            RunUpdate(args.Time);

            TimeSpan diff = DateTime.UtcNow - lastTime;
            DeltaTime = (float)(1 / (TargetFramerate * diff.TotalMilliseconds));

            framerateSamples[FrameCounter] = (double)diff.TotalMilliseconds;

            FrameTotal++;
            Console.WriteLine($"updateTime:{upadteTime:n2}, renderTime:{renderTime:n2}");
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
        async void RunUpdate(double deltaTime)
        {
            if (!window.IsFocused)
            {
                return;
            }
            DateTime startOfUpdating = DateTime.UtcNow;
            Input.UpdateAll(Engine);

            List<Task> updateTasks = threads.Select(thread => thread.runUpdate()).ToList();
            await Task.WhenAll(updateTasks);

            Update((float)deltaTime);
            upadteTime = (DateTime.UtcNow - startOfUpdating).TotalMilliseconds;
        }
        async void RunRender(float deltaTime)
        {
            if (!window.IsFocused)
            {
                return;
            }
            DateTime startOfUpdating = DateTime.Now;
            
            double diff1 = (DateTime.Now - startOfUpdating).TotalMilliseconds;
            List<Task> renderTasks = threads.Select(thread => thread.runRender()).ToList();
            await Task.WhenAll(renderTasks);
            double diff2 = (DateTime.Now - startOfUpdating).TotalMilliseconds;

            Render(deltaTime);
            double diff3 = (DateTime.Now - startOfUpdating).TotalMilliseconds;
            renderTime = (DateTime.Now  - startOfUpdating).TotalMilliseconds;
            doneRender = true;
        }
/*
        bool CheckExit()
        {
            if (reset || Engine.Exit || Running == false)
            {
                reset = false;
                Running = false;
                Engine.RunEvents = false;
                Resources.Clear();
                for (int i = 0; i < threads.Count; i++)
                {
                    threads[i].Stop();
                }
                threads = null;
                CleanUp();
                if (Engine.Exit)
                {
                    return false;
                }
                return true;
            }
            CheckForExit();
            return false;
        }
 */

        public void AddThreads(ThreadMono threadMono)
        {
            threads.Add(threadMono);
        }

        /// <summary> Gets the current framerate the application is running at. </summary>
        /// <returns> Application Framerate. </returns>
        public double GetFramerate()
        {
            return 1 / (framerateSamples.Sum() / TargetFramerate);
        }

        void CleanUp()
        {
            window.Close();
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