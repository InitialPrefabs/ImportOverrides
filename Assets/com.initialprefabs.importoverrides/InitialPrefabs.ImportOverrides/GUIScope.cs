using System;
using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    public readonly ref struct IndentScope {

        private readonly int indentLevel;

        public IndentScope(int indentLevel) {
            this.indentLevel = indentLevel;
            EditorGUI.indentLevel += indentLevel;
        }

        public void Dispose() {
            EditorGUI.indentLevel -= indentLevel;
        }
    }

    public readonly ref struct GUIScope {

        public static GUIScope Disabled() {
            return new GUIScope(false);
        }

        public static GUIScope Enabled() {
            return new GUIScope(true);
        }

        public GUIScope(bool enabled) {
            GUI.enabled = enabled;
        }

        public void Dispose() {
            GUI.enabled = true;
        }
    }

    public readonly ref struct ChangeCheckScope {
        private readonly Action onComplete;

        private ChangeCheckScope(Action onComplete) {
            this.onComplete = onComplete;
        }

        public static ChangeCheckScope Begin() {
            return Begin(null);
        }

        public static ChangeCheckScope Begin(Action onComplete) {
            EditorGUI.BeginChangeCheck();
            return new ChangeCheckScope(onComplete);
        }

        public void Dispose() {
            if (EditorGUI.EndChangeCheck()) {
                onComplete?.Invoke();
            }
        }
    }
}

