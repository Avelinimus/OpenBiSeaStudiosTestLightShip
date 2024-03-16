using System.Collections.Generic;
using MVC.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace MVC.Unity
{
    public class BehaviourEditorGUIx
    {
        private Rect fadeAreaRect;
        private Rect lastAreaRect;

        public Dictionary<string, BehaviourEditorGUIx.FadeArea> fadeAreas;

        public static int currentDepth = 0;
        public static int currentIndex = 0;
        public static bool isLayout = false;

        public static BehaviourEditor editor;

        public static GUIStyle defaultAreaStyle;
        public static GUIStyle defaultLabelStyle;
        public static GUIStyle stretchStyle;
        public static GUIStyle stretchStyleThin;

        public static float speed = 6;
        public static bool fade = true;
        public static bool fancyEffects = true;

        public Stack<BehaviourEditorGUIx.FadeArea> stack;

        public void RemoveID(string id)
        {
            if (fadeAreas == null)
            {
                return;
            }

            fadeAreas.Remove(id);
        }

        public bool DrawID(string id)
        {
            if (fadeAreas == null)
            {
                return false;
            }
            return fadeAreas[id].Show();
        }

        public void OnEnable(BehaviourEditor value)
        {
            editor = value;
        }

        public BehaviourEditorGUIx.FadeArea BeginFadeArea(bool open, string label, string id)
        {
            return BeginFadeArea(open, label, id, defaultAreaStyle);
        }

        public BehaviourEditorGUIx.FadeArea BeginFadeArea(bool open, string label, string id, GUIStyle areaStyle)
        {
            return BeginFadeArea(open, label, id, areaStyle, defaultLabelStyle);
        }

        public BehaviourEditorGUIx.FadeArea BeginFadeArea(bool open, string label, string id, GUIStyle areaStyle, GUIStyle labelStyle)
        {
            Color tmp1 = GUI.color;

            BehaviourEditorGUIx.FadeArea fadeArea = BeginFadeArea(open, id, 20, areaStyle);
            Color tmp2 = GUI.color;
            GUI.color = tmp1;

            if (label != "")
            {
                if (GUILayout.Button(label, labelStyle))
                {
                    fadeArea.open = !fadeArea.open;
                    editor.Repaint();
                }
            }

            GUI.color = tmp2;
            return fadeArea;
        }

        public class FadeArea
        {
            public Rect currentRect;
            public Rect lastRect;
            public float value;
            public float lastUpdate;
            public bool open;
            public Color preFadeColor;

            public void Switch()
            {
                lastRect = currentRect;
            }

            public FadeArea(bool open)
            {
                value = open ? 1 : 0;
            }

            public bool Show()
            {
                return open || value > 0F;
            }

            public static implicit operator bool(BehaviourEditorGUIx.FadeArea o)
            {
                return o.open;
            }
        }

        public BehaviourEditorGUIx.FadeArea BeginFadeArea(bool open, string id)
        {
            return BeginFadeArea(open, id, 0);
        }

        //openMultiple is set to false if only 1 BeginVertical call needs to be made in the BeginFadeArea (open, id) function.
        //The EndFadeArea function always closes two BeginVerticals
        public BehaviourEditorGUIx.FadeArea BeginFadeArea(bool open, string id, float minHeight)
        {
            return BeginFadeArea(open, id, minHeight, GUIStyle.none);
        }

        public BehaviourEditorGUIx.FadeArea BeginFadeArea(bool open, string id, float minHeight, GUIStyle areaStyle)
        {
            if (editor == null)
            {
                Debug.LogError("You need to set the 'EditorGUIx.editor' variable before calling this function");
                return null;
            }

            if (stretchStyle == null)
            {
                stretchStyle = new GUIStyle();
                stretchStyle.stretchWidth = true;
            }

            if (stack == null)
            {
                stack = new Stack<BehaviourEditorGUIx.FadeArea>();
            }

            if (fadeAreas == null)
            {
                fadeAreas = new Dictionary<string, BehaviourEditorGUIx.FadeArea>();
            }

            if (!fadeAreas.ContainsKey(id))
            {
                fadeAreas.Add(id, new BehaviourEditorGUIx.FadeArea(open));
            }

            BehaviourEditorGUIx.FadeArea fadeArea = fadeAreas[id];

            stack.Push(fadeArea);

            fadeArea.open = open;

            areaStyle.stretchWidth = true;

            Rect lastRect = fadeArea.lastRect;

            if (!fancyEffects)
            {
                fadeArea.value = open ? 1F : 0F;
                lastRect.height -= minHeight;
                lastRect.height = open ? lastRect.height : 0;
                lastRect.height += minHeight;
            }
            else
            {
                lastRect.height = lastRect.height < minHeight ? minHeight : lastRect.height;
                lastRect.height -= minHeight;
                float faded = Hermite(0F, 1F, fadeArea.value);
                lastRect.height *= faded;
                lastRect.height += minHeight;
                lastRect.height = Mathf.Round(lastRect.height);
            }

            Rect gotLastRect = GUILayoutUtility.GetRect(new GUIContent(), areaStyle, GUILayout.Height(lastRect.height));

            GUILayout.BeginArea(lastRect, areaStyle);

            Rect newRect = EditorGUILayout.BeginVertical();

            if (Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel)
            {
                newRect.x = gotLastRect.x;
                newRect.y = gotLastRect.y;
                newRect.width = gotLastRect.width;
                newRect.height += areaStyle.padding.top + areaStyle.padding.bottom;
                fadeArea.currentRect = newRect;

                if (fadeArea.lastRect != newRect)
                {
                    editor.Repaint();
                }

                fadeArea.Switch();
            }
            if (Event.current.type == EventType.Repaint)
            {
                float value = fadeArea.value;
                float targetValue = open ? 1F : 0F;

                float newRectHeight = fadeArea.lastRect.height;
                float deltaHeight = 400F / newRectHeight;

                float deltaTime = Mathf.Clamp(Time.realtimeSinceStartup - fadeAreas[id].lastUpdate, 0.00001F, 0.05F);

                deltaTime *= Mathf.Lerp(deltaHeight * deltaHeight * 0.01F, 0.8F, 0.9F);

                fadeAreas[id].lastUpdate = Time.realtimeSinceStartup;

                if (Event.current.shift)
                {
                    deltaTime *= 0.05F;
                }

                if (Mathf.Abs(targetValue - value) > 0.001F)
                {

                    float time = Mathf.Clamp01(deltaTime * speed);
                    value += time * Mathf.Sign(targetValue - value);
                    editor.Repaint();
                }
                else
                {
                    value = Mathf.Round(value);
                }

                fadeArea.value = Mathf.Clamp01(value);
            }

            if (fade)
            {
                Color c = GUI.color;
                fadeArea.preFadeColor = c;
                c.a *= fadeArea.value;
                GUI.color = c;
            }

            fadeArea.open = open;

            return fadeArea;
        }

        public void EndFadeArea()
        {

            if (stack.Count <= 0)
            {
                Debug.LogError("You are popping more Fade Areas than you are pushing, make sure they are balanced");
                return;
            }

            BehaviourEditorGUIx.FadeArea fadeArea = stack.Pop();
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();

            if (fade)
            {
                GUI.color = fadeArea.preFadeColor;
            }
        }

        public static float Hermite(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
        }

        public static float Sinerp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
        }

        public static float Coserp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
        }

        public static float Berp(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) *
                    (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }
    }
}
