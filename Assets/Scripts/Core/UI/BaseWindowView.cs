using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public abstract class ButtonListener : IDisposable
    {
        protected readonly Button _button;

        public Button Button => _button;

        public abstract void Dispose();

        protected ButtonListener(Button button)
        {
            _button = button;
        }
    }

    public sealed class ButtonListener<T1> : ButtonListener where T1 : Enum
    {
        private readonly T1 _action;
        private readonly object _arg;
        private Action<T1, object> _inputReceived;

        public ButtonListener(Button button, T1 action, object arg, Action<T1, object> inputReceived) : base(button)
        {
            _action = action;
            _arg = arg;
            _inputReceived = inputReceived;

            _button.onClick.AddListener(OnClicked);
        }

        public override void Dispose()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            _inputReceived.Invoke(_action, _arg);
        }
    }

    public abstract class BaseWindowView : MonoBehaviour, IDisposable
    {
        private const float kDurationAnimation = .2f;
        private const float kMaxScale = 1.1f;
        private const int kCountLoop = 2;

        public Action CLOSE_WINDOW;

        [SerializeField] private Button _closeButton;
        protected readonly CancellationToken _cancellationToken;

        protected BaseWindowView()
        {
            _cancellationToken = new CancellationToken();
        }

        protected virtual void OnEnable()
        {
            _closeButton.onClick.AddListener(CloseButton);
        }

        protected virtual void OnDisable()
        {
            _closeButton.onClick.RemoveListener(CloseButton);
        }

        public virtual async UniTask OpenAnimation()
        {
            ResetAnimation();

            await transform.DOScale(Vector3.one * kMaxScale, kDurationAnimation).SetEase(Ease.OutBack)
                .SetLoops(kCountLoop, LoopType.Yoyo).SetId(this)
                .WithCancellation(_cancellationToken);
        }

        public virtual async UniTask CloseAnimation()
        {
            ResetAnimation();

            await transform.DOScale(Vector3.one * kMaxScale, kDurationAnimation).SetEase(Ease.OutBack)
                .SetLoops(kCountLoop, LoopType.Yoyo).SetId(this)
                .WithCancellation(_cancellationToken);
        }

        public void ResetAnimation()
        {
            DOTween.Kill(this);
            transform.localScale = Vector3.one;
        }

        public void Dispose()
        {
            DOTween.Kill(this);
        }

        private void CloseButton()
        {
            CLOSE_WINDOW.SafeInvoke();
        }
    }

    public abstract class BaseWindowViewWithModel<T, T1> : BaseWindowView, IObserver
        where T : Observable where T1 : Enum
    {
        public Action<T1, object> INPUT_RECEIVED;

        private T _model;
        private readonly List<ButtonListener> _buttonListeners;

        public T Model
        {
            protected get => _model;
            set
            {
                _model?.RemoveObserver(this);

                OnApplyModel(value);

                _model = value;

                if (null == _model) return;

                _model.AddObserver(this);
                OnModelChanged(_model);
            }
        }

        protected BaseWindowViewWithModel()
        {
            _buttonListeners = new List<ButtonListener>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var buttonListener in _buttonListeners)
            {
                buttonListener.Dispose();
            }

            _buttonListeners.Clear();

            StopAllCoroutines();
        }

        protected abstract void OnModelChanged(T model);

        protected virtual void OnApplyModel(T model)
        {
        }

        protected void AddListener(Button button, T1 action, object arg = null)
        {
            _buttonListeners.Add(new ButtonListener<T1>(button, action, arg, OnInputReceived));
        }

        protected void RemoveListener(Button button)
        {
            for (int i = _buttonListeners.Count - 1; i >= 0; i--)
            {
                if (_buttonListeners[i].Button == button)
                {
                    _buttonListeners[i].Dispose();
                    _buttonListeners.RemoveAt(i);
                }
            }
        }

        private void OnInputReceived(T1 action, object arg)
        {
            INPUT_RECEIVED.SafeInvoke(action, arg);
        }

        #region Observer implementation

        public void OnObjectChanged(Observable observable)
        {
            if (observable is T model)
            {
                OnModelChanged(model);
            }
            else
            {
                OnModelChanged(Model);
            }
        }

        #endregion
    }
}