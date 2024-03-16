using Cysharp.Threading.Tasks;

using Game.Core.Managers;

using Injection;

using System;

using UnityEngine;

namespace Game.UI
{
    public abstract class BaseWindowMediator : IDisposable
    {
        public abstract string ViewType { get; }
        public abstract bool IsVisible { get; }
        public abstract int SiblingIndex { get; }
        public float BackgroundAlpha { get; set; }

        public abstract void Mediate(GameObject view, Transform parent);
        public abstract void Unmediate();

        public abstract void Show();
        public abstract void Hide();
        public abstract void HideEnd();

        public abstract UniTask CloseAnimation();
        public abstract UniTask OpenAnimation();

        public void Dispose()
        {
        }
    }

    public abstract class BaseWindowMediatorWithModel<T> : BaseWindowMediator where T : BaseWindowView
    {
        [Inject] protected PrefabManager _prefabManager;
        [Inject] protected WindowManager _windowManager;

        protected T View;
        public override string ViewType => typeof(T).Name;
        public override bool IsVisible => View != null && View.gameObject.activeSelf;
        public override int SiblingIndex => View == null ? 0 : View.transform.GetSiblingIndex();

        public override void Mediate(GameObject view, Transform parent)
        {
            view.transform.SetParent(parent);
            View = view.GetComponent<T>();
            View.transform.SetAsLastSibling();
            View.gameObject.SetActive(true);
            View.CLOSE_WINDOW += OnCloseClicked;
        }

        public override void Unmediate()
        {
            if (null == View)
                return;

            View.transform.localScale = Vector3.one;
            _prefabManager.Remove(View.gameObject);
            View.CLOSE_WINDOW -= OnCloseClicked;
            View.gameObject.SetActive(false);
            View = default;
        }

        public override async UniTask OpenAnimation()
        {
            await View.OpenAnimation();
        }

        public override async UniTask CloseAnimation()
        {
            await View.CloseAnimation();
        }

        protected virtual void OnCloseClicked()
        {
            _windowManager?.Close(this);
        }
    }
}