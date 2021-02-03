using System.Collections.Generic; //Collections like List<T>.
using UnityEngine; //Base to work with unity.
using UnityEngine.Tilemaps; //Base to work with tilemaps.

/// <summary>
/// Colliders generator for tilemaps
/// </summary>
public class TilemapColliderGeneratior : MonoBehaviour //Component declaration
{
    //Tilemap what now using.
    static Tilemap CurrentMap;
    //Placed rectangles on current tilemap.
    static List<RectInt> CollidedZones = new List<RectInt>();

    /// <summary>
    /// Change the current map to another after work.
    /// </summary>
    /// <param name="NewMap"></param>
    public void ChangeMap(Tilemap NewMap)
    {
        CurrentMap = NewMap;
        CurrentMap.tileAnchor = new Vector3(0.5f, 0.5f);
        CollidedZones = new List<RectInt>();
    }

    /// <summary>
    /// Find optimal rectangle to place box collider.
    /// </summary>
    /// <param name="StartPoint">Min rectangle value</param>
    /// <returns>Rectangle to place box collider</returns>
    public static Rect FindRectToMakeCollider(Vector3Int StartPoint)
    {
        //Can I start from this place?
        if (!CanBuildIn(StartPoint)) { return new Rect(); }

        Rect OutSize; //Output rectangle size.
        int SizeX = 0; //Temporary rectangle X size.
        int SizeY = 0; //Temporary rectangle Y size.

        bool FirstLine = true; //Is loop on first line
        for (int Y = StartPoint.y; Y <= CurrentMap.cellBounds.max.y; Y++)
        {
            int LapX = 0; //The x value of a row.
            for (int X = StartPoint.x; X <= CurrentMap.cellBounds.max.x; X++)
            {
                if (!CanBuildIn(new Vector3Int(X, Y, 0)))
                {
                    if (!FirstLine) //It was the first line?
                    {
                        if (LapX != SizeX) //If x value of row less than in the first row, we will break the loop.
                        {
                            OutSize = new Rect((Vector2Int)StartPoint, new Vector2(SizeX, SizeY));
                            return OutSize;
                        }
                    }
                    else
                    {
                        //Now it, not a first-line.
                        FirstLine = false;
                    }
                    break;
                } //Move by y loop.
                LapX++;
                //Set row size for rectangle.
                if (FirstLine) { SizeX = LapX; }
            }
            SizeY++;
        }
        return new Rect();
    }
    /// <summary>
    /// Can I place tile here?
    /// </summary>
    /// <param name="Where">Tile to check</param>
    static bool CanBuildIn(Vector3Int Where)
    {
        foreach (RectInt CollidedZone in CollidedZones) //Is "Where" place occupied by another collider?
        {
            foreach (Vector3Int Pos in CollidedZone.allPositionsWithin)
            {
                if (Pos == Where)
                {
                    return false;
                }
            }
        }
        if (CurrentMap.HasTile(Where)) //Have we any tile in the place where we want to create collider?
        {
            Tile TileInWhere = CurrentMap.GetTile<Tile>(Where); //Get information about tile in "Where".
            if ((TileInWhere as Tile).colliderType == Tile.ColliderType.None) //The tile shouldn't be enclosed in a collider.
            {
                return false;
            }

            return true;
        }

        return false;
    }
    /// <summary>
    /// Add collider to output object using rectangle.
    /// </summary>
    /// <param name="Size">Rectangle to place</param>
    /// <param name="Out">Gameobject to export colliders</param>
    static public BoxCollider2D AddCollider(Rect Size, Collider2D ColliderExample, GameObject Out)
    {
        return AddCollider((int)Size.x, (int)Size.y, (int)Size.width, (int)Size.height, ColliderExample, Out);
    }
    /// <summary>
    /// Add collider to output object using int sizes.
    /// </summary>
    /// <param name="X">X offset of collider</param>
    /// <param name="Y">Y offset of collider</param>
    /// <param name="XSize">X size of collider</param>
    /// <param name="YSize">Y size of collider</param>
    /// <param name="Out">Gameobject to export colliders</param>
    static public BoxCollider2D AddCollider(int X, int Y, int XSize, int YSize, Collider2D ColliderExample, GameObject Out)
    {
        //Size must be more then zero.
        if (XSize < 1 || YSize < 1) { return null; }

        //Add an occupied rectangle to "CollidedZones".
        RectInt TakenPlace = new RectInt(new Vector2Int(X, Y), new Vector2Int(XSize, YSize)); //X + 1
        CollidedZones.Add(TakenPlace);

        //Add collider to output GameObject
        Vector3 CellSize = CurrentMap.layoutGrid.cellSize;
        Vector3 ColliderSize = new Vector3(XSize * CellSize.x, YSize * CellSize.y); //Collider size by arguments.
        Vector3 HalfSizeOffset = ColliderSize / 2; //Offset to move collider to center.
        BoxCollider2D OutCollider = Out.AddComponent<BoxCollider2D>(); //Output collider component.
        OutCollider.size = ColliderSize; //Set size of output collider.
        //Calculate and set the position of the collider.
        OutCollider.offset = new Vector3(X * CellSize.x, Y * CellSize.y) + HalfSizeOffset;

        //Set material propertyes like in example collider
        if (ColliderExample != null)
        {
            OutCollider.sharedMaterial = ColliderExample.sharedMaterial;
            OutCollider.isTrigger = ColliderExample.isTrigger;
            OutCollider.usedByEffector = ColliderExample.usedByEffector;
        }
        return OutCollider;
    }

