using GameInConsole.Engine.Tools;
using GameInConsoleEngine.Resource;
using SDL2;
using SixLabors.ImageSharp.Formats.Tiff.Compression;

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
        IntPtr window;
        IntPtr renderer;
        bool reset = false;

        public double upadteTime;
        public double upadteThreadTime;
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

#if DEBUG
            SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
#endif

            TargetFramerate = 30;

        _reset:
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            window = SDL.SDL_CreateWindow("Window Here :)", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            Engine = new ConsoleEngine(width, height, fontW, fontH, window, renderer);

            threads = new List<ThreadMono>();
            Start();
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Engine = Engine;
                threads[i].Start();
            }
            StartTime = DateTime.Now;

            Running = true;

            // gör special checks som ska gå utanför spelloopen
            // om spel-loopen hänger sig ska man fortfarande kunna avsluta
            {
                int sampleCount = TargetFramerate;
                framerateSamples = new double[sampleCount];
                DateTime lastTime;
                float uncorrectedSleepDuration = 1000 / TargetFramerate;

                while (Running)
                {
                    lastTime = DateTime.UtcNow;

                    FrameCounter++;
                    FrameCounter = FrameCounter % sampleCount;

                    RunUpdate();
                    // Thread updateThread = new Thread(new ThreadStart(RunUpdate));
                    // updateThread.Start();

                    Engine.ClearBuffer();
                    Thread threadThread = new Thread(new ThreadStart(RunThreads));
                    threadThread.Start();

                    Thread renderThread = new Thread(new ThreadStart(RunRender));
                    renderThread.Start();
                    while (threadThread.ThreadState == ThreadState.Running || renderThread.ThreadState == ThreadState.Running)
                    {
                        
                    }
                    Engine.DisplayBuffer();




                    if (CheckExit())
                    {
                        goto _reset;
                    }

                    TimeSpan diff = DateTime.UtcNow - lastTime;
                    DeltaTime = (float)(1 / (TargetFramerate * diff.TotalSeconds));

                    float computingDuration = (float)diff.TotalMilliseconds;
                    int sleepDuration = (int)(uncorrectedSleepDuration - computingDuration);
                    if (sleepDuration > 0)
                    {
                        // programmet ligger före maxFps, sänker det
                        Thread.Sleep(sleepDuration);
                    }

                    //increases total frames
                    FrameTotal++;

                    // beräknar framerate

                    framerateSamples[FrameCounter] = diff.TotalSeconds;
                }
            }
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

        void RunUpdate()
        {
            Input.UpdateAll(Engine);
            DateTime startOfUpdating = DateTime.UtcNow;
            Update(DeltaTime);
            upadteTime = (DateTime.UtcNow - startOfUpdating).TotalMilliseconds;
        }
        void RunThreads()
        {
            DateTime startOfUpdating = DateTime.UtcNow;
            for (int i = 0; i < threads.Count; i++)
            {
                Thread thread = new Thread(new ThreadStart(threads[i].UpdateAll));
                thread.Start();
            }
            upadteThreadTime = (DateTime.UtcNow - startOfUpdating).TotalMilliseconds;
        }
        void RunRender()
        {
            DateTime startOfUpdating = DateTime.UtcNow;
            Render(DeltaTime);
            renderTime = (DateTime.UtcNow - startOfUpdating).TotalMilliseconds;
        }

        /*
        private void GameLoopLocked()
        {
            int sampleCount = TargetFramerate;
            framerateSamples = new double[sampleCount];

            DateTime lastTime;
            float uncorrectedSleepDuration = 1000 / TargetFramerate;
            while (Running)
            {
                lastTime = DateTime.UtcNow;

                FrameCounter++;
                FrameCounter = FrameCounter % sampleCount;

                // kör main programmet

                // RenderUpdate();

                // Thread updateThread = new Thread(new ThreadStart(Update));
                // Thread renderThread = new Thread(new ThreadStart(Render));
                // updateThread.Start();
                // renderThread.Start();

                Update(DeltaTime);
                for (int i = 0; i < threads.Count; i++)
                {
                    threads[i].Update(DeltaTime);
                }
                SDL.SDL_RenderClear(renderer);
                Render(DeltaTime);
                SDL.SDL_RenderPresent(renderer);
                Engine.getEvents();

                // while (updateThread.ThreadState == ThreadState.Running || renderThread.ThreadState == ThreadState.Running)
                // {
                // 
                // }

                TimeSpan diff = DateTime.UtcNow - lastTime;
                DeltaTime = (float)(1 / (TargetFramerate * diff.TotalSeconds));

                float computingDuration = (float)diff.TotalMilliseconds;
                int sleepDuration = (int)(uncorrectedSleepDuration - computingDuration);
                if (sleepDuration > 0)
                {
                    // programmet ligger före maxFps, sänker det
                    Thread.Sleep(sleepDuration);
                }

                //increases total frames
                FrameTotal++;

                // beräknar framerate

                framerateSamples[FrameCounter] = diff.TotalSeconds;
            }
        }
        */

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

        public void AddThreads(ThreadMono threadMono)
        {
            threads.Add(threadMono);
        }

        public void Reset()
        {
            reset = true;
        }

        /// <summary> Gets the current framerate the application is running at. </summary>
        /// <returns> Application Framerate. </returns>
        public double GetFramerate()
        {
            return 1 / (framerateSamples.Sum() / TargetFramerate);
        }

        private void CheckForExit()
        {
            if (Engine.GetKeyDown(ConsoleKey.Delete))
            {
                Running = false;
            }
        }

        void CleanUp()
        {
            SDL.SDL_DestroyRenderer(Engine.renderer);
            SDL.SDL_DestroyWindow(Engine.window);
            window = IntPtr.Zero;
            renderer = IntPtr.Zero;
            SDL.SDL_Quit();

        }

        /// <summary> Run once on Creating, import Resources here. </summary>
        public abstract void Start();
        /// <summary> Run every frame with rendering. Do math here. </summary>
        public abstract void Update(float daltatime);
        /// <summary> Run every frame after updating. Do drawing here. </summary>
        public abstract void Render(float daltatime);
    }
}