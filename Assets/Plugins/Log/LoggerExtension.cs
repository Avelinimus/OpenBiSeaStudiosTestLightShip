using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MVC
{
    public static class LoggerExtension
    {
        private const string _pathLog = "/../Library/Log.txt";

        public static void Init(LoggerDebugType type)
        {
            Log.Header = "CookingRage";
#if !UNITY_EDITOR
            Log.LoggerDebugType = type;
#else
            Log.LoggerDebugType = LoggerDebugType.Unity;
#endif

            Log.Channels.Clear();
            Log.Channels.Add(Channel.Error);
            Log.Channels.Add(Channel.Exception);

#if !UNITY_EDITOR
            Log.IsUseTimeFormat = true;
#else
            if (LoadConfiguration())
            {
                Log.UpdateChannels();
                return;
            }
#endif
            foreach (var channel in GetValues<Channel>())
            {
                Log.Channels.Add(channel);
            }

            Log.UpdateChannels();

#if UNITY_EDITOR
            SaveConfigurationLog();
#endif
        }

        public static bool LoadConfiguration()
        {
            string path = Application.dataPath + _pathLog;

            if (File.Exists(path))
            {
                try
                {
                    string[] lines = File.ReadAllLines(path);

                    if (lines.Length > 0)
                    {
                        Log.Channels.Clear();
                    }

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (string.IsNullOrEmpty(lines[i]))
                            continue;

                        Channel channel = (Channel)Enum.Parse(typeof(Channel), lines[i]);

                        Log.Channels.Add(channel);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return false;
                }

                return true;
            }

            return false;
        }

        private static void SaveConfigurationLog()
        {
            string path = Application.dataPath + _pathLog;

            string[] saveChannel = new string[Log.Channels.Count];

            for (int i = 0; i < Log.Channels.Count; i++)
            {
                saveChannel[i] = Log.Channels[i].ToString();
            }

            File.WriteAllLines(path, saveChannel);
        }

        private static TEnum[] GetValues<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray<TEnum>();
        }
    }
}