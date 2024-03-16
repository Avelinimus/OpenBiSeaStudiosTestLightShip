using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class WindowBackgroundView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public float BackgroundAlpha
        {
            set
            {
                _image.raycastTarget = value > 0;
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, value);
            }
        }
    }
}