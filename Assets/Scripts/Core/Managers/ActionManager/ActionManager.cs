using Injection;

using System;
using System.Collections.Generic;

namespace Game.Core.Managers
{
    public sealed class ExtendedActionManager : ActionManager
    {
        public List<ActionBase> Actions
        {
            get { return _actions; }
        }
    }

    public class ActionManager : IDisposable
    {
        [Inject]
        private Context _context;

        protected readonly List<ActionBase> _actions = new List<ActionBase>();

        /// <summary>
        /// цепочка екшенів
        /// </summary>
        /// <param name="actions"></param>
        public void AddAction(ActionBase action)
        {
            _context.Get<Injector>().Inject(action);
            _actions.Add(action);
            action.Run(this, _context);
        }

        internal void RemoveAction(ActionBase action)
        {
            _actions.Remove(action);
        }

        public void Dispose()
        {
            _actions.Clear();
        }
    }
}