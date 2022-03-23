using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PhysicsApplier))]
public class MassMaterialApply : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Apply Material to children"))
        {
            foreach (PhysicsBlock child in ((PhysicsApplier) target).GetComponentsInChildren<PhysicsBlock>())
            {
                try
                {
                    child.ApplyMaterial(((PhysicsApplier) target).changeLook);
                    EditorUtility.SetDirty(child);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }
    }
}
