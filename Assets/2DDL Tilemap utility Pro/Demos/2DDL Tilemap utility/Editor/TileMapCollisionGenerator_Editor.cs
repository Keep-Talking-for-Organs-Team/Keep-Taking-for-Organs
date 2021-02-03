using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileMapCollisionGenerator))]//Custom inspector of TileMapCollisionGenerator script
public class TileMapCollisionGenerator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();//Base GUI
        #region Generate collisions button
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate collisions"))//Generate collisions button
        {
            TileMapCollisionGenerator Script = target as TileMapCollisionGenerator;
            Script.GenerateTilemapCollision(Script.Tilemap, Script.Output);
        }
        if (GUILayout.Button("Destroy collisions"))//Generate collisions button
        {
            TileMapCollisionGenerator Script = target as TileMapCollisionGenerator;
            Script.DestroyTilemapCollision(Script.Output);
        }
        GUILayout.EndHorizontal();
        #endregion
    }
}
