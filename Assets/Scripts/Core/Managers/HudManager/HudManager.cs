using Game.UI;
using Game.UI.Hud;
using Game.Utils;
using Injection;
using MVC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Managers
{
    public sealed class HudManager
    {
        private const string kHudsPath = "Huds/{0}";

        private Action<bool> SINGLE_HUD_OPENED;
        public Action<Type> HUD_OPENED;
        public Action<Type> HUD_CLOSED;

        [Inject] private GameView _gameView;
        [Inject] private Injector _injector;
        [Inject] private PrefabManager _prefabManager;

        private Mediator _openedHud;
        private readonly List<Mediator> _additionalHuds;

        public bool IsShowed<T>()
        {
            return _openedHud is T || _additionalHuds.Exists(temp => temp is T);
        }

        public bool IsOpened()
        {
            return null != _openedHud;
        }

        public HudManager()
        {
            _additionalHuds = new List<Mediator>();
        }

        public T ShowSingle<T>(params object[] args) where T : Mediator
        {
            if (null != _openedHud)
            {
                HideSingle();
            }

            _openedHud = (Mediator) Activator.CreateInstance(typeof(T), args);
            _injector.Inject(_openedHud);
            var fileName = typeof(T).Name.Replace("HudMediator", string.Empty);

            var prefab = _prefabManager.Get(string.Format(kHudsPath, fileName));
            _openedHud.Mediate(prefab, _gameView.HudsLayer, _prefabManager);
            _openedHud.InternalShow();

            SINGLE_HUD_OPENED.SafeInvoke(true);

            return (T) _openedHud;
        }

        public void HideSingle()
        {
            if (null == _openedHud)
                return;

            _openedHud.InternalHide();
            _openedHud.Unmediate();
            _openedHud = null;

            SINGLE_HUD_OPENED.SafeInvoke(false);
        }

        public T ShowAdditional<T>(params object[] args) where T : Mediator
        {
            var opened = _additionalHuds.FirstOrDefault(temp => temp is T);

            if (null != opened)
            {
                Log.Warning(Channel.Hud, typeof(T) + " is already opened");

                return opened as T;
            }

            var hud = (Mediator) Activator.CreateInstance(typeof(T), args);
            _additionalHuds.Add(hud);

            _injector.Inject(hud);
            var fileName = typeof(T).Name.Replace("HudMediator", "");

            var prefabResult = _prefabManager.Get(string.Format(kHudsPath, fileName));

            hud.Mediate(prefabResult, _gameView.HudsLayer, _prefabManager);
            hud.InternalShow();

            HUD_OPENED.SafeInvoke(hud.GetType());

            return (T) hud;
        }

        public void HideAdditional<T>()
        {
            for (int i = _additionalHuds.Count - 1; i >= 0; i--)
            {
                var hud = _additionalHuds[i];

                if (!(hud is T))
                    continue;

                hud.InternalHide();
                hud.Unmediate();
                _additionalHuds.RemoveAt(i);

                HUD_CLOSED.SafeInvoke(hud.GetType());
            }
        }

        public void HideAdditional(object target)
        {
            for (int i = _additionalHuds.Count - 1; i >= 0; i--)
            {
                var hud = _additionalHuds[i];

                if (!(hud.GetType() == target.GetType()))
                    continue;

                hud.InternalHide();
                hud.Unmediate();
                _additionalHuds.RemoveAt(i);

                HUD_CLOSED.SafeInvoke(hud.GetType());
            }
        }

        public void HideAllAdditionals()
        {
            for (int i = _additionalHuds.Count - 1; i >= 0; i--)
            {
                var hud = _additionalHuds[i];

                hud.InternalHide();
                hud.Unmediate();

                _additionalHuds.RemoveAt(i);

                HUD_CLOSED.SafeInvoke(hud.GetType());
            }
        }
    }
}