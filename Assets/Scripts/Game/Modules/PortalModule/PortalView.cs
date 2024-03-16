using Game.UI.Modules.Camera;
using UnityEngine;

namespace Game.Modules
{
    public sealed class PortalView : MonoBehaviour
    {
        [SerializeField] private GameObject _ground;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _arWorld;
        [SerializeField] private GameObject _realWorld;

        private Camera _mainCamera;

        public void Initialize(Camera camera)
        {
            _mainCamera = camera;
        }

        private void Update()
        {
            Vector3 cameraOffset = _mainCamera.transform.position - transform.position;
            _camera.transform.position = transform.position + cameraOffset;
            _camera.transform.rotation = Quaternion.LookRotation(_mainCamera.transform.forward, Vector3.up);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.GetComponent<CameraView>())
            {
                _arWorld.SetActive(!_arWorld.activeSelf);
                _realWorld.SetActive(!_realWorld.activeSelf);
                _mainCamera.cullingMask ^= 1 << LayerMask.NameToLayer("ArWorld");
            }
        }
    }
}