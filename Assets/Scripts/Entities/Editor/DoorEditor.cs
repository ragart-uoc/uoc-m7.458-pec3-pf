using UnityEditor;

namespace PEC3.Entities.Editor
{
    [CustomEditor(typeof(Door))]
    public class DoorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var door = (Door) target;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doorType"));
            var currentSelectedBehaviour = door.doorType;
            switch (currentSelectedBehaviour)
            {
                case Door.DoorTypes.Automatic:
                    break;
                case Door.DoorTypes.Locked:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("keycardColor"));
                    break;
                case Door.DoorTypes.Timed:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("timerButtonPressed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTime"));
                    break;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftDoor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightDoor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftClosedLocation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightClosedLocation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftOpenLocation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightOpenLocation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doorSpeed"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
