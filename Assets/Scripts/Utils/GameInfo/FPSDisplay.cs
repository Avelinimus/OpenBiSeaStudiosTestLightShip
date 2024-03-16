using System;
using TMPro;
using UnityEngine;

namespace Game.Utils
{
    public sealed class FPSDisplay : MonoBehaviour
    {
        public static Action<float> FPS_RECEIVED;

        private const float _medianLearnrate = .05f;

        [SerializeField] private readonly float _updateInterval = .5f;
        [SerializeField] private readonly bool _showMedian = false;
        [SerializeField] private TMP_Text _fpsText;

        private float _accumulated;
        private int _frames;
        private float _timeLeft;
        private float _currentFPS;

        private float _median;
        private float _average;
        private int _lastTime;

        private void Awake()
        {
            _lastTime = Environment.TickCount;
            _timeLeft = _updateInterval;
        }

        private void Update()
        {
            var deltaTime = (Environment.TickCount - _lastTime) / 1000f;
            _lastTime = Environment.TickCount;

            _timeLeft -= deltaTime;

            if (deltaTime > 0)
            {
                _accumulated += Time.timeScale / deltaTime;
            }

            ++_frames;

            if (_timeLeft <= 0.0)
            {
                _currentFPS = _accumulated / _frames;

                if (null != FPS_RECEIVED)
                {
                    FPS_RECEIVED.Invoke(_currentFPS);
                }

                _average += (Mathf.Abs(_currentFPS) - _average) * 0.1f;

                _median += Mathf.Sign(_currentFPS - _median) *
                           Mathf.Min(_average * _medianLearnrate, Mathf.Abs(_currentFPS - _median));

                float fps = _showMedian ? _median : _currentFPS;
                _fpsText.text = $"{fps:F2} FPS ({1000.0f / fps:F1} ms)";

                _timeLeft = _updateInterval;
                _accumulated = 0.0F;
                _frames = 0;
            }
        }
    }
}