using Game.Core.Managers;

using UnityEngine;

namespace Game.UI.Hud
{
    public abstract class Mediator
    {
        public abstract string ViewType { get; }

        public abstract void Mediate(GameObject prefab, Transform parent, PrefabManager prefabManager);
        public abstract void Unmediate();

        public abstract void InternalShow();
        public abstract void InternalHide();
    }

    public abstract class Mediator<T> : Mediator where T : IHud
    {
        private bool _isShowed;
        protected T _view;
        public override string ViewType => typeof(T).Name;
        protected T View => _view;

        private PrefabManager _prefabManager;
        private GameObject _prefab;

        public sealed override void Mediate(GameObject prefab, Transform parent, PrefabManager prefabManager)
        {
            _prefabManager = prefabManager;
            _prefab = prefab;
            prefab.transform.SetParent(parent);
            var view = prefab.GetComponent<T>();
            _view = view;
            _isShowed = false;
        }

        public sealed override void Unmediate()
        {
            if (_isShowed)
            {
                Hide();
            }

            _prefabManager.Remove(_prefab);
            _view = default(T);
        }

        public sealed override void InternalShow()
        {
            _view.IsActive = true;
            Show();
        }

        public sealed override void InternalHide()
        {
            _view.IsActive = false;
            Hide();
        }

        protected abstract void Show();
        protected abstract void Hide();
    }
}