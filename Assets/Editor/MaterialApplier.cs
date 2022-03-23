using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PhysicsBlock)), CanEditMultipleObjects]
public class MaterialApplier : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Apply Material"))
        {
            try
            {
                ((PhysicsBlock) target).ApplyMaterial(false);
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error!", ex.Message, "Oops");
            }
        }

    }
}
