using Game.Core.Managers;

using Injection;

using MVC;

using Newtonsoft.Json;

using System;

using UnityEngine;

namespace Game.Managers
{
    public enum DataType
    {
        Localization,
        Sessions,
        GeneratorWorld,
        World
    }

    public sealed class DataManager : Manager
    {
        public const string kPatternSave = "{0}_{1}";
        private const string kPlayerPrefsKeyUser = "DataManager_User";
        private const string kEmpty = "";

        [Inject] private GameStartBehaviour _gameStartBehaviour;

        public override void Initialize()
        {
            try
            {
                var dataUser = PlayerPrefs.GetString(kPlayerPrefsKeyUser);
            }
            catch (Exception e)
            {
                Log.Exception(Channel.Core, e);
            }

            _gameStartBehaviour.APPLICATION_QUITED += OnApplicationQuited;
        }

        public override void Dispose()
        {
            _gameStartBehaviour.APPLICATION_QUITED -= OnApplicationQuited;
        }

        private void OnApplicationQuited()
        {
            Dispose();
        }

        public T Load<T>(DataType type, out bool isExists, string worldId = kEmpty) where T : new()
        {
            try
            {
                var data = PlayerPrefs.GetString(string.Format(kPatternSave, worldId, type));

                if (string.IsNullOrEmpty(data))
                {
                    isExists = false;

                    return new T();
                }

                isExists = true;

                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception e)
            {
                PlayerPrefs.DeleteAll();
                Log.Exception(Channel.Parse, e);
                isExists = false;

                return new T();
            }
        }

        public void Save(DataType type, object data, string worldId = kEmpty)
        {
            var json = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(string.Format(kPatternSave, worldId, type), json);
            PlayerPrefs.Save();
        }

        public void SaveInt(DataType type, int data, string worldId = kEmpty)
        {
            PlayerPrefs.SetInt(string.Format(kPatternSave, worldId, type), data);
            PlayerPrefs.Save();
        }

        public int LoadInt(DataType type, int defaultValue, string worldId = kEmpty)
        {
            return PlayerPrefs.GetInt(string.Format(kPatternSave, worldId, type), defaultValue);
        }

        public void DeleteSave(DataType type, string worldId = kEmpty)
        {
            PlayerPrefs.DeleteKey(string.Format(kPatternSave, worldId, type));
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}