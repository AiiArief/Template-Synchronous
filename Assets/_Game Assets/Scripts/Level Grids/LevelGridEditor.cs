using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGrid))]
class LevelGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGrid grid = (LevelGrid)target;
        if(GUILayout.Button("Destroy Nodes"))
        {
            grid.EditorDestroyAllNodes();
        }

        if(GUILayout.Button("Generate Nodes"))
        {
            grid.EditorGenerateAllGridNodes();
        }
    }
}
