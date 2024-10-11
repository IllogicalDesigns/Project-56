using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Flood_Test : MonoBehaviour {
    private const byte BARRIER = 0;
    private const byte HOT = 10;
    private const byte STARTING = 1;

    [SerializeField] private Transform player;
    [SerializeField] private Transform searcher;
    [SerializeField] private int maxNewHeat = 50;
    [SerializeField] private int propagationSteps = 5;
    [SerializeField] private float propagationTime = 0.05f;
    [SerializeField] private float barrierOppositeBehindDotVal = 0.5f;

    private byte[,,] grid;
    private Vector3 start;
    private int xSize = -1;
    private int ySize = -1;
    private int zSize = -1;
    private InfluenceMap3D map3D;
    private Vector3 bestChaseGridPoint;
    private List<Vector3Int> newHeat = new List<Vector3Int>();
    private List<Vector3Int> oldHeat = new List<Vector3Int>();
    private List<Vector3Int> newBarriers = new List<Vector3Int>();
    
    private float alpha = 0.25f;

    // Start is called before the first frame update
    private void Start() {
        map3D = FindObjectOfType<InfluenceMap3D>();
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            Debug.Log("HeatWaveChasePropagation has been called");
            StartCoroutine(HeatWaveChasePropagation(player, searcher, propagationSteps, propagationTime));
        }
    }

    public IEnumerator HeatWaveChasePropagation(Transform target, Transform searcher, int PROPSTEPS = 5, float PROPTIME = 0.05f) {
        grid = map3D.GetGrid();
        
        if (grid == null || grid.GetLength(0)  == 0) {
            Debug.Log("Flood_Test:HeatWaveChasePropagation() grid is null or 0 size, returning");
            yield break;
        }

        Vector3Int size = map3D.GetSizeVector();
        xSize = size.x;
        ySize = size.y;
        zSize = size.z;

        start = map3D.start;

        var initTargetPosition = map3D.WorldToGrid(target.position);
        var initDirection = target.position - searcher.position;

        newBarriers.Clear();
        newHeat.Clear();
        oldHeat.Clear();

        if(grid[initTargetPosition.x, initTargetPosition.y, initTargetPosition.z] == BARRIER) {
            Debug.Log("It appears the the chased is inside of a barrier, this will break the flood");
        }

        Debug.DrawRay(map3D.GridToWorld(initTargetPosition), Vector3.up * 5f, Color.cyan, 10f);

        HeatUpInitLocation(grid, initTargetPosition, newHeat, oldHeat);
        GenerateInitBarrier(grid, newBarriers, initTargetPosition, initDirection, target.position);

        for (int i = 0; i < PROPSTEPS; i++) {
            if (newHeat.Count > maxNewHeat) break;

            PropagateHeat(grid, newHeat, oldHeat);
            CoolOffOldHeat(grid, oldHeat);
            PropagateBarriers(grid, newBarriers);

            yield return new WaitForSeconds(PROPTIME);
        }

        bestChaseGridPoint = FindCentroidOrClosest(oldHeat);

        //Red represents the best location to chase to
        Debug.DrawRay(map3D.GridToWorld(bestChaseGridPoint), Vector3.up * 100, Color.red, 10f);
    }

    private void GenerateInitBarrier(byte[,,] grid, List<Vector3Int> newBarriers, Vector3Int initPosition, Vector3 initDirection, Vector3 targetPos) {
        List<Vector3Int> neighbors = GetNeighbors(grid, initPosition.x, initPosition.y, initPosition.z);
        for (int i = 0; i < neighbors.Count; i++) {
            SetOppositeToBarrier(grid, neighbors[i].x, neighbors[i].y, neighbors[i].z, targetPos, newBarriers);
        }
    }

    private void SetOppositeToBarrier(byte[,,] grid, int x, int y, int z, Vector3 targetPos, List<Vector3Int> newBarriers) {
        if (grid[x, y, z] != STARTING) return;

        // Calculate the world position of the current cell
        Vector3 cellPos = map3D.GridToWorld(x, y, z);

        // Calculate the vector from the current cell to the given world position
        Vector3 toCell = -(cellPos - targetPos);
        Vector3 toUs = -(searcher.position - targetPos);

        // Calculate the dot product between the vector and the opposite direction
        float dotProduct = Vector3.Dot(toCell.normalized, toUs.normalized);

        // Check if the dot product is above a certain threshold (e.g., 0.8f)
        if (dotProduct > barrierOppositeBehindDotVal) {
            // Set the cell to 0, orig impl was -1
            grid[x, y, z] = BARRIER;
            newBarriers.Add(new Vector3Int(x, y, z));
        }
    }

    public static Vector3Int FindCentroidOrClosest(List<Vector3Int> vectors) {
        int count = vectors.Count;
        Vector3Int sum = Vector3Int.zero;

        foreach (Vector3Int vector in vectors) {
            sum += vector;
        }

        Vector3Int centroid = sum / count;

        if (vectors.Contains(centroid)) {
            return centroid;
        }

        Vector3Int closestVector = vectors[0];
        float closestDistance = Vector3Int.Distance(centroid, closestVector);

        for (int i = 1; i < count; i++) {
            Vector3Int currentVector = vectors[i];
            float currentDistance = Vector3Int.Distance(centroid, currentVector);

            if (currentDistance < closestDistance) {
                closestVector = currentVector;
                closestDistance = currentDistance;
            }
        }

        return closestVector;
    }

    public int ManhattanDistance(Vector3Int p1, Vector3Int p2) {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y) + Mathf.Abs(p1.z - p2.z);
    }

    private void HeatUpInitLocation(byte[,,] grid, Vector3Int initGridLocation, List<Vector3Int> newHeat, List<Vector3Int> oldHeat) {
        grid[initGridLocation.x, initGridLocation.y, initGridLocation.z] = HOT;
        newHeat.Add(new Vector3Int(initGridLocation.x, initGridLocation.y, initGridLocation.z));
        oldHeat.Add(new Vector3Int(initGridLocation.x, initGridLocation.y, initGridLocation.z));
    }

    private void PropagateHeat(byte[,,] grid, List<Vector3Int> newHeat, List<Vector3Int> oldHeat) {
        List<Vector3Int> tempHeat = new List<Vector3Int>(newHeat);
        newHeat.Clear();

        foreach (var item in tempHeat) {
            var neighbors = GetNeighbors(grid, item.x, item.y, item.z);
            foreach (var subItem in neighbors) {
                var val = grid[subItem.x, subItem.y, subItem.z];

                if (val == BARRIER) continue; //converted from -1 in orig impl
                if (val != BARRIER + 1) continue; //converted from 0 in orig impl

                grid[subItem.x, subItem.y, subItem.z] = 10;
                newHeat.Add(new Vector3Int(subItem.x, subItem.y, subItem.z));
                oldHeat.Add(new Vector3Int(subItem.x, subItem.y, subItem.z));
            }
        }
    }

    private void CoolOffOldHeat(byte[,,] grid, List<Vector3Int> oldHeat) {
        for (int i = 0; i < oldHeat.Count; i++) {
            var x = oldHeat[i].x;
            var y = oldHeat[i].y;
            var z = oldHeat[i].z;
            if (grid[x, y, z] > STARTING + 1) {
                grid[x, y, z] -= 1;
            }
            else {
                // If the value is 1, remove the point from oldHeat.  Orig impl used 0 here
                grid[x, y, z] = STARTING;
                oldHeat.RemoveAt(i);
                i--;  // Decrement i to account for the removed element
            }
        }
    }

    private void PropagateBarriers(byte[,,] grid, List<Vector3Int> newBarriers) {
        List<Vector3Int> tempBarrier = new List<Vector3Int>(newBarriers);
        newBarriers.Clear();

        foreach (var item in tempBarrier) {
            var neighbors = GetNeighbors(grid, item.x, item.y, item.z);
            foreach (var subItem in neighbors) {
                var val = grid[subItem.x, subItem.y, subItem.z];

                if (val == BARRIER) continue;

                if (val == STARTING) {
                    SetPointToBarrier(grid, newBarriers, subItem);
                }
            }
        }

        static void SetPointToBarrier(byte[,,] grid, List<Vector3Int> newBarriers, Vector3Int subItem) {
            grid[subItem.x, subItem.y, subItem.z] = BARRIER;
            newBarriers.Add(new Vector3Int(subItem.x, subItem.y, subItem.z));
        }
    }

    public List<Vector3Int> GetNeighbors(byte[,,] grid, Vector3Int vec3Postion, int maxDist = 1)
    {
        return GetNeighbors(grid, vec3Postion.x, vec3Postion.y, vec3Postion.z, maxDist);
    }
    
    public List<Vector3Int> GetNeighbors(byte[,,] grid, int x, int y, int z, int maxDist = 1) {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for (int i = x - maxDist; i <= x + maxDist; i++) {
            for (int j = y - maxDist; j <= y + maxDist; j++) {
                for (int k = z - maxDist; k <= z + maxDist; k++) {
                    // Skip the current cell
                    if (i == x && j == y && k == z) {
                        continue;
                    }

                    // Check if the cell is inside the grid
                    if (i >= 0 && i < grid.GetLength(0) && j >= 0 && j < grid.GetLength(1) && k >= 0 && k < grid.GetLength(2)) {
                        neighbors.Add(new Vector3Int(i, j, k));
                    }
                }
            }
        }

        return neighbors;
    }

    public Vector3 GridToWorld(int x, int y, int z) {
        return start + (new Vector3(x, y, z) * map3D.size);
    }

    private Color ByteToColor(byte i) {
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
                return Color.clear;

            default:
                return new Color(1f, 0f, 0f, alpha);
        }
    }

    private void OnDrawGizmosSelected() {
        if (player) {
            Gizmos.DrawLine(player.position, player.position + player.forward);
        }

        if (grid == null || grid.GetLength(0) == 0) return;
        
        const float XYSIZEREDUCTION = 0.2f;
        const float ZHEIGHT = 0.1f;
        
        var halfExtent = map3D.size - XYSIZEREDUCTION;

        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    Vector3 worldPostion = GridToWorld(x, y, z);
                    Gizmos.color = ByteToColor(grid[x, y, z]);
                    Gizmos.DrawCube(worldPostion, new Vector3(halfExtent, ZHEIGHT, halfExtent));
                }
            }
        }
    }
}
