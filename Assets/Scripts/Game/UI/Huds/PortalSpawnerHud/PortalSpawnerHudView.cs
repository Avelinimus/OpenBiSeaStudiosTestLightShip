using Game.UI;
using Game.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Huds
{
    public class PortalSpawnerHudView : BaseHud
    {
        public event Action SPAWN_PORTAL;

        [SerializeField] private Button _spawnPortalBtn;

        protected override void OnEnable()
        {
            _spawnPortalBtn.onClick.AddListener(OnSpawnBtn);
        }

        protected override void OnDisable()
        {
            _spawnPortalBtn.onClick.RemoveListener(OnSpawnBtn);
        }

        private void OnSpawnBtn()
        {
            SPAWN_PORTAL.SafeInvoke();
        }
    }
}