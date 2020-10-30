using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(Atmosphere))]
public class AtmosphereEditor : Editor
{
    private Atmosphere atmosphere { get { return (Atmosphere)target; } }

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            CreateEditor(atmosphere.settings).OnInspectorGUI();

            if (check.changed)
            {
            }
        }
    }
}
#endif