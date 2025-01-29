﻿namespace GameInConsoleEngine.Engine
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
        private Thread gameThread;

        private List<ThreadMono> threads = new List<ThreadMono>();  

        private double[] framerateSamples;

        bool reset = false;

        /// <summary> Initializes the ConsoleGame. Creates the instance of a ConsoleEngine and starts the game loop. </summary>
        /// <param name="width">Width of the window.</param>
        /// <param name="height">Height of the window.</param>
        /// <param name="fontW">Width of the font.</param>
        /// <param name="fontH">´Height of the font.</param>
        /// <param name="m">Framerate mode to run at.</param>
        /// <see cref="FramerateMode"/> <see cref="ConsoleEngine"/>
        public void Construct(int width, int height, int fontW, int fontH)
        {
        _reset:

            TargetFramerate = 30;

            Engine = new ConsoleEngine(width, height, fontW, fontH);
            Start();
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Start();
            }
            StartTime = DateTime.Now;

            gameThread = new Thread(new ThreadStart(GameLoopLocked));

            Running = true;
            gameThread.Start();

            // gör special checks som ska gå utanför spelloopen
            // om spel-loopen hänger sig ska man fortfarande kunna avsluta
            while (Running)
            {
                if (reset)
                {
                    for (int i = 0; i < threads.Count; i++)
                    {
                        threads[i].Stop();
                    }
                    reset = false;
                    Running = false;
                    goto _reset;
                }
                CheckForExit();
            }
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Stop();
            }
        }

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
                Render(DeltaTime);

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

        /// <summary> Run once on Creating, import Resources here. </summary>
        public abstract void Start();
        /// <summary> Run every frame with rendering. Do math here. </summary>
        public abstract void Update(float daltatime);
        /// <summary> Run every frame after updating. Do drawing here. </summary>
        public abstract void Render(float daltatime);
    }
}