using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this script to a URDFRobot gameobject for which you wish to flip the collisions or visuals.
/// For simplicity no custom editor is created, but functions are simply called when tickboxes are ticked.
/// </summary>
[ExecuteInEditMode]
public class FlipRobotVisuals : MonoBehaviour
{
    // public enum FlippedObjects
    // {
    //     Visuals,
    //     Collisions
    // }

    // [Tooltip("Select which objects to flip in the URDFRobot.")]
    // public FlippedObjects objectsToFlip;
    // [Tooltip("Correct the visuals to the selected axis type.")]
    // public RosSharp.ImportSettings.axisType axisCorrection;
    // [Tooltip("Tick this to flip the objects.")]
    // public bool flipObjects;

    // void Update()
    // {
    //     if (flipObjects)
    //     {
    //         Flip();
    //     }
        
    //     flipObjects = false;        
    // }

    // void Flip()
    // {
    //     Quaternion correctYtoZ = Quaternion.Euler(-90, 0, 90);
    //     Quaternion correctZtoY = Quaternion.Inverse((correctYtoZ));
    //     Quaternion correction = new Quaternion();

    //     if (axisCorrection == RosSharp.ImportSettings.axisType.zAxis)
    //         correction = correctYtoZ;
    //     else
    //         correction = correctZtoY;

    //     // Register rotations for undo system
    //     UnityEditor.Undo.IncrementCurrentGroup();
    //     UnityEditor.Undo.SetCurrentGroupName("Flip Objects");
    //     int undoGroupIndex = UnityEditor.Undo.GetCurrentGroup();

    //     if (objectsToFlip == FlippedObjects.Visuals)
    //     {
    //         RosSharp.Urdf.UrdfVisual[] visualMeshList = gameObject.GetComponentsInChildren<RosSharp.Urdf.UrdfVisual>();

    //         foreach (RosSharp.Urdf.UrdfVisual visual in visualMeshList)
    //         {
    //             UnityEditor.Undo.RecordObject(visual.transform, "Flip URDF visuals");
    //             visual.transform.localRotation = visual.transform.localRotation * correction;
    //         }
    //     }
    //     else
    //     {
    //         RosSharp.Urdf.UrdfCollision[] collisionMeshList = gameObject.GetComponentsInChildren<RosSharp.Urdf.UrdfCollision>();

    //         foreach (RosSharp.Urdf.UrdfCollision collision in collisionMeshList)
    //         {
    //             UnityEditor.Undo.RecordObject(collision.transform, "Flip URDF collisions");
    //             collision.transform.localRotation = collision.transform.localRotation * correction;
    //         }
    //     }

    //     // Make all rotations a single undo operation
    //     UnityEditor.Undo.CollapseUndoOperations(undoGroupIndex);
    // }
}
