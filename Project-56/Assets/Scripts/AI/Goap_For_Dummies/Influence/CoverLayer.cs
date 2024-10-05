using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class CoverLayer : InfluenceLayer {
    [SerializeField] public BaseLayer baseLayer;
    private byte otherCoverWeight = 1;
    private const byte coverWeight = 5;
    private const byte fullCoverWeight = 10;
    [Space] 
    [SerializeField]private float eyeHeight = 1.5f;
    [SerializeField] private float eyeHeightDist = 1f;
    [Space]
    [SerializeField]private float halfHeight = 0.25f;
    [SerializeField] private float halfHeightDist = 1f;

    [SerializeField] private LayerMask mask = (1<<8);


    private void Awake()
    {
        layer = BuildLayer();
    }

    public Dictionary<Vector3Int, byte> BuildLayer() {
        layer.Clear();

        baseLayer.BuildLayer();

        size = baseLayer.size;
        offset = baseLayer.offset;
        xSize = baseLayer.xSize;
        ySize = baseLayer.ySize;
        zSize = baseLayer.zSize;
        start = baseLayer.start;

        var baseLayerLayer = baseLayer.GetLayer();

        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    var index = new Vector3Int(x, y, z);
                    if(!baseLayer.layer.ContainsKey(index)) continue;
                    
                    var full = CheckForFullCover(index);
                    var half = CheckForHalfCover(index);

                    var value = 0;
                    if (full) {
                        value = fullCoverWeight;
                    } else if (half && !full) {
                        value = coverWeight;
                    }
                    else {
                        continue;
                    }
                    
                    layer.TryAdd(index, (byte)value);
                }
            }
        }

        return layer;
    }

    private void CheckIfCover(int x, int y, int z, Dictionary<Vector3Int, byte> baseLayerLayer) {
        var index = new Vector3Int(x, y, z);
        if (!baseLayerLayer.ContainsKey(index)) return;

        var localNeighbors = GetNeighbors(baseLayerLayer, index.x, index.y, index.z, 1);
        foreach (var localNeighbor in localNeighbors) {
            if (index.y != localNeighbor.y) continue;
            if (!baseLayerLayer.ContainsKey(localNeighbor) || baseLayerLayer[localNeighbor] == BARRIER) {
                var value = coverWeight;
                if (CheckForFullCover(index)) value = fullCoverWeight;
                layer.TryAdd(index, value);
                break;
            }
        }
    }

    bool EyeHeightRay(Vector3Int index, Vector3 dir) {
        var worldPos = GridToWorld(index) + (Vector3.up * eyeHeight) ;
        // Debug.DrawRay(worldPos, dir * eyeHeightDist, Color.blue, 5f);
        if (Physics.Raycast(worldPos, dir, eyeHeightDist, mask)) return true;
        return false;
    }
    
    bool HalfHeightRay(Vector3Int index, Vector3 dir) {
        var worldPos = GridToWorld(index) + (Vector3.up * halfHeight) ;
        // Debug.DrawRay(worldPos, dir * halfHeightDist, Color.cyan, 5f);
        if (Physics.Raycast(worldPos, dir, halfHeightDist, mask)) return true;
        return false;
    }

    bool CheckForFullCover(Vector3Int index) {
        bool isFullCover = false;
        isFullCover = EyeHeightRay(index, Vector3.forward);
        if (isFullCover) return true;
        isFullCover = EyeHeightRay(index, Vector3.back);
        if (isFullCover) return true;
        isFullCover = EyeHeightRay(index, Vector3.left);
        if (isFullCover) return true;
        isFullCover = EyeHeightRay(index, Vector3.right);
        if (isFullCover) return true;
        
        return false;
    }
    
    bool CheckForHalfCover(Vector3Int index) {
        bool isHalfCover = false;
        isHalfCover = HalfHeightRay(index, Vector3.forward);
        if (isHalfCover) return true;
        isHalfCover = HalfHeightRay(index, Vector3.back);
        if (isHalfCover) return true;
        isHalfCover = HalfHeightRay(index, Vector3.left);
        if (isHalfCover) return true;
        isHalfCover = HalfHeightRay(index, Vector3.right);
        if (isHalfCover) return true;
        
        return false;
    }

    public override Color ByteToColor(byte i) {
        const float alpha = 0.25f;
        switch (i) {
            case fullCoverWeight:
                return new Color(0f, 1f, 0f, alpha);
            case coverWeight:
                return new Color(0f, 0.5f, 0f, alpha);
            case BARRIER:
                return Color.clear;
            default:
                return new Color(1f, 0f, 1f, alpha);
        }
    }
}