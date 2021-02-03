using UnityEngine;

public class GenerateCollisions : MonoBehaviour
{
    public TilemapColliderGeneratior GeneratorPro;
    public TileMapCollisionGenerator Generator;
    public GameObject OutPro;
    public GameObject Out;

    public void Generate()
    {
        foreach (Collider2D Col in Out.GetComponents<Collider2D>())
        {
            Destroy(Col);
        }
        foreach (Collider2D Col in OutPro.GetComponents<Collider2D>())
        {
            Destroy(Col);
        }
        Generator.GenerateTilemapCollision();
        GeneratorPro.GenerateColliders();
        Debug.Log("Here");
    }
}
