using Core;

using System;

namespace Game.UI
{
    public interface IBehaviourWithModel<T> where T : IObservable
    {
        T Model { set; }
    }

    public abstract class BehaviourWithModel<T> : BehaviourComponent, IBehaviourWithModel<T>, IObserver where T : Observable
    {
        [NonSerialized]
        private bool _released = false;
        private T _model;

        public T Model
        {
            protected get
            {
                return _model;
            }
            set
            {
                if (null != _model)
                {
                    _model.RemoveObserver(this);
                }

                OnApplyModel(value);

                _model = value;

                if (null != _model)
                {
                    _model.AddObserver(this);
                    OnModelChanged(_model);
                }
            }
        }

        protected BehaviourWithModel()
        {
        }

        protected abstract void OnModelChanged(T model);

        protected virtual void OnApplyModel(T model)
        {
        }

        #region Observer implementation
        public virtual void OnObjectChanged(Observable observable)
        {
            if (observable is T)
            {
                OnModelChanged((T)observable);
            }
            else
            {
                OnModelChanged(Model);
            }
        }
        #endregion

        protected sealed override void OnDestroy()
        {
            if (!_released)
            {
                OnReleaseResources();
                this.Model = null;
            }

            _released = true;
        }
    }
}
