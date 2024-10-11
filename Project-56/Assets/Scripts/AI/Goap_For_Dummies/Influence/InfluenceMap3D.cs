using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Serialization;

public class InfluenceMap3D : MonoBehaviour {

    [Header("Grid settings")]
    [SerializeField] private Vector3 offset;
    [SerializeField] public MeshRenderer boundsHint;

    [SerializeField] public float size = 2f;
    private byte[,,] grid;
    public Vector3 start;

    private const byte ARBITRARYHOTVALUE = 10;
    private const byte BARRIER = 0;
    private const byte STARTING = 1;

    private int xSize = -1;
    private int ySize = -1;
    private int zSize = -1;

    //[Header("Ground check")]
    private float groundCheckLength = 1f;
    private Vector3 groundCheckOffset = Vector3.up;
    private bool GroundCheck = false;

    [Header("Nav check")]
    [SerializeField] private bool navCheck = true;
    [SerializeField] private float navMeshSampleAllowance = 0.2f;
    [FormerlySerializedAs("dropNav")]
    [Space]
    [SerializeField] private bool dropNavagationPoints = false;
    [SerializeField] private Vector3 dropNavOffset = new Vector3(0f, 0.25f, 0f);
    [SerializeField] private float dropNavLength = 1.6f;
    [SerializeField] private LayerMask layerMask = 1 << 0;

    [Header("Debug")]
    [SerializeField] private bool drawDebug;
    [SerializeField] private bool showBarriers;
    
    private float alpha = 0.25f;

    private void Start()
    {
        grid = LoadData();
        
        if(grid==null) 
            Debug.Log(("No grid data found!!!"));
        else
        {
            
            Debug.Log("grid found of [" + grid.GetLength(0) + " " + grid.GetLength(1) + " " + grid.GetLength(2) + "]");
        }
    }

    public byte[,,] GetGrid() {
        if (grid == null) BuildGrid();

        byte[,,] copy = (byte[,,])grid.Clone();
        return copy;
    }

    public bool IsWithinBounds(Vector3Int gridPos) {
        return gridPos.x >= 0 && gridPos.x < grid.GetLength(0) &&
               gridPos.y >= 0 && gridPos.y < grid.GetLength(1) &&
               gridPos.z >= 0 && gridPos.z < grid.GetLength(2);
    }

    public Vector3 GetStart() {
        return start;
    }

    public Vector3Int GetSizeVector() {
        return new Vector3Int(xSize, ySize, zSize);
    }

    public Vector3 GridToWorldPosition(int x, int y, int z) {
        return GridToWorldPosition(new Vector3Int(x,y,z));
    }
    
    public Vector3 GridToWorldPosition(Vector3Int vec) {
        return start + offset + (new Vector3(vec.x, vec.y, vec.z) * size);
    }

    public Vector3 GridToWorld(int x, int y, int z) {
        return GridToWorld(new Vector3(x,y,z));
    }

    public Vector3 GridToWorld(Vector3 vector) {
        return start + (vector * size);
    }
    

    public Vector3Int WorldToGrid(Vector3 worldPos) {
        Vector3 localPos = worldPos - start;
        var x = Mathf.RoundToInt(localPos.x / size);
        var y = Mathf.RoundToInt(localPos.y / size);
        var z = Mathf.RoundToInt(localPos.z / size);

        x = Mathf.Clamp(x, 1, grid.GetLength(0));
        y = Mathf.Clamp(y, 1, grid.GetLength(1));
        z = Mathf.Clamp(z, 1, grid.GetLength(2));

        return new Vector3Int(x-1, y-1, z-1);
    }

    #region GridCreation

