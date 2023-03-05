#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic.Editor.Internal
{
    /// <summary>Alternative for <see cref="EditorGUILayout"/> that works in <see cref="PropertyDrawer"/>.</summary>
    internal static class HenryEditorGUI
    {
        public static bool isInHorizontalSection = false;
        public static Rect rectBackup = Rect.zero;
        public static bool active = false;
        public static Rect rectInitial = Rect.zero;
        public static Rect rect = Rect.zero;

        public static int indentLevel
        {
            get
            {
                return EditorGUI.indentLevel;
            }
            set
            {
                if (!active) return;
                EditorGUI.indentLevel = value;
                rect.x = value * 20f;
            }
        }

        public static void LabelField(string label, GUIStyle style)
        {
            if (!active) { Space(); return; };

            EditorGUI.LabelField(rect, label, style);
            Space();
        }

        public static void LabelField(string label)
        {
            if (!active) { Space(); return; };

            EditorGUI.LabelField(rect, label);
            Space();
        }

        public static void Space(int distance = 20)
        {
            if (isInHorizontalSection) return;
            rect.y += distance;
        }

        public static void BeginHorizontal()
        {
            isInHorizontalSection = true;
            rectBackup = rect;
        }

        public static void EndHorizontal()
        {
            isInHorizontalSection = false;
            rect = rectBackup;

            Space();
        }

        public static bool Button(GUIContent guiContent)
        {
            if (!active) { Space(); return false; };

            var crect = rect;
            crect.x += 15f;
            crect.width -= 15f;
            var result = GUI.Button(crect, guiContent);
            Space();
            return result;
        }

        public static void Horizontal(float width1, float width2, Action gui1, Action gui2)
        {
            BeginHorizontal();

            var backup = rect;

            rect.width = width1;
            gui1();
            rect.x += width1;
            rect.width = width2;
            gui2();

            rect = backup;

            EndHorizontal();
        }

        public static void Horizontal(float width1, float width2, float width3, Action gui1, Action gui2, Action gui3)
        {
            BeginHorizontal();

            var backup = rect;

            rect.width = width1;
            gui1();
            rect.x += width1;
            rect.width = width2;
            gui2();
            rect.x += width2;
            rect.width = width3;
            gui3();

            rect = backup;

            EndHorizontal();
        }

        public static void Horizontal(float width1, float width2, float width3, float width4, Action gui1, Action gui2, Action gui3, Action gui4)
        {
            BeginHorizontal();

            var backup = rect;

            rect.width = width1;
            gui1();
            rect.x += width1;
            rect.width = width2;
            gui2();
            rect.x += width2;
            rect.width = width3;
            gui3();
            rect.x += width3;
            rect.width = width4;
            gui4();

            rect = backup;

            EndHorizontal();
        }

        public static int Popup(int selectedIndex, GUIContent[] cacheReactiveOutputs)
        {
            if (!active) { Space(); return selectedIndex; };

            selectedIndex = EditorGUI.Popup(rect, selectedIndex, cacheReactiveOutputs);
            Space();
            return selectedIndex;
        }

        public static string TextField(string text)
        {
            if (!active) { Space(); return text; };

            text = EditorGUI.TextField(rect, text);
            Space();
            return text;
        }

        public static float FloatField(float value)
        {
            if (!active) { Space(); return value; };

            value = EditorGUI.FloatField(rect, value);
            Space();
            return value;
        }
    }
}

#endif