#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic.Editor
{
    public static class LogicCreateMenu
    {
        [MenuItem("GameObject/Reactive/Logic Auto", false)]
        private static void CreateMenuLogicAuto()
        {
            var go = EditorCreateGameObject("Logic Auto");
            go.AddComponent<LogicAuto>();
        }

        [MenuItem("GameObject/Reactive/Logic Branch", false)]
        private static void CreateMenuLogicBranch()
        {
            var go = EditorCreateGameObject("Logic Branch");
            go.AddComponent<LogicBranch>();
        }

        [MenuItem("GameObject/Reactive/Logic Case", false)]
        private static void CreateMenuLogicCase()
        {
            var go = EditorCreateGameObject("Logic Case");
            go.AddComponent<LogicCase>();
        }

        [MenuItem("GameObject/Reactive/Logic Compare", false)]
        private static void CreateMenuLogicCompare()
        {
            var go = EditorCreateGameObject("Logic Compare");
            go.AddComponent<LogicCompare>();
        }

        [MenuItem("GameObject/Reactive/Logic Counter", false)]
        private static void CreateMenuLogicCounter()
        {
            var go = EditorCreateGameObject("Logic Counter");
            go.AddComponent<LogicCounter>();
        }

        [MenuItem("GameObject/Reactive/Logic Destroy", false)]
        private static void CreateMenuLogicDestroy()
        {
            var go = EditorCreateGameObject("Logic Destroy");
            go.AddComponent<LogicDestroy>();
        }

        [MenuItem("GameObject/Reactive/Logic Filter", false)]
        private static void CreateMenuLogicFilter()
        {
            var go = EditorCreateGameObject("Logic Filter");
            go.AddComponent<LogicFilter>();
        }

        [MenuItem("GameObject/Reactive/Logic Group", false)]
        private static void CreateMenuLogicGroup()
        {
            var go = EditorCreateGameObject("Logic Group");
            go.AddComponent<LogicGroup>();
        }

        [MenuItem("GameObject/Reactive/Logic Instantiate", false)]
        private static void CreateMenuLogicInstantiate()
        {
            var go = EditorCreateGameObject("Logic Instantiate");
            go.AddComponent<LogicInstantiate>();
        }

        [MenuItem("GameObject/Reactive/Logic Log", false)]
        private static void CreateMenuLogicLog()
        {
            var go = EditorCreateGameObject("Logic Log");
            go.AddComponent<LogicLog>();
        }

        [MenuItem("GameObject/Reactive/Logic Relay", false)]
        private static void CreateMenuLogicRelay()
        {
            var go = EditorCreateGameObject("Logic Relay");
            go.AddComponent<LogicRelay>();
        }

        [MenuItem("GameObject/Reactive/Logic Timer", false)]
        private static void CreateMenuLogicTimer()
        {
            var go = EditorCreateGameObject("Logic Timer");
            go.AddComponent<LogicTimer>();
        }

        [MenuItem("GameObject/Reactive/Logic Trigger", false)]
        private static void CreateMenuLogicTrigger()
        {
            var go = EditorCreateGameObject("Logic Trigger");
            go.AddComponent<LogicTrigger>();
        }

        [MenuItem("GameObject/Reactive/Logic Unity", false)]
        private static void CreateMenuLogicUnity()
        {
            var go = EditorCreateGameObject("Logic Unity");
            go.AddComponent<LogicUnity>();
        }

#if ALPACAIT_DYNAMICLIGHTING

        [MenuItem("GameObject/Reactive/Reactive Dynamic Light", false)]
        private static void CreateMenuExternalDynamicLightingReactiveDynamicLight()
        {
            var go = EditorCreateGameObject("Reactive Dynamic Light");
            go.AddComponent<External.DynamicLighting.ReactiveDynamicLight>();
        }

#endif

        /// <summary>Adds a new game object to the scene.</summary>
        /// <param name="name">The name of the game object that will be created.</param>
        /// <returns>The game object that has been added to the scene.</returns>
        private static GameObject EditorCreateGameObject(string name)
        {
            GameObject go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);

            // place the new game object as a child of the current selection in the editor.
            var parent = Selection.activeTransform;
            if (parent)
            {
                // keep the game object transform identity.
                go.transform.SetParent(parent, false);
            }
            else
            {
                // move it in front of the current camera.
                var camera = GetSceneViewCamera();
                if (camera)
                {
                    go.transform.position = camera.transform.TransformPoint(Vector3.forward * 2f);
                }
            }

            // make sure it's selected and unity editor will let the user rename the game object.
            Selection.activeGameObject = go;
            return go;
        }

        /// <summary>Attempts to find the most likely scene view camera.</summary>
        /// <returns>The camera if found else null.</returns>
        private static Camera GetSceneViewCamera()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView)
            {
                return sceneView.camera;
            }
            else
            {
                var current = Camera.current;
                if (current)
                {
                    return current;
                }
            }
            return null;
        }
    }
}

#endif