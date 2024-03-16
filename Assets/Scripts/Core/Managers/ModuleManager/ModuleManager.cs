using Injection;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Core.Managers
{
    public sealed class ModuleManager : Manager
    {
        [Inject] private Injector _injector;

        public readonly Dictionary<object, List<Module>> _moduleMap;

        public ModuleManager()
        {
            _moduleMap = new Dictionary<object, List<Module>>();
        }

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }

        public void AddModule<T>(object objectToMap, params object[] args) where T : Module
        {
            var result = (T)Activator.CreateInstance(typeof(T), args);
            AddModuleInternal(objectToMap, result);
        }

        public void AddModule<T, T1>(object objectToMap, Component moduleView, params object[] args) where T : Module
        {
            var view = moduleView.GetComponent<T1>();

            if (null == view)
                return;

            var newArgs = new object[args.Length + 1];
            newArgs[0] = view;

            for (int i = 0; i < args.Length; i++)
            {
                newArgs[i + 1] = args[i];
            }

            var result = (T)Activator.CreateInstance(typeof(T), newArgs);

            AddModuleInListInternal(objectToMap, result);
            _injector.Inject(result);
            result.Initialize();
        }

        public void SetPauseModule<T>(object objectToMap, bool isActive)
        {
            var listWithModules = _moduleMap[objectToMap];

            for (int i = listWithModules.Count - 1; i >= 0; i--)
            {
                if (listWithModules[i] is T)
                {
                    listWithModules[i].SetPause(isActive);
                }
            }
        }

        public void DisposeAllLevelModules()
        {
            foreach (var levelModule in _moduleMap)
            {
                foreach (var module in levelModule.Value)
                {
                    module.Dispose();
                }
            }

            _moduleMap.Clear();
        }

        public void DisposeModule<T>(object objectToMap)
        {
            if (!_moduleMap.TryGetValue(objectToMap, out var listWithModules))
                return;

            for (int i = listWithModules.Count - 1; i >= 0; i--)
            {
                if (listWithModules[i] is T)
                {
                    listWithModules[i].Dispose();
                    listWithModules.RemoveAt(i);
                }
            }
        }

        public void DisposeModules(object objectToMap)
        {
            if (!_moduleMap.TryGetValue(objectToMap, out var listWithModules))
                return;

            for (int i = listWithModules.Count - 1; i >= 0; i--)
            {
                listWithModules[i].Dispose();
            }

            listWithModules.Clear();
            _moduleMap.Remove(objectToMap);
        }

        private void AddModuleInternal(object objectToMap, Module result)
        {
            AddModuleInListInternal(objectToMap, result);
            _injector.Inject(result);
            result.Initialize();
        }

        private void AddModuleInListInternal(object objectToMap, Module result)
        {
            if (_moduleMap.TryGetValue(objectToMap, out var moduleList))
            {
                moduleList.Add(result);
            }
            else
            {
                _moduleMap.Add(objectToMap, new List<Module> { result });
            }
        }
    }
}