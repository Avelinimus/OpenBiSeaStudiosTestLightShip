using Injection;

using MVC;

using System;
using System.Collections.Generic;

namespace Game.Core.Managers
{
    public class ActionBase : IDisposable
    {
        private readonly OneListener<bool> _completeListener = new OneListener<bool>();
        /// <summary>
        /// завершення виконання екшена і всіх під-екшенів, переметр - результат виконання
        /// </summary>
        public event Action<bool> COMPLETE
        {
            add { _completeListener.Add(value); }
            remove { _completeListener.Remove(value); }
        }


        protected Context _context;
        protected ActionManager _actionManager;

        private List<ActionBase> _children = new List<ActionBase>();

        /// <summary>
        /// чи актуальна змінна _childrenComleted
        /// </summary>
        private bool _valid;

        /// <summary>
        /// фініш виконання дії
        /// </summary>
        private bool _completed;

        /// <summary>
        /// чи завершились виконуваться всі діти
        /// </summary>
        private bool _childrenComleted;

        /// <summary>
        /// чи очікується закінчення викодіння child
        /// </summary>
        private bool _waiting;

        /// <summary>
        /// результат виконання екшена
        /// </summary>
        private bool _isSuccess = true;

        /// <summary>
        /// чи визивався метод Do
        /// </summary>
        private bool _isDid;

        public bool IsDid { get { return _isDid; } }

        /// <summary>
        /// тіло екшена
        /// </summary>
        /// <returns>false for async actions</returns>
        public virtual bool Do()
        {
            return true;
        }

        /// <summary>
        /// чи виконався поточний екшен і всі під-екшени
        /// </summary>
        public bool IsComplete
        {
            get
            {
                Validate();

                if (_children.Count > 0)
                {
                    return _childrenComleted && _completed;
                }
                return _completed;
            }
        }

        /// <summary>
        /// викликається для асинхроних екшенів
        /// </summary>
        /// <param name="result">результат виконання</param>
        public void Complete(bool result)
        {
            _completed = true;
            _valid = false;
            _isSuccess = result;

            if (result == false)
            {
                CancelCompleteHandler();
                _actionManager.RemoveAction(this);
                _completeListener.Invoke(false);
                Dispose();
                return;
            }

            if (IsComplete)
            {
                SuccessCompleteHandler();
                _actionManager.RemoveAction(this);
                _completeListener.Invoke(result);
                Dispose();
            }
            else
            {
                TryStartChildren();
            }
        }

        /// <summary>
        /// результат виконання
        /// </summary>
        public bool IsSuccess
        {
            get { return _isSuccess; }
        }

        /// <summary>
        /// валідація під-екшенів
        /// </summary>
        private void Validate()
        {
            if (_valid)
            {
                return;
            }

            _childrenComleted = true;

            for (int i = 0; i < _children.Count; i++)
            {
                if (!_children[i].IsComplete)
                {
                    _childrenComleted = false;
                    break;
                }
            }

            _valid = true;
        }

        /// <summary>
        /// додати вкладений екшн
        /// </summary>
        /// <param name="child">новий екшен</param>
        public void AddAction(ActionBase child)
        {
            if (IsComplete)
            {
                Log.Error(Channel.Log, "[ActionBase][AddAction] Не можна додати під-екшн в екшн який вже виконався.");
                AddNewQueue(child);
                return;
            }

            _valid = false;

            _children.Add(child);
            child.COMPLETE += ChildCompleteHandler;

            TryStartChildren();
        }

        public void AddActions(params ActionBase[] children)
        {
            foreach (var actionBase in children)
            {
                AddAction(actionBase);
            }
        }

        protected void AddNewQueue(ActionBase child)
        {
            _actionManager.AddAction(child);
        }

        protected virtual void SuccessCompleteHandler()
        {

        }

        protected virtual void CancelCompleteHandler()
        {

        }

        internal void Run(ActionManager actionManager, Context context)
        {
            Init(actionManager, context);

            if (Do())
            {
                Complete(true);
            }
        }

        protected virtual void Initialize() { }

        internal void Init(ActionManager actionManager, Context context)
        {
            _actionManager = actionManager;
            _context = context;
            _isDid = true;
            Initialize();
        }

        private void ChildCompleteHandler(bool result)
        {
            _waiting = false;
            _valid = false;

            _isSuccess = result;

            if (result == false)
            {
                _children.Clear();
                CancelCompleteHandler();
                _actionManager.RemoveAction(this);
                _completeListener.Invoke(false);
                Dispose();
                _completeListener.RemoveAll();
                return;
            }

            if (IsComplete)
            {
                _children.Clear();
                SuccessCompleteHandler();
                _actionManager.RemoveAction(this);
                _completeListener.Invoke(true);
                Dispose();
                _completeListener.RemoveAll();
            }
            else
            {
                TryStartChildren();
            }
        }

        private void TryStartChildren()
        {
            if (_waiting || !_completed)
            {
                return;
            }

            for (int i = 0; i < _children.Count; i++)
            {
                if (!_children[i]._completed)
                {
                    _waiting = true;
                    if (_children[i]._isDid)
                    {
                        break;
                    }

                    _context.Get<Injector>().Inject(_children[i]);
                    _children[i].Init(_actionManager, _context);

                    if (_children[i].Do())
                    {
                        _valid = false;
                        if (_children.Count > 0 && !_children[i]._completed)
                        {
                            _children[i].Complete(_children[i].IsSuccess);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (_children.Count > 0 && _children[i]._completed && !_children[i].IsSuccess)
                    {
                        break;
                    }

                    if (_children.Count > 0 && _children[i]._isDid && !_children[i].IsComplete)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// відписатися від всіх івентів, на які підписався екшен
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}