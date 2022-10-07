// using UnityEditor;
// using UnityEditor.AssetImporters;
// using UnityEngine;

// [CustomEditor(typeof(URDFImporter))]
// public class URDFImporterEditor : ScriptedImporterEditor
// {
//     // Serialized properties
//     private SerializedProperty _xmlData;
//     private SerializedProperty _filepath;
//     private SerializedProperty _remappedMaterials;
//     private SerializedProperty _settings;
//     private SerializedProperty _chosenAxis;
//     private SerializedProperty _convexMethod;
//     private SerializedProperty _robotName;
//     private SerializedProperty _materials;

//     // UI
//     private bool _materialFoldout = true;
//     private Vector2 _scrollPostion = Vector2.zero;
    
//     /// <summary>
//     /// This method is called when a .urdf file is selected in the assets menu.
//     /// </summary>
//     public override void OnEnable()
//     {
//         base.OnEnable();
//         _xmlData = serializedObject.FindProperty("_xmlData");
//         _filepath = serializedObject.FindProperty("_filepath");
//         _remappedMaterials = serializedObject.FindProperty("_remappedMaterials");
//         _settings = serializedObject.FindProperty("_settings");
//         _chosenAxis = _settings.FindPropertyRelative("choosenAxis");
//         _convexMethod = _settings.FindPropertyRelative("convexMethod");
//         _robotName = serializedObject.FindProperty("_robotName");
//         _materials = serializedObject.FindProperty("_materials");
//     }

//     /// <summary>
//     /// Draw the custom inspector for .urdf files
//     /// </summary>
//     public override void OnInspectorGUI()
//     {
//         GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
//         {
//             alignment = TextAnchor.MiddleLeft,
//             fontSize = 13
//         };

//         // Update the serializedObject in case it has been changed outside the Inspector.
//         serializedObject.Update();

//         EditorGUILayout.LabelField("Name: " + _robotName.stringValue, titleStyle);
//         EditorGUILayout.Space();

//         // Import options
//         EditorGUILayout.PropertyField(_chosenAxis, new GUIContent("Select Axis Type"));
//         EditorGUILayout.PropertyField(_convexMethod, new GUIContent("Mesh Decomposer"));
//         EditorGUILayout.Space();
        
//         // Material remapping
//         _materialFoldout = EditorGUILayout.Foldout(_materialFoldout, "Materials remapping");
//         if (_materialFoldout)
//         {
//             for (int i = 0; i < _remappedMaterials.arraySize; i++)
//             {
//                 SerializedProperty materialProperty = _remappedMaterials.GetArrayElementAtIndex(i);
//                 EditorGUILayout.PropertyField(materialProperty, new GUIContent(_materials.GetArrayElementAtIndex(i).stringValue));
//             }
//         }
//         EditorGUILayout.Space();

//         // Show raw XML file
//         // _scrollPostion = EditorGUILayout.BeginScrollView(_scrollPostion, GUILayout.MaxHeight(500f));
//         // EditorGUILayout.TextArea(_xmlData.stringValue);
//         // EditorGUILayout.EndScrollView();

//         // Apply the changes so Undo/Redo is working
//         serializedObject.ApplyModifiedProperties();

//         GUI.enabled = !HasModified();

//         // Import button
//         if (GUILayout.Button(new GUIContent("Import URDF", GUI.enabled ? "" : "Import changes must be applied to import URDF.")))
//         {
//              Selection.activeObject = PrefabUtility.InstantiatePrefab(Selection.activeObject as GameObject);
//         }

//         GUI.enabled = true;

//         // Call ApplyRevertGUI to show Apply and Revert buttons.
//         ApplyRevertGUI();
//     }
// }