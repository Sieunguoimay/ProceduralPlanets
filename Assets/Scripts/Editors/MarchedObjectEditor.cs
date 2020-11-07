using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
[CustomEditor(typeof(MarchedObject))]
public class MarchedObjectEditor : Editor
{
    private MarchedObject marchedObject;

    private Editor occupancyGridEditor;
    private Editor noiseOccupancyEditor;
    private Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if (check.changed)
            {
                marchedObject.Initialize();
            }
        }

        DrawSettingsEditor(marchedObject.occupancyGridSettings, marchedObject.OnOccupancyGridSettingsUpdated, ref marchedObject.occupancyGridSettingsFoldout, ref occupancyGridEditor);
        DrawSettingsEditor(marchedObject.noiseOccupancySettings, marchedObject.OnNoiseOccupancySettingsUpdated, ref marchedObject.noiseOccupancySettingsFoldout, ref noiseOccupancyEditor);

        if (GUILayout.Button("Generate"))
        {
            marchedObject.Initialize();
        }
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
                if (foldout)
                {
                    //var editor = CreateEditor(settings);

                    CreateCachedEditor(settings, null, ref editor);

                    editor.OnInspectorGUI();
                    if (check.changed)
                    {
                        onSettingUpdated?.Invoke();
                    }
                }
            }
        }
    }
    private void OnEnable()
    {
        marchedObject = target as MarchedObject;
    }
}
