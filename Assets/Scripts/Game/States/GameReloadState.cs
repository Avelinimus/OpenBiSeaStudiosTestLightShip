using Game.UI;
using Injection;
using System.Collections;

namespace Game.States
{
    public sealed class GameReloadState : GameState
    {
        [Inject] private GameStartBehaviour _gameStartBehaviour;
        [Inject] private GameView _gameView;

        public override void Initialize()
        {
            _gameView.StopAllCoroutines();
            _gameView.StartCoroutine(Reload());
        }

        public override void Dispose()
        {
            _gameView.StopAllCoroutines();
        }

        private IEnumerator Reload()
        {
            _gameStartBehaviour.Reload();
            yield break;
        }
    }
}