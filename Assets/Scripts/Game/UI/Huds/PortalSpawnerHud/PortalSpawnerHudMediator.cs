using Game.UI.Hud;
using Game.UI.Managers;
using Injection;

namespace Game.Huds
{
    public sealed class PortalSpawnerHudMediator : Mediator<PortalSpawnerHudView>
    {
        [Inject] private GameManager _gameManager;
        protected override void Show()
        {
            View.SPAWN_PORTAL += OnSpawnPortal;
        }

        protected override void Hide()
        {
            View.SPAWN_PORTAL -= OnSpawnPortal;
        }

        private void OnSpawnPortal()
        {
            _gameManager.FireSpawnPortal();
        }
    }
}