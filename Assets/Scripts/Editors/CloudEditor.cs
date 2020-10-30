#if UNITY_EDITOR

using UnityEditor;


[CustomEditor(typeof(Cloud))]
public class CloudEditor : Editor
{
    Cloud cloud => target as Cloud;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            CreateEditor(cloud.settings).OnInspectorGUI();

            if (check.changed)
            {
                cloud.UpdateFromEditor();
            }
        }
    }
}

#endif //UNITY_EDITOR