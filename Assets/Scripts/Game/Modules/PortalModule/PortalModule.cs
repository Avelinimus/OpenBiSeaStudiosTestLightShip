using Game.Core;
using Game.UI;
using Game.UI.Managers;
using Injection;
using UnityEngine;

namespace Game.Modules
{
    public sealed class PortalModule : Module<PortalModuleView>
    {
        private const float kRadius = 1.5f;
        [Inject] private GameManager _gameManager;
        [Inject] private GameView _gameView;

        private PortalView _portalView;

        public PortalModule(PortalModuleView view) : base(view)
        {
        }

        public override void Initialize()
        {
            _gameManager.SPAWN_PORTAL += OnSpawnPortal;
        }

        public override void Dispose()
        {
            _gameManager.SPAWN_PORTAL -= OnSpawnPortal;
            _portalView = null;
        }

        private void OnSpawnPortal()
        {
            if (_portalView != null)
            {
                View.Factory.Release(_portalView);
                _portalView = null;
            }

            _portalView = View.Factory.Get<PortalView>();
            _portalView.Initialize(_gameView.Camera);
            var player = _gameView.Camera.transform;
            var offset = player.forward * kRadius;
            var resultPosition = player.transform.position + offset;

            _portalView.transform.position
                = new Vector3(resultPosition.x, player.transform.position.y - .2f, resultPosition.z);
            _portalView.transform.LookAt(player.position);

            _portalView.transform.eulerAngles = new Vector3(0, _portalView.transform.eulerAngles.y,
                _portalView.transform.eulerAngles.z);
        }
    }
}