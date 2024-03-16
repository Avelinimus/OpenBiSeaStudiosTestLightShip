using Game.Core.Managers;
using Game.Utils;
using System;

namespace Game.UI.Managers
{
    public sealed class GameManager : Manager
    {
        public event Action SPAWN_PORTAL;

        public override void Dispose()
        {
            
        }

        public void FireSpawnPortal()
        {
            SPAWN_PORTAL.SafeInvoke();
        }
    }
}