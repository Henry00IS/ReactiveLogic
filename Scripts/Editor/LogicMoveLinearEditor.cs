#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic.Editor
{
    [CustomEditor(typeof(LogicMoveLinear))]
    [CanEditMultipleObjects]
    public class LogicMoveLinearEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var detectedNonKinematic = false;
            var detectedNonInterpolated = false;
            foreach (var target in targets)
            {
                if (target is LogicMoveLinear logicMoveLinear)
                {
                    var rigidbody = logicMoveLinear.GetComponent<Rigidbody>();
                    if (rigidbody)
                    {
                        if (!rigidbody.isKinematic)
                            detectedNonKinematic = true;

                        if (rigidbody.interpolation == RigidbodyInterpolation.None)
                            detectedNonInterpolated = true;
                    }
                }
            }

            if (detectedNonKinematic)
            {
                EditorGUILayout.HelpBox("Non-Kinematic rigidbody detected. You probably want it to be kinematic for expected behavior.", MessageType.Warning);
            }

            if (detectedNonInterpolated)
            {
                EditorGUILayout.HelpBox("Rigidbody detected without interpolation. You probably want it to have interpolation for a smooth visual experience.", MessageType.Info);
            }

            base.OnInspectorGUI();
        }
    }
}

#endif