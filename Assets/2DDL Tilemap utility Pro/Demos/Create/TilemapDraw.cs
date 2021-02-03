using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDraw : MonoBehaviour
{
    public Tile TileToDraw;
    public Tilemap TilemapToDraw;
    public GameObject Point;
    Camera Cam;
    private void Awake()
    {
        Cam = Camera.main;
    }
    private void Update()
    {
        Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int TilePos = new Vector3Int((int)MousePos.x, (int)MousePos.y, 0);
        if (Point != null) { Point.transform.position = TilemapToDraw.CellToWorld(TilePos) + TilemapToDraw.tileAnchor; }
        if (Input.GetMouseButton(1))
        {
            TilemapToDraw.SetTile(TilePos, TileToDraw);
        }
        if (Input.GetMouseButton(2))
        {
            Cam.transform.position += -new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
        Cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
    }
}