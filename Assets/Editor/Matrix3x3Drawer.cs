using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Matrix3x3))]
public class Matrix3x3Drawer : PropertyDrawer {
    private bool unfolded = true;
    private float verticalPadding = 2f;

    public float LineHeight
    {
        get => EditorGUIUtility.singleLineHeight;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (unfolded)
            return LineHeight * 4;
        else
            return LineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw name of matrix, and allow folding the fields for its values
        Rect headerRect = position;
        headerRect.height = LineHeight;
        unfolded = EditorGUI.BeginFoldoutHeaderGroup(headerRect, unfolded, label);

        if (unfolded)
        {
            Rect foldRect = EditorGUI.IndentedRect(position);
            foldRect.y += LineHeight;
            foldRect.height -= LineHeight;

            SerializedProperty coeffs = property.FindPropertyRelative("_coeffs");

            for (int i = 0; i < 3; i++)
            {
                float[] matrixRow = new float[3];
                SerializedProperty[] matrixRowProps = new SerializedProperty[3];
                for (int j = 0; j < 3; j++)
                {
                    matrixRowProps[j] = coeffs.GetArrayElementAtIndex(3 * i + j);
                    matrixRow[j] = (float)matrixRowProps[j].doubleValue;
                }

                Rect rect = new Rect(foldRect.x,
                                     foldRect.y + i * LineHeight + verticalPadding,
                                     foldRect.width,
                                     LineHeight - verticalPadding);

                // Draw each row of the matrix
                GUIContent[] subLabels = {new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z")};
                EditorGUI.MultiFloatField(rect, subLabels, matrixRow);

                // Update properties
                for (int j = 0; j < 3; j++)
                {
                    matrixRowProps[j].doubleValue = (double)matrixRow[j];
                }
            }
        }

        EditorGUI.EndFoldoutHeaderGroup();

        EditorGUI.EndProperty();
    }
}
