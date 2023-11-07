
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapCreator))]
public class MapCreatorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapCreator myScript = (MapCreator)target;
        if (GUILayout.Button("Create"))
        {
            myScript.CreateMap();
        }
    }
}
