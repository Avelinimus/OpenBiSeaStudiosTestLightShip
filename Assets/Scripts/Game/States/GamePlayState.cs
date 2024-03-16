using Game.Core.Managers;
using Game.Huds;
using Game.Modules;
using Game.UI;
using Game.UI.Managers;
using Injection;

namespace Game.States
{
    public sealed class GamePlayState : GameLoadingState
    {
        [Inject] private Injector _injector;
        [Inject] private Context _context;
        [Inject] private HudManager _hudManager;
        [Inject] private GameView _gameView;
        [Inject] private ModuleManager _moduleManager;

        private GameManager _gameManager;

        public override  void Initialize()
        {
            _gameManager = new GameManager();
            _gameManager.Initialize();
            _context.Install(_gameManager);
            _injector.Inject(_gameManager);
            _hudManager.ShowAdditional<PortalSpawnerHudMediator>();
            AddModules();
        }

        public override void Dispose()
        {
            RemoveAllModules();
            _hudManager.HideAdditional<PortalSpawnerHudMediator>();
            _context.Uninstall(_gameManager);
        }

        private void AddModules()
        {
            _moduleManager.AddModule<PortalModule, PortalModuleView>(this, _gameView.PortalModuleView);
        }

        private void RemoveAllModules()
        {
            _moduleManager.DisposeModules(this);
        }
    }
}