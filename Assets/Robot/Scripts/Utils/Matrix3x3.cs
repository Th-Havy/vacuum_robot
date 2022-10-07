using UnityEngine;
using UnityEditor;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

/// <summary>
/// 3x3 double matrix class.
/// </summary>
[System.Serializable]
public class Matrix3x3
{
    [SerializeField]
    private double[] _coeffs = new double[9];
    public double this[int i, int j]
    {
        get => this._coeffs[3 * i + j];
        set => this._coeffs[3 * i + j] = value;
    }

    public Matrix3x3(double fillValue = 0)
    {
        _coeffs = new double[9];

        for (int i = 0; i < _coeffs.Length; i++)
        {
            _coeffs[i] = fillValue;
        }
    }

    public Matrix3x3(double[] coeffs)
    {
        if (coeffs.Length != 9)
            throw new System.ArgumentException("Invalid array size, should be 9", "coeffs");

        _coeffs = new double[9];

        for (int i = 0; i < _coeffs.Length; i++)
        {
            _coeffs[i] = coeffs[i];
        }
    }

    public double[] toArray()
    {
        return _coeffs;
    }

    /// <summary>
    /// Reorder the matrix from a source coordinate system (C1) to another system (C2).
    /// </summary>
    public Matrix3x3 FromTo<C1, C2>()
        where C1 : ICoordinateSpace, new()
        where C2 : ICoordinateSpace, new()
    {
        Matrix3x3 mat = new Matrix3x3(0);

        // Find reordering of matrix rows/columns
        Vector3<C2> reordering = (new Vector3<C1>(0, 1, 2)).To<C2>();
        int[] orderIndices = new int[3];
        orderIndices[0] = (int)Mathf.Abs(reordering.x);
        orderIndices[1] = (int)Mathf.Abs(reordering.y);
        orderIndices[2] = (int)Mathf.Abs(reordering.z);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                mat[i, j] = this[orderIndices[i], orderIndices[j]];
            }
        }

        return mat;
    }

    public Matrix3x3 FromUnityToROS()
    {
        return FromTo<RUF, FLU>();
    }

    public Matrix3x3 FromROSToUnity()
    {
        return FromTo<FLU, RUF>();
    }
}

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