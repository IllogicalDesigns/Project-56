using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupLayer : InfluenceLayer {
    [Space] [SerializeField] public BaseLayer baseLayer;
    [SerializeField] public Transform[] group;
    [SerializeField] public int maxRange = 20;

    public Vector3Int startPos;
    public Coroutine coroutine;

    private void Start() {
        BuildLayer();
    }

    public virtual void FixedUpdate() {
        layer = new Dictionary<Vector3Int, byte>();
        if(coroutine == null) coroutine = StartCoroutine(CalculateGroupLayer());
    }

    IEnumerator CalculateGroupLayer() {
        foreach (var item in group) {
            var itemGrid = WorldToGrid(item.position);

            if (!baseLayer.layer.ContainsKey(itemGrid))
                itemGrid = GetCloseWalkableNode(itemGrid);

            var localLayer = PerformFloodFill(itemGrid);
            CombineLayers(localLayer);
        }
        
        yield return new WaitForFixedUpdate();

        coroutine = null;
    }

    public virtual void CombineLayers(Dictionary<Vector3Int, byte> localLayer) {
        foreach (var local in localLayer) {
            var added = layer.TryAdd(local.Key, local.Value);

            if (!added) {
                layer[local.Key] += local.Value;
            }
        }
    }

    public virtual Dictionary<Vector3Int, byte> PerformFloodFill(Vector3Int startPosition) {
        startPos = startPosition;

        var localLayer = new Dictionary<Vector3Int, byte>();
        var queue = new Queue<Vector3Int>();
        queue.Enqueue(startPosition);

        while (queue.Count > 0) {
            var position = queue.Dequeue();

            if (!baseLayer.layer.ContainsKey(position) || localLayer.ContainsKey(position))
                continue;

            var dist = (byte)Mathf.RoundToInt(maxRange + 1 - ManhattanDistance(position, startPos));
            if (dist == 0) continue;

            localLayer.TryAdd(position, dist);

            // Enqueue neighboring positions
            EnqueueIfValid(queue, localLayer, position + Vector3Int.up);
            EnqueueIfValid(queue, localLayer, position + Vector3Int.down);
            EnqueueIfValid(queue, localLayer, position + Vector3Int.left);
            EnqueueIfValid(queue, localLayer, position + Vector3Int.right);
            EnqueueIfValid(queue, localLayer, position + Vector3Int.forward);
            EnqueueIfValid(queue, localLayer, position + Vector3Int.back);
        }

        // coroutine = null;
        // yield break;
        return localLayer;
    }

    public void EnqueueIfValid(Queue<Vector3Int> queue, Dictionary<Vector3Int, byte> localLayer, Vector3Int position) {
        if (baseLayer.layer.ContainsKey(position) && !localLayer.ContainsKey(position))
            queue.Enqueue(position);
    }

    public Vector3Int GetCloseWalkableNode(Vector3Int vec) {
        var neighbors = GetNeighbors(baseLayer.layer, vec.x, vec.y, vec.z);
        foreach (var neighbor in neighbors)
            if (baseLayer.layer.ContainsKey(neighbor)) {
                vec = neighbor;
                break;
            }

        return vec;
    }

    public Dictionary<Vector3Int, byte> BuildLayer() {
        // layer.Clear();

        baseLayer.BuildLayer();

        size = baseLayer.size;
        offset = baseLayer.offset;
        xSize = baseLayer.xSize;
        ySize = baseLayer.ySize;
        zSize = baseLayer.zSize;
        start = baseLayer.start;

        return layer;
    }
}