using Game.Modules;
using UnityEngine;

namespace Game.UI
{
    public sealed class GameView : MonoBehaviour
    {
        private const string kSimulationCamera = "SimulationCamera";

        public Transform HudsLayer;
        public Transform WindowsLayer;
        public Camera Camera;
        
        public PortalModuleView PortalModuleView;

        private Camera _simulateCamera;

#if UNITY_EDITOR
        private void Awake()
        {
            _simulateCamera = GameObject.Find(kSimulationCamera).GetComponent<Camera>();
        }
#endif
    }
}