    /// <summary>
    /// Should I generate colliders in "Awake" or "Start" methods?
    /// </summary>
    public bool GenerateAtStart = true;
    //If "GenerateAtStart" is true, at what moment should I generate colliders?
    [SerializeField]
    AutoGenerationMethod GenerateAt = AutoGenerationMethod.Awake;
    /// <summary>
    /// Tilemaps to make colliders.
    /// </summary>
    public TilemapBuildInfo[] TilemapsToGenerateColliders;
    /// <summary>
    /// Gameobject to export colliders.
    /// </summary>
    public GameObject Output;

    /// <summary>
    /// If true, all colliders from tilemaps will be divided by sub GameObjects
    /// </summary>
    public bool DivideOutByTilemaps;

    /// <summary>
    /// Generate colliders for all selected tilemaps.
    /// </summary>
    public void GenerateColliders()
    {
        Output.transform.position = new Vector3(0, 0, 0);
        Output.transform.rotation = Quaternion.identity;
        Output.transform.localScale = new Vector3(1, 1, 0);

        //Export targets. It will be one or more.
        List<GameObject> ExportObjects = GetOutGameOjects();

        for (int i = 0; i < TilemapsToGenerateColliders.Length; i++)
        {
            //Getting properties
            Tilemap Map = TilemapsToGenerateColliders[i].Map;
            Collider2D ExampleCollider = TilemapsToGenerateColliders[i].ColliderExample;
            Vector2 ExtraOffset = TilemapsToGenerateColliders[i].ExtraOffset;
            Vector2 ExtraSize = TilemapsToGenerateColliders[i].ExtraSize;

            if (Map == null)
            {
                continue;
            }

            //Copy components to another object
            Component[] ComponentsToAdd = TilemapsToGenerateColliders[i].ComponentsToAdd;
            foreach (Component Component in ComponentsToAdd)
            {
                CopyComponent(Component, ExportObjects[DivideOutByTilemaps ? i : 0]);
            }

            ChangeMap(Map);
            BoundsInt Bounds = CurrentMap.cellBounds;
            foreach (Vector3Int Pos in Bounds.allPositionsWithin)
            {
                //Add collider to output object
                BoxCollider2D Collider = AddCollider(FindRectToMakeCollider(Pos), ExampleCollider, ExportObjects[DivideOutByTilemaps ? i : 0]);
                if (Collider == null) { continue; }
                //Add extra size and offset if collider exist
                Collider.offset += ExtraOffset;
                Collider.size += ExtraSize;
            }
        }

        //Set how many export objects should be
        List<GameObject> GetOutGameOjects()
        {
            List<GameObject> Out = new List<GameObject>();
            if (DivideOutByTilemaps)
            {
                for (int i = 0; i < TilemapsToGenerateColliders.Length; i++)
                {
                    GameObject SubObj = Instantiate(new GameObject(TilemapsToGenerateColliders[i].Map.name));
                    SubObj.name = SubObj.name.Replace("(Clone)", "");
                    SubObj.transform.parent = Output.transform;
                    Out.Add(SubObj);
                }
            }
            else
            {
                Out.Add(Output);
            }
            return Out;
        }
        //Copy component to another object
        Component CopyComponent(Component original, GameObject destination)
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            // Copied fields can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy;
        }
    }

    //This method calling before "Start" method. 
    void Awake()
    {
        if (GenerateAtStart && GenerateAt == AutoGenerationMethod.Awake)
        {
            GenerateColliders();
        }
    }
    //This method calling on first game frame.
    void Start()
    {
        if (GenerateAtStart && GenerateAt == AutoGenerationMethod.Start)
        {
            GenerateColliders();
        }
    }

#if UNITY_EDITOR
    //This method calling when you change properties of this component
    private void OnValidate()
    {
        foreach (TilemapBuildInfo Info in TilemapsToGenerateColliders)
        {
            //All example colliders shouldn't be calculating in Physics.
            if (Info.ColliderExample != null)
            {
                Info.ColliderExample.enabled = false;
            }
            //Set group name (you will see it in array element name, in the editor)
            if (Info.Map != null)
            {
                Info.GroupName = Info.Map.name;
            }
        }
    }
#endif

    //This enum created for popup list in editor
    public enum AutoGenerationMethod { Awake, Start }
}
[System.Serializable] //Serialization attribute
public class TilemapBuildInfo //Info for build tilemap
{
#if UNITY_EDITOR
    [HideInInspector]
    public string GroupName; //This field is required to change the name of an array cell in editor.
#endif
    public Tilemap Map; //Tilemap
    public Vector3 ExtraOffset; //Offset for tiles of tilemap
    public Vector3 ExtraSize; //Additional size for tiles of tilemap
    public Collider2D ColliderExample; //Example of a collider. I take settings from it.
    public Component[] ComponentsToAdd; //Components what will be added before generation
}