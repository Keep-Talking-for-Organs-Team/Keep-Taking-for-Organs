using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

[AddComponentMenu("Tilemap/Tile map collision generator")]//You may change it, hacker =)
public class TileMapCollisionGenerator : MonoBehaviour
{
    public bool GenerateOnAwake = true;//Why not?
    private void Awake()//On Awake: collision generate
    {
        if (GenerateOnAwake)//if, if, if...
            GenerateTilemapCollision();
    }
  
    public Tilemap Tilemap;
    public GameObject Output;
    public Vector3 ColliderOffset;
    public Vector3 AdditionalSize;
    /// <summary>
    /// Generate tilemap collision without args
    /// </summary>
    public void GenerateTilemapCollision()
    {
        GenerateTilemapCollision(Tilemap, Output);
    }
    /// <summary>
    /// Generate tilemap collision with args
    /// </summary>
    /// <param name="Tilemap">Tile map to generate collisions</param>
    /// <param name="CollisionsObject">Gameobject with output collisions</param>
    public void GenerateTilemapCollision(Tilemap Tilemap,GameObject CollisionsObject)
    {
        if(Tilemap == null || CollisionsObject == null)//Null Exception for you <3
        {
            Debug.Log("Tilemap or Collisions Object = null. I can't work!");
            return;
        }
        Vector3 TileAncor = Tilemap.tileAnchor;//Tile Anchor. Read doc =D.
        BoundsInt Bounds = Tilemap.cellBounds;//Bounds of tilemap

        for (int y = Bounds.y; y <= Bounds.yMax; y++)//From yMin to yMax
        {
            for (int x = Bounds.x; x <= Bounds.xMax; x++)//from xMin to xMax on y
            {             
                if (Tilemap.HasTile(new Vector3Int(x, y, 0)))//Chack tile
                {
                    Tile WorkTile = Tilemap.GetTile<Tile>(new Vector3Int(x, y, 0));//Tile to work
                    if (WorkTile.colliderType == Tile.ColliderType.None)
                        continue;

                    Vector3 CellPos = Tilemap.CellToWorld(new Vector3Int(x, y, 0));//Get collision world pos

                    if (WorkTile.sprite.GetPhysicsShapePointCount(0) == 4)//Chack collision type
                    {
                        //Box collider type(4 sides collider)
                        BoxCollider2D Collider = CollisionsObject.AddComponent<BoxCollider2D>();//Add collision
                        Collider.offset = CellPos + TileAncor / (1 / Tilemap.cellSize.x) + ColliderOffset;// + new Vector3(TileAncor.x * Tilemap.cellSize.x, TileAncor.y * Tilemap.cellSize.y);//Set collision pos
                        Collider.size = Tilemap.cellSize + AdditionalSize;//Set collision size
                    }
                    else
                    {
                        //Polygon collider type(not 4 sides collider)
                        PolygonCollider2D Collider = CollisionsObject.AddComponent<PolygonCollider2D>();//Add collision

                        List<Vector2> Vertices = new List<Vector2>();//Vertices array 
                        foreach(Vector2 a in WorkTile.sprite.vertices)
                        {
                            Vertices.Add(new Vector3(a.x * Tilemap.cellSize.x, a.y * Tilemap.cellSize.y));//Set vertices size
                        }

                        Collider.SetPath(0, Vertices.ToArray());//Set collision path
                        Collider.offset = CellPos + new Vector3(TileAncor.x * Tilemap.cellSize.x, TileAncor.y * Tilemap.cellSize.y);//Set collision pos
                    }
                }
            }
        }
    }
    public void DestroyTilemapCollision(GameObject CollisionsObject)
    {
        foreach(Collider2D a in CollisionsObject.GetComponents<Collider2D>())
        {
            GameObject.DestroyImmediate(a);
        }
    }
}
