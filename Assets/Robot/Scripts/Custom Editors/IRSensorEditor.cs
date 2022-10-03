using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor that merely draws the default editor, while making sure to update the collision cone of the sensor.
/// </summary>
[CustomEditor(typeof(IRSensor))]
[CanEditMultipleObjects]
public class IRSensorEditor : Editor 
{
    string[] _propertiesRequiringConeRebuild = { "_minRange", "_maxRange", "_fov" };
    string _sensingDirectionProperty = "_sensingDirection";
    SerializedObject _oldValues;
    
    void OnEnable()
    {
        _oldValues = serializedObject;
    }

    public override void OnInspectorGUI()
    {
        // Draw default editor
        base.OnInspectorGUI();

        IRSensor sensor = (IRSensor)target;

        serializedObject.Update();
        foreach (string prop in _propertiesRequiringConeRebuild)
        {
            if (serializedObject.FindProperty(prop).floatValue != _oldValues.FindProperty(prop).floatValue)
            {
                sensor.RequestConeColliderRebuild();
                break;
            }
        }
        
        if (serializedObject.FindProperty(_sensingDirectionProperty).vector3Value != _oldValues.FindProperty(_sensingDirectionProperty).vector3Value)
        {
            sensor.RequestConeRealignment();
        }

        _oldValues = new SerializedObject(sensor);
    }
}