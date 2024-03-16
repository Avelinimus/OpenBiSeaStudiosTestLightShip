using System;

namespace Game.Core
{
    public abstract class State : IDisposable
    {
        public virtual void Initialize(params object[] args) {}
        public abstract void Initialize();
        public abstract void Dispose();
    }
}