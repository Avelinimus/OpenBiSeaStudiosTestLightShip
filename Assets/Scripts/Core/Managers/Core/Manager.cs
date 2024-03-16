using System;

namespace Game.Core.Managers
{
    public abstract class Manager : IDisposable
    {
        public bool IsInitialized
        {
            get;
            set;
        }

        protected Manager()
        {
            IsInitialized = false;
        }

        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        public abstract void Dispose();
    }
}
