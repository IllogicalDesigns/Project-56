using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapNavagation : MonoBehaviour {
    private const byte Barrier = 0;
    public static Vector3Int WorldToGrid(Vector3 worldPos, InfluenceLayer layer) {
        Vector3 localPos = worldPos - layer.start;
        var x = Mathf.RoundToInt(localPos.x / layer.size);
        var y = Mathf.RoundToInt(localPos.y / layer.size);
        var z = Mathf.RoundToInt(localPos.z / layer.size);
        return new Vector3Int(x-1, y-1, z-1);
    }
    
    public static List<Vector3Int> GetNeighbors(InfluenceLayer layer, Vector3Int vec3Postion, int maxDist = 1, bool GetBarriers = false)
    {
        return GetNeighbors(layer.layer, vec3Postion.x, vec3Postion.y, vec3Postion.z, maxDist);
    }
    
    public static List<Vector3Int> GetNeighbors(Dictionary<Vector3Int, byte> layer, int x, int y, int z, int maxDist = 1) {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for (int i = x - maxDist; i <= x + maxDist; i++) {
            for (int j = y - maxDist; j <= y + maxDist; j++) {
                for (int k = z - maxDist; k <= z + maxDist; k++) {
                    // Skip the current cell
                    if (i == x && j == y && k == z) {
                        continue;
                    }

                    var index = new Vector3Int(i, j, k);
                    neighbors.Add(index);
                }
            }
        }

        return neighbors;
    }
    
    private static Node GetBestNode (List<Node> openList)
    {
        Node result = null;
        float currentF = float.PositiveInfinity;

        for (int i = 0; i < openList.Count; i++)
        {
            var cell = openList[i];

            if (cell.f < currentF)
            {
                currentF = cell.f;
                result = cell;
            }
        }

        return result;
    }
    
    public static int ManhattanDistance(Vector3Int p1, Vector3Int p2) {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y) + Mathf.Abs(p1.z - p2.z);
    }
    
    static int GetHeuristicOfGridPos(InfluenceLayer layer, Vector3Int gridPosition, Vector3Int goalGridPosition) {
        if (!layer.layer.TryGetValue(gridPosition, out var value))
            value = Barrier;
        
        return ManhattanDistance(gridPosition, goalGridPosition) + value;
    }
    
    private static Node[] ConstructPath (Node destination)
    {
        var path = new List<Node>() { destination };

        var current = destination;
        while (current.parent != null)
        {
            current = current.parent;
            path.Add(current);
        }

        path.Reverse();
        return path.ToArray();
    }
    
    public static Node[] FindPath(Vector3 start, Vector3 end, InfluenceLayer layer, InfluenceLayer baseLayer, Func<InfluenceLayer, Vector3Int, Vector3Int, int> getHeuristicFunc = null)
    {
        getHeuristicFunc = getHeuristicFunc ?? GetHeuristicOfGridPos;
        
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        var startGrid = WorldToGrid(start, layer);
        var endGrid = WorldToGrid(end, layer);
        
        Node startNode = new Node(startGrid, 1, getHeuristicFunc(layer, startGrid, endGrid), null);
        Node endNode = new Node(endGrid, 1, 0, null);
        
        openNodes.Add(startNode);

        int iterations = 10000;

        while (openNodes.Count > 0 && iterations > 0)
        {
            var bestNode = GetBestNode(openNodes);
            openNodes.Remove(bestNode);

            var neighbours = GetNeighbors(layer, bestNode.gridPos);
            foreach (var neighbour in neighbours) {
                if(!baseLayer.layer.ContainsKey(neighbour)) 
                    continue;
                // layer.layer.TryGetValue(neighbour, out byte value);
                // // if (!layer.layer.TryGetValue(neighbour, out var value))
                // //     value = Barrier;
                // //
                // if(value < Barrier+1) continue;
                var value = 1;
                
                var currNode = new Node(neighbour, 1, 1, null);
                
                if (currNode.gridPos == endNode.gridPos)
                {
                    currNode.parent = bestNode;
                    return ConstructPath(currNode);
                }
                
                var g = bestNode.cost + (currNode.gridPos - bestNode.gridPos).magnitude + value;
                var h =  getHeuristicFunc(layer, currNode.gridPos, endNode.gridPos);

                if (openNodes.Contains(currNode) && currNode.f < (g + h))
                    continue;
                if (closedNodes.Contains(currNode) && currNode.f < (g + h))
                    continue;

                currNode.cost = (int)g;
                currNode.heuristic = (int)h;
                currNode.parent = bestNode;

                if (!openNodes.Contains(currNode))
                    openNodes.Add(currNode);
            }
            if (!closedNodes.Contains(bestNode))
                closedNodes.Add(bestNode);

            iterations--;
        }

        return null;
    }
    
    public class Node
    {
        public Vector3Int gridPos;
        public int cost;
        public int heuristic;
        public Node parent;
        public float f { get { return cost + heuristic; } }

        public Node(Vector3Int gridPos, int cost, int heuristic, Node parent)
        {
            this.gridPos = gridPos;
            this.cost = cost;
            this.heuristic = heuristic;
            this.parent = parent;
        }
    
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Node other = (Node)obj;
            return gridPos.Equals(other.gridPos);
        }

        public override int GetHashCode()
        {
            return gridPos.GetHashCode();
        }

    }
}
