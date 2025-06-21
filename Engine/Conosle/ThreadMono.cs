using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInConsoleEngine.Engine
{
    public abstract class ThreadMono
    {
        public ConsoleEngine Engine;
        public DateTime startUpdateTime;
        public DateTime startReaderTime;
        public double UpdateTime;
        public double ReaderTime;
        public abstract void Awake();
        public abstract void Start();
        public abstract void Stop();
        public abstract void Update();
        public abstract void Render();
        internal async void runAwake()
        {
            await Task.Run(() =>
            {
                Awake();
            });
        }
        internal async void runStart()
        {
            await Task.Run(() =>
            {
                Start();
            });
        }
        internal async Task runUpdate()
        {
            startUpdateTime = DateTime.Now;
            await Task.Run(() =>
            {
                Update();
                UpdateTime = (DateTime.UtcNow - startUpdateTime).TotalMilliseconds;
            });
        }
        internal async Task runRender()
        {
            startReaderTime = DateTime.Now;
            await Task.Run(() =>
            {
                Render();
                ReaderTime = (DateTime.UtcNow - startReaderTime).TotalMilliseconds;
            });
        }
    }
}
