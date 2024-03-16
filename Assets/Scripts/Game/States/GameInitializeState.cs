using Game.Core.Managers;
using Injection;

using System.Collections.Generic;

namespace Game.States
{
    public sealed class GameInitializeState : GameLoadingState
    {
        [Inject] private GameStateManager _gameStateManager;
        [Inject] private Context _context;

        public override void Initialize()
        {
            InitializeManagers();
            _gameStateManager.SwitchToState<GamePlayState>();
        }

        public override void Dispose()
        {
        }

        private void InitializeManagers()
        {
            var allItems = new List<object>();
            _context.FillLinks(allItems);

            foreach (var item in allItems)
            {
                if (item is not Manager manager) continue;

                if (manager.IsInitialized)
                    continue;

                manager.IsInitialized = true;
                manager.Initialize();
            }
        }
    }
}