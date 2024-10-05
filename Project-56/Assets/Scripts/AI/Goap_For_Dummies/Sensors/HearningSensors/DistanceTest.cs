using UnityEngine;

public class DistanceTest : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float range = 10f;
    [SerializeField] HearingSensor sensor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    private void OnDrawGizmos() {
        if(target != null) {
            Gizmos.DrawWireSphere(transform.position, range);

            Gizmos.color = sensor.CanWeHear(target.gameObject) ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
