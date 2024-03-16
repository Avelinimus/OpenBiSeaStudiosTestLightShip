using DG.Tweening;
using Game.Core;
using Game.Core.Managers;
using Game.Managers;
using Game.States;
using Game.Utils;
using Injection;
using MVC;
using System;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public sealed class GameContext : IDisposable
    {
        public Context _context;

        public Context Context => _context;

        public void Initialize(Component[] components, params object[] objects)
        {
            _context = new Context();

            _context.Install(
                new Injector(_context),
                new ModuleManager(),
                new GameSceneManager(),
                new GameStateManager(),
                new ActionManager(),
                new PrefabManager(),
                new SpriteManager(),
                new HudManager(),
                new WindowManager(),
                new DataManager(),
                new SessionManager()
            );

            _context.Install(components);

            foreach (var obj in objects)
            {
                _context.Install(obj);
            }

            _context.ApplyInstall();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public void RunGame()
        {
            _context.Get<GameStateManager>().SwitchToState(typeof(GameInitializeState));
        }

        public void ReloadGame()
        {
            _context.Get<GameStateManager>().Dispose();
            _context.Dispose();
        }
    }

    public sealed class GameStartBehaviour : MonoBehaviour
    {
        public Action<bool> APPLICATION_PAUSED;
        public Action APPLICATION_QUITED;

        public GameContext GameContext;
        private Timer _timer;

        private void Start()
        {
            DOTween.SetTweensCapacity(500, 200);
            LoggerExtension.Init(LoggerDebugType.Unity);

            GameContext = new GameContext();

            _timer = new Timer();

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Application.runInBackground = false;

            GameContext.Initialize(GetComponents<Component>(), _timer);
            GameContext.RunGame();
        }

        public void Reload()
        {
            GameContext.ReloadGame();
            Start();
        }

        private void Update()
        {
            _timer.Update();
        }

        private void LateUpdate()
        {
            _timer.LateUpdate();
        }

        private void FixedUpdate()
        {
            _timer.FixedUpdate();
        }

        private void OnApplicationPause(bool isPaused)
        {
            Log.Info(Channel.Debug, "APPLICATION_PAUSED " + isPaused);
            APPLICATION_PAUSED.SafeInvoke(isPaused);
        }

        private void OnApplicationQuit()
        {
            GameContext.Dispose();
            APPLICATION_QUITED.SafeInvoke();
        }
    }
}