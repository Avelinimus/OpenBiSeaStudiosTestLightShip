using Cysharp.Threading.Tasks;
using Game.UI;
using Injection;
using System;
using System.Collections.Generic;

namespace Game.Core.Managers
{
    public enum WindowType
    {
        None = 0,
    }

    public sealed class WindowManager : Manager, IDisposable
    {
        private const string kPathWindows = "Windows/{0}/{1}";
        private const string kPathBackground = "Windows/WindowBackground";
        private const string kNameFileWindowView = "WindowView";
        private const float kBackgroundAlpha = .7f;

        [Inject] private PrefabManager _prefabManager;
        [Inject] private GameView _gameView;
        [Inject] private Injector _injector;

        private readonly Dictionary<WindowType, BaseWindowMediator> _windowsMap;
        private readonly List<BaseWindowMediator> _queue;
        private WindowBackgroundView _windowBackgroundView;

        public int CountOpened => _queue.Count;

        public WindowManager()
        {
            _queue = new List<BaseWindowMediator>();

            _windowsMap = new Dictionary<WindowType, BaseWindowMediator>
            {
            };
        }

        public override void Initialize()
        {
            foreach (var item in _windowsMap)
            {
                _injector.Inject(item.Value);
            }
        }

        public override void Dispose()
        {
            if (_windowBackgroundView != null && _windowBackgroundView.gameObject != null)
                _prefabManager.RemoveForever(_windowBackgroundView.gameObject);
        }

        public async UniTask PreloadAll()
        {
            var gameObjectAwait = await _prefabManager.GetAsync(kPathBackground, _gameView.WindowsLayer);
            _windowBackgroundView = gameObjectAwait.GetComponent<WindowBackgroundView>();

            foreach (var window in _windowsMap)
            {
                var mediator = window.Value;
                var fileName = mediator.ViewType.Replace(kNameFileWindowView, string.Empty);

                gameObjectAwait = await _prefabManager.GetAsync(string.Format(kPathWindows, fileName, fileName),
                    _gameView.WindowsLayer);

                mediator.Mediate(gameObjectAwait, _gameView.WindowsLayer);
                mediator.Unmediate();
            }

            SetBackground(false);
        }

        public async UniTask Open(WindowType windowType, float backgroundAlpha = kBackgroundAlpha)
        {
            var mediator = _windowsMap[windowType];

            if (mediator.IsVisible) return;

            var fileName = mediator.ViewType.Replace(kNameFileWindowView, string.Empty);

            var gameObjectAwait = await _prefabManager.GetAsync(string.Format(kPathWindows, fileName, fileName),
                _gameView.WindowsLayer);

            mediator.Mediate(gameObjectAwait, _gameView.WindowsLayer);
            mediator.Show();
            mediator.BackgroundAlpha = backgroundAlpha;
            _queue.Add(mediator);
            SetBackground(true, mediator.SiblingIndex - 1, backgroundAlpha);
            await mediator.OpenAnimation();
        }

        public async UniTask Close(WindowType windowType)
        {
            var mediator = _windowsMap[windowType];

            if (!mediator.IsVisible) return;

            _queue.Remove(mediator);
            mediator.Hide();
            await mediator.CloseAnimation();
            mediator.HideEnd();
            mediator.Unmediate();
            var lastMediator = _queue.Count <= 0 ? null : _queue[^1];
            var siblingIndex = _queue.Count <= 0 ? 0 : _queue[^1].SiblingIndex - 1;
            SetBackground(CountOpened > 0, siblingIndex, lastMediator?.BackgroundAlpha ?? kBackgroundAlpha);
        }

        public async UniTask Close(BaseWindowMediator mediator)
        {
            if (!mediator.IsVisible) return;

            _queue.Remove(mediator);
            mediator.Hide();
            await mediator.CloseAnimation();
            mediator.HideEnd();
            mediator.Unmediate();
            var siblingIndex = _queue.Count <= 0 ? 0 : _queue[^1].SiblingIndex - 1;
            SetBackground(CountOpened > 0, siblingIndex);
        }

        public void CloseAll()
        {
            foreach (var window in _windowsMap)
            {
                var mediator = window.Value;
                mediator.Unmediate();
            }

            SetBackground(false);
        }

        private void SetBackground(bool isActive, int index = 0, float backgroundAlpha = kBackgroundAlpha)
        {
            _windowBackgroundView.gameObject.SetActive(isActive);
            _windowBackgroundView.transform.SetSiblingIndex(index <= 0 ? 0 : index);
            _windowBackgroundView.BackgroundAlpha = backgroundAlpha;
        }
    }
}