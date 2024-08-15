using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

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
}