    public void BuildGrid() {
        if (!HasBoundsHint()) return;
        
        InitializeGridSizeBasedOnBounds();
        InitializeGridStartBasedOnBounds();

        grid = new byte[xSize, ySize, zSize];

        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    Vector3 location = GridToWorldPosition(x, y, z);

                    if (navCheck) {
                        location = DropNavPoints(location);
                        bool mesh = HasNavMesh(location);

                        if (!mesh) {
                            grid[x, y, z] = BARRIER;
                            continue;
                        }
                    }

                    // if (GroundCheck) {
                    //     bool ground = HasGround(location);
                    //     if (!ground) {
                    //         grid[x, y, z] = BARRIER;
                    //         continue;
                    //     }
                    // }

                    grid[x, y, z] = STARTING;
                }
            }
        }

        SaveGrid();
    }
    
    private Vector3 DropNavPoints(Vector3 location) {
        if (!dropNavagationPoints) return location;
        
        RaycastHit rayHit;
        Debug.DrawRay(location + dropNavOffset, Vector3.down * dropNavLength, Color.yellow, 1);
        if (Physics.Raycast(location + dropNavOffset, Vector3.down, out rayHit, dropNavLength, layerMask)) {
            location.y = rayHit.point.y;
        }
        

        return location;
    }

    public void ClearGrid() {
        grid = new byte[0, 0, 0];
    }

    private bool HasGround(Vector3 location) {
        RaycastHit rayHit;
        if (Physics.Raycast(location + groundCheckOffset, Vector3.down, out rayHit, groundCheckLength, layerMask)) {
            Debug.DrawRay(location + groundCheckOffset, Vector3.down * groundCheckLength, Color.cyan, 10f);
            return true;
        }

        return false;
    }

    private bool HasNavMesh(Vector3 location) {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(location, out hit, navMeshSampleAllowance, -1)) {
            // Debug.DrawRay(hit.position, Vector3.up, Color.green, 20f);
            return true;
        }
        else {
            return false;
        }
    }

    private bool HasBoundsHint() {
        if (boundsHint == null)
        {
            boundsHint = gameObject.GetComponent<MeshRenderer>();
        }
        
        if (boundsHint == null) {
            Debug.Log("InfluenceMap3D Collider boundsHint is null");
            return false;
        }
        else {
            return true;
        }
    }

    private void InitializeGridStartBasedOnBounds() {
        start = boundsHint.bounds.center - (boundsHint.bounds.size * 0.5f);
    }

    private void InitializeGridSizeBasedOnBounds() {
        xSize = Mathf.RoundToInt(boundsHint.bounds.size.x / size);
        ySize = Mathf.RoundToInt(boundsHint.bounds.size.y / size);
        zSize = Mathf.RoundToInt(boundsHint.bounds.size.z / size);
    }

    #endregion

    #region SparseArray

    static Dictionary<string, byte> ToSparseArray(byte[,,] array)
    {
        var sparseArray = new Dictionary<string, byte>();

        int size1 = array.GetLength(0);
        int size2 = array.GetLength(1);
        int size3 = array.GetLength(2);

        for (int i = 0; i < size1; i++)
        {
            for (int j = 0; j < size2; j++)
            {
                for (int k = 0; k < size3; k++)
                {
                    byte value = array[i, j, k];
                    if (value != 0)
                    {
                        string key = $"{i},{j},{k}";
                        sparseArray[key] = value;
                    }
                }
            }
        }

        return sparseArray;
    }

    static byte GetSparseValue(Dictionary<string, byte> sparseArray, int i, int j, int k)
    {
        string key = $"{i},{j},{k}";
        if (sparseArray.TryGetValue(key, out byte value))
        {
            return value;
        }

        return 0;
    }

    static void SetSparseValue(Dictionary<string, byte> sparseArray, int i, int j, int k, byte value)
    {
        string key = $"{i},{j},{k}";
        sparseArray[key] = value;
    }

    #endregion
    
    #region SaveLoad

    public void SaveGrid()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/map.dat";

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, grid);
        stream.Close();
    }
    
    public static byte[,,] LoadData()
    {
        string path = Application.persistentDataPath + "/map.dat";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            byte[,,] data = formatter.Deserialize(stream) as byte[,,];

            stream.Close();

            return data;
        } else
        {
            Debug.LogError("Error: Save file not found in " + path);
            return null;
        }
    }

    public static byte[,,] LoadByteArray(string filePath) {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.Open);

        byte[,,] loadedArray = (byte[,,])formatter.Deserialize(fileStream);
        fileStream.Close();

        return loadedArray;
    }

    #endregion

    #region DrawGizmos

    private void OnDrawGizmos()
    {
        if(drawDebug)
            DrawInfluenceMap();
    }

    public void DrawInfluenceMap()
    {
        if (!HasBoundsHint()) return;
        InitializeGridSizeBasedOnBounds();
        InitializeGridStartBasedOnBounds();
        
        if(grid == null) return;

        if (grid.GetLength(0) == 0)
        {
            grid = LoadData();
            return;
        }

        const float XYSIZEREDUCTION = 0.2f;
        const float ZHEIGHT = 0.1f;

        var halfExtent = size - XYSIZEREDUCTION;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    if (grid[x, y, z] == STARTING) continue;
                    Vector3 worldPostion = GridToWorldPosition(x, y, z);
                    Gizmos.color = ByteToColor(grid[x, y, z]);
                    Gizmos.DrawCube(worldPostion, new Vector3(halfExtent, ZHEIGHT, halfExtent));
                }
            }
        }
    }

    public Color ByteToColor(byte i) {
        switch (i) {
            case STARTING + 9:
                return new Color(1f, 0.20f, 0.2f, alpha); // Red

            case STARTING + 8:
                return new Color(0.9f, 0.1f, 0.1f, alpha); // Red

            case STARTING + 7:
                return new Color(0.8f, 0f, 0f, alpha); // Red

            case STARTING + 6:
                return new Color(0.70f, 0f, 0f, alpha); // Red

            case STARTING + 5:
                return new Color(0.60f, 0f, 0f, alpha); // Red

            case STARTING + 4:
                return new Color(0.50f, 0f, 0.1f, alpha); // Red

            case STARTING + 3:
                return new Color(0.40f, 0f, 0.2f, alpha); // Red

            case STARTING + 2:
                return new Color(0.30f, 0f, 0.3f, alpha); // Red

            case STARTING + 1:
                return new Color(0.20f, 0f, 0.4f, alpha); // Red

            case STARTING:
                return new Color(0f, 0f, 0f, alpha);

            case BARRIER:
                if(showBarriers)
                    return new Color(1f, 0f, 1f, alpha);
                else
                    return Color.clear;

            default:
                return new Color(1f, 0f, 0f, alpha);
        }
    }
    

    #endregion
}
