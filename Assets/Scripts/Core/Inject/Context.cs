using System;
using System.Collections.Generic;
using System.Linq;

namespace Injection
{
    public sealed class Context : IDisposable
    {
        private readonly Dictionary<Type, object> _objectsMap;

        public Context()
        {
            _objectsMap = new Dictionary<Type, object>(100);
            _objectsMap[typeof(Context)] = this;
        }

        public Context(Context parent)
        {
            _objectsMap = new Dictionary<Type, object>(parent._objectsMap);
            _objectsMap[typeof(Context)] = this;
        }

        public void Dispose()
        {
            var keys = _objectsMap.Keys.ToList();

            foreach (var key in keys)
            {
                if (!_objectsMap.ContainsKey(key) || null == _objectsMap[key])
                    continue;

                var value = _objectsMap[key];

                if (value is IDisposable && !(value is Context))
                {
                    (value as IDisposable).Dispose();
                }
            }
            _objectsMap.Clear();
        }

        public void Install(params object[] objects)
        {
            foreach (object obj in objects)
            {
                _objectsMap[obj.GetType()] = obj;
            }
        }

        public void InstallByType(object obj, Type type)
        {
            _objectsMap[type] = obj;
        }

        public void ApplyInstall()
        {
            var injector = Get<Injector>();
            foreach (object obj in _objectsMap.Values)
            {
                injector.Inject(obj);
            }
        }

        public void Uninstall(params object[] objects)
        {
            foreach (object obj in objects)
            {
                if (null == obj)
                    continue;

                _objectsMap.Remove(obj.GetType());
            }
        }

        public void UninstallByType(Type type)
        {
            _objectsMap.Remove(type);
        }

        public T Get<T>() where T : class
        {
#if UNITY_EDITOR
            if (!_objectsMap.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("Not found " + typeof(T));
            }
#endif

            return (T)_objectsMap[typeof(T)];
        }

        public object Get(Type type)
        {
#if UNITY_EDITOR
            if (!_objectsMap.ContainsKey(type))
            {
                throw new KeyNotFoundException("Not found " + type);
            }
#endif
            return _objectsMap[type];
        }

        public void FillLinks(List<object> links)
        {
            links.AddRange(_objectsMap.Values);
        }
    }
}