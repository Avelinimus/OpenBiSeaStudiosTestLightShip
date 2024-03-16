using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MVC.Unity.Editor
{
    [InitializeOnLoad]
    public static class UnityInitializeEditor
    {
        public class TypeEditor
        {
            public Type Type;
            public bool EditorForChildClasses;
            public BehaviourEditor Editor;
        }

        private static bool DisplayLoadProgress
        {
            get
            {
#if EDITORS_LOAD_PROGRESS
                return true;
#endif
                return false;
            }
        }

        private static readonly OneListener _initializedListener = new OneListener();
        public static event Action INITIALIZED
        {
            add { _initializedListener.Add(value); }
            remove { _initializedListener.Remove(value); }
        }

        private static readonly List<MonoScript> _results = new List<MonoScript>();
        private static readonly List<TypeEditor> _editors = new List<TypeEditor>();
        private static readonly List<Type> _checked = new List<Type>();
        private static bool _initialized;


        static UnityInitializeEditor()
        {
            _initialized = false;
        }

        [MenuItem("Tools/UpdateEditors")]
        public static void UpdateEditors()
        {
            _checked.Clear();
            _editors.Clear();
            _results.Clear();
            var allPaths = AssetDatabase.GetAllAssetPaths().Where(s => s.Contains("Editor"));
            foreach (var path in allPaths.Where(p => p.Contains(".cs")))
            {
                var asyncScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                _results.Add(asyncScript);
            }
            EditorApplication.update += ExecuteAsyncs;
        }

        [MenuItem("Tools/Clear cache")]
        public static void ClearCache()
        {
            Debug.Log("Cache clearing "+Caching.ClearCache());
        }

        public static bool Initialized { get { return _initialized; } }

        public static BehaviourEditor Get(Type type)
        {
            if (_checked.Contains(type)) return null;
            TypeEditor result = null;
            foreach (var editor in _editors)
            {
                if ((editor.EditorForChildClasses && type.IsSubclassOf(editor.Type)) || editor.Type == type)
                {
                    result = editor;
                    break;
                }
            }

            if (result != null)
            {
                return result.Editor;
            }
            _checked.Add(type);
            return null;
        }

        private static void ExecuteAsyncs()
        {
            try
            {
                ClearProgressBar();

                var current = 0f;
                DisplayProgressBar("Parse MonoScripts", "", current / _results.Count);
                foreach (var result in _results)
                {
                    ++current;

                    var script = result;
                    if (script != null)
                    {
                        var clazz = script.GetClass();
                        if (clazz != null && clazz.IsSubclassOf(typeof(BehaviourEditor)))
                        {
                            var attributes = clazz.GetCustomAttributes(typeof(BehaviourCustomEditor), true);
                            if (attributes.Length != 0)
                            {
                                var attribute = attributes[0] as BehaviourCustomEditor;
                                if (attribute != null)
                                {
                                    _editors.Add(new TypeEditor
                                    {
                                        Editor = Activator.CreateInstance(clazz) as BehaviourEditor,
                                        EditorForChildClasses = attribute.EditorForChildClasses,
                                        Type = attribute.InspectedType
                                    });
                                }
                            }
                        }
                    }

                    DisplayProgressBar("Parse MonoScripts", "", current / _results.Count);
                }
                _results.Clear();
            }
            finally
            {
                ClearProgressBar();
                _checked.Clear();
                EditorApplication.update -= ExecuteAsyncs;
                _initialized = true;
                _initializedListener.Invoke();
            }
        }

        private static void DisplayProgressBar(string title, string info, float progress)
        {
            if (DisplayLoadProgress)
            {
                EditorUtility.DisplayProgressBar(title, info, progress);
            }
        }

        private static void ClearProgressBar()
        {
            if (DisplayLoadProgress)
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
