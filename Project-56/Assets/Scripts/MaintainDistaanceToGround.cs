using UnityEngine;

public class MaintainDistaanceToGround : MonoBehaviour
{
    [SerializeField] float maxDistance = 4;
    [SerializeField] Vector3 rayOffset = Vector3.up;
    [SerializeField] Vector3 footOffset = default;
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform defaultTransform;
    public bool onGround;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(defaultTransform.position + rayOffset, -Vector3.up * maxDistance, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(defaultTransform.position + rayOffset, -Vector3.up, out hit, maxDistance, terrainLayer)) {
            transform.position = footOffset + hit.point;
            Debug.DrawLine(defaultTransform.position + rayOffset, footOffset + hit.point, Color.green);
        }
    }
}
