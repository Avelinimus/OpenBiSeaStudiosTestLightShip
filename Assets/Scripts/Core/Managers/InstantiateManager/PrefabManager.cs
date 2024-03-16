using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Core.Managers
{
    public sealed class PrefabManager : Manager
    {
        private const string kPrefabPoolName = "PrefabPool";

        private readonly Dictionary<string, Queue<GameObject>> _pools;
        private readonly List<PrefabResult> _prefabs;
        private Transform _prefabPool;

        public PrefabManager()
        {
            _pools = new Dictionary<string, Queue<GameObject>>();
            _prefabs = new List<PrefabResult>();
        }

        public override void Initialize()
        {
            var root = Object.FindObjectOfType<GameStartBehaviour>().transform;
            var prefabPool = GameObject.Find(kPrefabPoolName);

            if (prefabPool == null)
            {
                prefabPool = new GameObject(kPrefabPoolName);
                prefabPool.transform.SetParent(root);
            }

            _prefabPool = prefabPool.transform;
        }

        public override void Dispose()
        {
            RemoveForeverAll();
            _prefabs.Clear();
        }

        public GameObject Get(string path, Transform transform = null)
        {
            GameObject result;

            var isExists = _pools.ContainsKey(path);

            if (transform == null)
            {
                transform = _prefabPool;
            }

            if (isExists)
            {
                var pool = _pools[path];

                if (pool.Count > 0)
                {
                    result = pool.Dequeue();
                    result.SetActive(true);
                }
                else
                {
                    var prefabResult = new PrefabResult(path, _prefabs.Count);
                    result = prefabResult.Instantiate(transform);
                    _prefabs.Add(prefabResult);
                }
            }
            else
            {
                var prefabResult = new PrefabResult(path, _prefabs.Count);
                result = prefabResult.Instantiate(transform);
                _pools.Add(path, new Queue<GameObject>());
                _prefabs.Add(prefabResult);
            }


            return result;
        }

        public async UniTask<GameObject> GetAsync(string path, Transform transform = null)
        {
            GameObject result;

            var isExists = _pools.ContainsKey(path);

            if (transform == null)
            {
                transform = _prefabPool;
            }

            if (isExists)
            {
                var pool = _pools[path];

                if (pool.Count > 0)
                {
                    result = pool.Dequeue();
                    result.SetActive(true);
                }
                else
                {
                    await UniTask.Yield();
                    var prefabResult = new PrefabResult(path, _prefabs.Count);
                    result = prefabResult.Instantiate(transform);
                    _prefabs.Add(prefabResult);
                }
            }
            else
            {
                await UniTask.Yield();
                var prefabResult = new PrefabResult(path, _prefabs.Count);
                result = prefabResult.Instantiate(transform);
                _pools.Add(path, new Queue<GameObject>());
                _prefabs.Add(prefabResult);
            }


            return result;
        }

        public void Remove(GameObject gameObject)
        {
            var prefabResult = _prefabs.FirstOrDefault(temp => temp.IsRemove(gameObject));

            if (prefabResult == null) return;

            prefabResult.Hide();
            _pools[prefabResult.Path].Enqueue(gameObject);
        }


        public void RemoveForever(GameObject gameObject)
        {
            var prefabResult = _prefabs.FirstOrDefault(temp => temp.IsRemove(gameObject));

            if (prefabResult == null) return;

            prefabResult.Dispose();
            _prefabs.Remove(prefabResult);
        }


        public void RemoveAll()
        {
            foreach (var prefabResult in _prefabs)
            {
                prefabResult.Hide();
            }
        }


        public void RemoveForeverAll()
        {
            foreach (var prefabResult in _prefabs)
            {
                prefabResult.Dispose();
            }

            _pools.Clear();
            _prefabs.Clear();
        }


        private sealed class PrefabResult : IDisposable
        {
            public string Path;
            public int Id;

            private GameObject _gameObject;

            public bool IsActive => _gameObject.activeSelf;

            public PrefabResult(string path, int id)
            {
                Path = path;
                Id = id;
            }

            ~PrefabResult()
            {
                ReleaseUnmanagedResources();
            }

            public void Dispose()
            {
                ReleaseUnmanagedResources();
                GC.SuppressFinalize(this);
            }

            private void ReleaseUnmanagedResources()
            {
                Path = null;
                Id = -1;
                Object.Destroy(_gameObject);
            }

            public GameObject Instantiate(Transform transform)
            {
                var resource = Resources.Load<GameObject>(Path);
                _gameObject = Object.Instantiate(resource, transform);

                return _gameObject;
            }

            public GameObject Recall(Transform transform)
            {
                _gameObject.SetActive(true);
                _gameObject.transform.SetParent(transform);

                return _gameObject;
            }

            public bool IsRemove(GameObject gameObject)
            {
                return _gameObject.Equals(gameObject);
            }

            public void Hide()
            {
                _gameObject.SetActive(false);
            }
        }
    }
}