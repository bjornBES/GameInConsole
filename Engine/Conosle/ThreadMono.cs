using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInConsoleEngine.Engine
{
    public abstract class ThreadMono
    {
        public abstract void Start();
        public abstract void Stop();
        public abstract void Update(float deltaTime);
    }
}
