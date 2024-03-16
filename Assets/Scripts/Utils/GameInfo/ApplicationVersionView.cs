using TMPro;

using UnityEngine;

namespace Game.Utils
{
    public sealed class ApplicationVersionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txt;

        private void Awake()
        {
            _txt.gameObject.SetActive(Debug.isDebugBuild);
            _txt.text = "v:" + Application.version;
        }
    }
}