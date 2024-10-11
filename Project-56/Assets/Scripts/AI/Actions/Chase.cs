using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : GAction {
    [SerializeField] float stoppingDist = 5f;
    [SerializeField] Transform player;

    private const byte BARRIER = 0;
    private const byte HOT = 10;
    private const byte STARTING = 1;

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

    void Start() {
        map3D = FindAnyObjectByType<InfluenceMap3D>();
        player = GameManager.player.transform;
        searcher = transform;
        AddEffects(Cat.chaseGoal, null);
    }

    // Update is called once per frame
    private void Update() {
        //if (Input.GetKeyDown(KeyCode.I)) {
        //    Debug.Log("HeatWaveChasePropagation has been called");
        //    StartCoroutine(HeatWaveChasePropagation(player, searcher, propagationSteps, propagationTime));
        //}
    }

    public IEnumerator HeatWaveChasePropagation(Transform target, Transform searcher, int PROPSTEPS = 5, float PROPTIME = 0.05f) {
        Debug.Log("HeatWaveChasePropagation has been called");
        grid = map3D.GetGrid();

        if (grid == null || grid.GetLength(0) == 0) {
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

        Debug.Log("map3D.GetSizeVector() " + size + " start " + start + " initTargetPosition " + initTargetPosition + " initDirection " + initDirection);

        newBarriers.Clear();
        newHeat.Clear();
        oldHeat.Clear();

        if (map3D.IsWithinBounds(initTargetPosition)) {
            Debug.Log("map3D.IsWithinBounds(initTargetPosition) == true");
        } else {
            Debug.Log("map3D.IsWithinBounds(initTargetPosition) == false");
        }

        //if (grid[initTargetPosition.x, initTargetPosition.y, initTargetPosition.z] == BARRIER) {
        //    initTargetPosition = FindClosestNonBarrier(initTargetPosition, grid, BARRIER);
        //    Debug.Log("We were in a wall, so new initTargetPos " + initTargetPosition);
        //}

        //debug the initTargetPosition
        //Debug.DrawRay(map3D.GridToWorld(initTargetPosition), Vector3.up * 5f, Color.cyan, 10f);

        HeatUpInitLocation(grid, initTargetPosition, newHeat, oldHeat);
        GenerateInitBarrier(grid, newBarriers, initTargetPosition, initDirection, target.position);

        for (int i = 0; i < PROPSTEPS; i++) {
            if (newHeat.Count > maxNewHeat) {
                Debug.Log("newHeat.Count > maxNewHeat breaking");
                break;
            }
            //Debug.Log("Prop, step " + i);

            PropagateHeat(grid, newHeat, oldHeat);
            CoolOffOldHeat(grid, oldHeat);
            PropagateBarriers(grid, newBarriers);


            //Debug.Break();

            yield return new WaitForEndOfFrame();
        }

        bestChaseGridPoint = FindCentroidOrClosest(oldHeat);

        //Red represents the best location to chase to
        Debug.DrawRay(map3D.GridToWorld(bestChaseGridPoint), Vector3.up * 100, Color.red, 10f);
        Debug.DrawLine(map3D.GridToWorld(bestChaseGridPoint), map3D.GridToWorld(initTargetPosition), Color.magenta, 10f);
        Debug.Log("HeatWaveChasePropagation is now Finished");
        //Debug.Break();
    }

    private static readonly Vector3Int[] directions = {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), // Move in x-axis
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), // Move in y-axis
        new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)  // Move in z-axis
    };

    // Helper method to check if the position is within grid bounds
    private static bool IsValidPosition(Vector3Int pos, byte[,,] grid) {
        return pos.x >= 0 && pos.x < grid.GetLength(0) &&
               pos.y >= 0 && pos.y < grid.GetLength(1) &&
               pos.z >= 0 && pos.z < grid.GetLength(2);
    }

    public static Vector3Int FindClosestNonBarrier(Vector3Int initTargetPosition, byte[,,] grid, int BARRIER) {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        // Start with the initial target position
        queue.Enqueue(initTargetPosition);
        visited.Add(initTargetPosition);

        // BFS search
        while (queue.Count > 0) {
            Vector3Int current = queue.Dequeue();

            // If the current position is not a barrier, return it as the closest point
            if (grid[current.x, current.y, current.z] != BARRIER) {
                return current;
            }

            // Explore all possible directions (6 directions in 3D space)
            foreach (var direction in directions) {
                Vector3Int neighbor = current + direction;

                // Check if the neighbor is within bounds and hasn't been visited
                if (IsValidPosition(neighbor, grid) && !visited.Contains(neighbor)) {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        // If no non-barrier point is found, return the initial target position or a default value
        Debug.Log("No non-barrier point found.");
        return initTargetPosition; // or return another fallback value
    }


    private void GenerateInitBarrier(byte[,,] grid, List<Vector3Int> newBarriers, Vector3Int initPosition, Vector3 initDirection, Vector3 targetPos) {
        List<Vector3Int> neighbors = GetNeighbors(grid, initPosition.x, initPosition.y, initPosition.z);
        Debug.Log("GenerateInitBarrier Barrier.count = " + neighbors.Count);
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
        Debug.Log("Heating grid pos " + initGridLocation);
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

    public List<Vector3Int> GetNeighbors(byte[,,] grid, Vector3Int vec3Postion, int maxDist = 1) {
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

    private void OnDrawGizmos() {
        if (player) {
            Gizmos.DrawLine(player.position, player.position + player.forward);
        }

        if (grid == null || grid.GetLength(0) == 0) return;

        const float XYSIZEREDUCTION = 0.2f;
        const float ZHEIGHT = 0.1f;

        var halfExtent = map3D.size - XYSIZEREDUCTION;

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(map3D.GridToWorld(bestChaseGridPoint), 0.5F);

        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    if (grid[x, y, z] == STARTING) continue;
                    Vector3 worldPostion = GridToWorld(x, y, z);
                    Gizmos.color = ByteToColor(grid[x, y, z]);
                    Gizmos.DrawCube(worldPostion, new Vector3(halfExtent, ZHEIGHT, halfExtent));
                }
            }
        }
    }

    public override IEnumerator Perform() {
        gAgent.RemoveGoal(Cat.attackGoal);
        gAgent.RemoveGoal(Cat.investigateGoal);
        gameObject.SendMessage("SetBehaviorState", Cat.CatBehavior.Chase);

        yield return new WaitForSeconds(0.5f);

        bestChaseGridPoint = map3D.WorldToGrid(player.position);

        yield return StartCoroutine(HeatWaveChasePropagation(player, searcher, propagationSteps, propagationTime));

        yield return gAgent.Goto(map3D.GridToWorld(bestChaseGridPoint), stoppingDist);
        //gAgent.agent.SetDestination(map3D.GridToWorld(bestChaseGridPoint));
        //yield return new WaitForSeconds(5f);

        CompletedAction();
    }
}


