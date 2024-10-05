using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class InfluenceLayer : MonoBehaviour
{
    public const byte BARRIER = 0;
    public const byte STARTING = 1;
    private const float debugAlpha = 0.25f;
    [SerializeField] private bool showBarriers = false;
    
    public Dictionary<Vector3Int, byte> layer = new Dictionary<Vector3Int, byte>();
    [SerializeField] private string layerName;
    public float size = 2f;
    public Vector3 offset;
    
    public int xSize = -1;
    public int ySize = -1;
    public int zSize = -1;
    public Vector3 start;
    
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

    public Dictionary<Vector3Int, byte> GetLayer()
    {
        return layer;
    }
    
    public List<Vector3Int> GetNeighbors(Dictionary<Vector3Int, byte> layer, int x, int y, int z, int maxDist = 1) {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for (int i = x - maxDist; i <= x + maxDist; i++) {
            for (int j = y - maxDist; j <= y + maxDist; j++) {
                for (int k = z - maxDist; k <= z + maxDist; k++) {
                    // Skip the current cell
                    if (i == x && j == y && k == z) {
                        continue;
                    }
                    
                    var index = new Vector3Int(i, j, k);
                    if(layer.ContainsKey(index))neighbors.Add(index);
                }
            }
        }

        return neighbors;
    }
    
    public int ManhattanDistance(Vector3Int p1, Vector3Int p2) {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y) + Mathf.Abs(p1.z - p2.z);
    }

    public Vector3Int WorldToGrid(Vector3 worldPos) {
        Vector3 localPos = worldPos - start;
        var x = Mathf.RoundToInt(localPos.x / size);
        var y = Mathf.RoundToInt(localPos.y / size);
        var z = Mathf.RoundToInt(localPos.z / size);
        return new Vector3Int(x-1, y-1, z-1);
    }

    #region SparseArray
    public Dictionary<Vector3Int, byte> ToSparseArray(byte[,,] array)
    {
        var sparseArray = new Dictionary<Vector3Int, byte>();

        int size1 = array.GetLength(0);
        int size2 = array.GetLength(1);
        int size3 = array.GetLength(2);

        for (int x = 0; x < size1; x++)
        {
            for (int y = 0; y < size2; y++)
            {
                for (int z = 0; z < size3; z++)
                {
                    byte value = array[x, y, z];
                    if (value != 0)
                    {
                        Vector3Int key = new Vector3Int(x, y, z);
                        sparseArray[key] = value;
                    }
                }
            }
        }

        return sparseArray;
    }
    #endregion

    #region SaveLoad

    public void SaveLayer()
    {
        if(layer == null) return;
        
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + layerName +".dat";

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, layer);
        stream.Close();
    }
    
    public Dictionary<Vector3Int, byte> LoadLayer()
    {
        string path = Application.persistentDataPath + "/" + layerName +".dat";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Dictionary<Vector3Int, byte> data = formatter.Deserialize(stream) as Dictionary<Vector3Int, byte>;

            stream.Close();

            return data;
        } else
        {
            Debug.LogError("Error: Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region DrawGizmos

    private void OnDrawGizmosSelected()
    {
        DrawInfluenceLayer();
    }

    public void DrawInfluenceLayer()
    {
        if (layer == null || layer.Count == 0) return;
        
        start = GetStart();
        var size = GetSizeVector();
        xSize = size.x;
        ySize = size.y;
        zSize = size.z;
        
        const float XYSIZEREDUCTION = 0.2f;
        const float ZHEIGHT = 0.1f;
        
        var halfExtent = size;
        
        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    var gridPosition = new Vector3Int(x, y, z);
                    if (!layer.ContainsKey(gridPosition))
                    {
                        Vector3 position = GridToWorldPosition(gridPosition);
                        Gizmos.color = ByteToColor(BARRIER);
                        if(Gizmos.color == Color.clear) continue;
                        
                        Gizmos.DrawWireCube(position, new Vector3(0.25f, 0.25f, 0.25f));
                    }
                    else
                    {

                        Vector3 worldPostion = GridToWorldPosition(gridPosition);
                        Gizmos.color = ByteToColor(layer[gridPosition]);
                        if(Gizmos.color == Color.clear) continue;
                        
                        Gizmos.DrawCube(worldPostion, new Vector3(1 - 0.1f, ZHEIGHT, 1 - 0.1f));
                    }
                }
            }
        }
    }

    public virtual Color ByteToColor(byte i) {
        switch (i) {
            case STARTING + 16:
                return new Color(1f, 0.90f, 0.9f, debugAlpha);
            case STARTING + 15:
                return new Color(1f, 0.80f, 0.8f, debugAlpha);
            case STARTING + 14:
                return new Color(1f, 0.70f, 0.7f, debugAlpha);
            case STARTING + 13:
                return new Color(1f, 0.60f, 0.6f, debugAlpha);
            case STARTING + 12:
                return new Color(1f, 0.50f, 0.5f, debugAlpha);
            case STARTING + 11:
                return new Color(1f, 0.40f, 0.4f, debugAlpha);
            case STARTING + 10:
                return new Color(1f, 0.30f, 0.3f, debugAlpha);
            case STARTING + 9:
                return new Color(1f, 0.20f, 0.2f, debugAlpha); // Red
            case STARTING + 8:
                return new Color(0.9f, 0.1f, 0.1f, debugAlpha); // Red
            case STARTING + 7:
                return new Color(0.8f, 0f, 0f, debugAlpha); // Red
            case STARTING + 6:
                return new Color(0.70f, 0f, 0f, debugAlpha); // Red
            case STARTING + 5:
                return new Color(0.60f, 0f, 0f, debugAlpha); // Red
            case STARTING + 4:
                return new Color(0.50f, 0f, 0.1f, debugAlpha); // Red
            case STARTING + 3:
                return new Color(0.40f, 0f, 0.2f, debugAlpha); // Red
            case STARTING + 2:
                return new Color(0.30f, 0f, 0.3f, debugAlpha); // Red
            case STARTING + 1:
                return new Color(0.20f, 0f, 0.4f, debugAlpha); // Red
            case STARTING:
                return new Color(0f, 0f, 0f, debugAlpha);

            case BARRIER:
                if(showBarriers)
                    return new Color(1f, 0f, 1f, debugAlpha);
                else
                    return Color.clear;

            default:
                return new Color(1f, 1f, 1f, debugAlpha);
        }
    }
    

    #endregion
}
