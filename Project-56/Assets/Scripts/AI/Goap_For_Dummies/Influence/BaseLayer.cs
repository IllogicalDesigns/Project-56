using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLayer : InfluenceLayer
{
    [Header("Grid settings")]
    [SerializeField] public MeshRenderer boundsHint;

    [Header("Nav check")]
    [SerializeField] private bool navCheck = true;
    [SerializeField] private float navMeshSampleAllowance = 0.2f;
    [Space]
    [SerializeField] private bool dropNavagationPoints = false;
    [SerializeField] private Vector3 dropNavOffset = new Vector3(0f, 0.25f, 0f);
    [SerializeField] private float dropNavLength = 1.6f;
    [SerializeField] private LayerMask layerMask = 1 << 0;
    // Start is called before the first frame update
    private void Awake()
    {
        BuildLayer();
        
        if(layer==null) 
            Debug.Log(("No grid data found!!!"));
    }

    #region GridCreation

    public void BuildLayer() {
        if (!HasBoundsHint()) return;
        
        InitializeGridSizeBasedOnBounds();
        InitializeGridStartBasedOnBounds();

        byte[,,] grid = new byte[xSize, ySize, zSize];

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

                    grid[x, y, z] = STARTING;
                }
            }
        }

        layer = ToSparseArray(grid);
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

    private bool HasNavMesh(Vector3 location) {
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(location, out hit, navMeshSampleAllowance, -1)) {
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
}
