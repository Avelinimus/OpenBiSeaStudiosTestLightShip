using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using MVC;
using System.IO;

namespace Tools
{
    public class UnityLogEditor : EditorWindow
    {
        [MenuItem("Tools/UnityLogs")]
        public static void Open()
        {
            var window = GetWindow<UnityLogEditor>();
            window.Show(true);
            
            window.minSize = new Vector2(400, 400);
        }
        
        private Dictionary<Channel , bool> _checkChannels = new Dictionary<Channel,bool>();
                
        private void OnEnable()
        {
            LoggerExtension.LoadConfiguration();
            CreateListChannel();
        }
        
        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            for (int index = 0; index < _checkChannels.Count; index++)
            {
                var item = _checkChannels.ElementAt(index);
                var itemKey = item.Key;
                bool itemValue = item.Value;

                GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                itemValue = EditorGUILayout.ToggleLeft(itemKey.ToString() , itemValue);
                
                GUILayout.EndHorizontal();

                _checkChannels[itemKey] = itemValue;

                if (!itemValue && Log.Channels.Contains(itemKey))
                {
                    Log.Channels.Remove(itemKey);
                }

                if (itemValue && !Log.Channels.Contains(itemKey))
                {
                    Log.Channels.Add(itemKey);
                }
            }
            GUILayout.EndVertical();
            
            GUILayout.Space(20);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select all chanels"))
            {
                SelectAllChannels();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Disable all chanels"))
            {
                DisableAllChannels();
            }
            GUILayout.EndHorizontal();
             
            if (GUILayout.Button("Save configuration"))
            {
                SaveConfigurationLog();
            }
         }

        private void OnDisable()
        {
             if(_checkChannels != null)
            _checkChannels.Clear();
        }
        
        private void CreateListChannel()
        {
            foreach (Enum e in Enum.GetValues(typeof(Channel)))
            {
                if (Log.Channels.Contains(e))
                {
                    _checkChannels.Add((Channel)e , true);
                }
                else
                {
                    _checkChannels.Add((Channel)e , false);
                }
            }
        }

        private void SelectAllChannels()
        {
            foreach (Enum e in Enum.GetValues(typeof(Channel)))
            {
                if (!Log.Channels.Contains(e))
                {
                    Log.Channels.Add(e);

                    _checkChannels[(Channel)e] = true;
                }
            }
        }

        private void DisableAllChannels()
        {
            foreach (Enum e in Enum.GetValues(typeof(Channel)))
            {
                if (Log.Channels.Contains(e))
                {
                    Log.Channels.Remove(e);

                    _checkChannels[(Channel)e] = false;
                }
            }
        }

        private void SaveConfigurationLog()
        {
            string path = Application.dataPath + "/../Library/Log.txt";
            
            string [] saveChannel = new string [Log.Channels.Count];
            int index = 0;
            
            for (int i = 0; i < _checkChannels.Count; i++)
            {
                var item = _checkChannels.ElementAt(i);

                if (item.Value)
                {
                    saveChannel[index] = item.Key.ToString();
                    index++;
                }
            }

            File.WriteAllLines(path, saveChannel);
        }
    }
}