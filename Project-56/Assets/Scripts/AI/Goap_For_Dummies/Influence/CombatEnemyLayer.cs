using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatEnemyLayer : GroupLayer {
    [Space] 
    [SerializeField] private byte coolingRate = 1;
    [SerializeField] private byte heatingRate = 2;
    [SerializeField] private byte minimumHeat = 1;
    [SerializeField] private byte maximumHeat = 1;
    [SerializeField] private LayerMask mask = (1 << 0);
    
    public override void FixedUpdate() {
        if(coroutine == null) coroutine = StartCoroutine(CalculateCombatLayer());
    }
    
    IEnumerator CalculateCombatLayer() {
        int maxRayFrame = 10;
        layer = new Dictionary<Vector3Int, byte>(baseLayer.layer);

        int i = 0;
        foreach (var item in group) {
            var itemGrid = WorldToGrid(item.position);

            if (!baseLayer.layer.ContainsKey(itemGrid))
                itemGrid = GetCloseWalkableNode(itemGrid);

            var localLayer = PerformFloodFill(itemGrid);
            localLayer = SightCheck(localLayer);
            CombineLayers(localLayer);

            i++;
            if (i > maxRayFrame) {
                yield return new WaitForFixedUpdate();
                i = 0;
            }
        }

        yield return new WaitForSeconds(1f);

        coroutine = null;
    }

    Dictionary<Vector3Int, byte> SightCheck(Dictionary<Vector3Int, byte> localLayer) {
        Dictionary<Vector3Int, byte> visibleLayer = new Dictionary<Vector3Int, byte>();
        foreach (var local in localLayer) {
            foreach (var enemy in group) {
                if (CheckEnemyLineOfSight(local, enemy)) 
                    continue;

                var value = local.Value + heatingRate;
                if (value > maximumHeat) value = maximumHeat;
                visibleLayer.TryAdd(local.Key, (byte)value);
            }
        }

        return visibleLayer;
    }

    private bool CheckEnemyLineOfSight(KeyValuePair<Vector3Int, byte> local, Transform enemy) {
        var worldPos = GridToWorld(local.Key);

        Debug.DrawLine(enemy.position + Vector3.up, worldPos + Vector3.up * 0.1f, Color.cyan, 1f);
        if (Physics.Linecast(enemy.position + Vector3.up, worldPos + Vector3.up * 0.1f, mask))
            return true;
        return false;
    }

    public override void CombineLayers(Dictionary<Vector3Int, byte> localLayer) {
        foreach (var local in localLayer) {
            var added = layer.TryAdd(local.Key, local.Value);
            
            layer[local.Key] += local.Value;
        }
    }
}
