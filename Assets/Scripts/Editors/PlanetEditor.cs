using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;

    Editor shapeEditor;
    Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }
        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref planet.colorSettingsFoldout, ref colorEditor);

        if (GUILayout.Button("temp"))
        {
            var temp = planet.colorSettings.biomeColorSettings.biomes;
            planet.colorSettings.biomeColorSettings.biomes = new ColorSettings.BiomeColorSettings.Biome[3];
            temp.CopyTo(planet.colorSettings.biomeColorSettings.biomes,1);
            planet.colorSettings.biomeColorSettings.biomes[0] = new ColorSettings.BiomeColorSettings.Biome()
            {
                gradient = temp[1].gradient,
                tint = temp[1].tint,
                startHeight = temp[1].startHeight,
                tintPercent = temp[1].tintPercent,
        };
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
        planet = (Planet)target;
    }
}
