using System;

namespace Game.Core
{
    public abstract class Module : IDisposable
    {
        public abstract void Initialize();

        public abstract void Dispose();

        public virtual void OnUnPaused()
        {

        }

        public virtual void OnPaused()
        {

        }

        internal void SetPause(bool isActive)
        {
            if (isActive)
            {
                OnUnPaused();
            }
            else
            {
                OnPaused();
            }
        }
    }

    public abstract class Module<T> : Module
    {
        protected readonly T View;

        protected Module(T view)
        {
            View = view;
        }
    }
}