using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Stalk : GAction
{
    [SerializeField] float innerRange = 10f;
    [SerializeField] float outerRange = 30f;

    //[SerializeField] float innerRange = 10f;
    [SerializeField] float currentOuterRange = 30f;
    [SerializeField] float shrinkRate = 1.5f;

    [SerializeField] Vector3 pointOfInterest;

    [SerializeField] float speed = 3.5f;
    [SerializeField] float requiredBuffer = 2f;
    [SerializeField] float frontDotRequirement = 0.25f;
    [SerializeField] float maxDistance = 10f;

    Vector3 randdomPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddEffects(Cat.stalkGoal, null);
        AddPreconditions(Cat.pointOfInterest, null);
    }

    // Update is called once per frame
    void Update()
    {
        //Tighten this search circle over time well in stalk
        if (outerRange > innerRange) {
            currentOuterRange -= shrinkRate * Time.deltaTime;
        }
    }

    public override IEnumerator Perform() {
        var poi = gAgent.agentState.GetStates()[Cat.pointOfInterest];
        if (poi == null) { yield break; }

        pointOfInterest = (Vector3)poi;

        currentOuterRange = outerRange;

        gAgent.agent.speed = speed;

        int allowedFails = 25;
        do {
            var randPos = GetRandomPosition(pointOfInterest, innerRange, currentOuterRange);
            randdomPosition = randPos;
            if (!IsInFront(randPos)) {
                allowedFails--;
                continue;
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randPos, out hit, maxDistance, NavMesh.AllAreas)) {
                randPos = hit.position;
            }

            yield return gAgent.Goto(randPos);
        } while (allowedFails > 0 && currentOuterRange > innerRange + requiredBuffer);

        ResetDebugs();

        CompletedAction();
    }

    private void ResetDebugs() {
        pointOfInterest = Vector3.zero;
        randdomPosition = Vector3.zero;
    }

    public override void Interruppted() {
        ResetDebugs();
        base.Interruppted();
    }

    // Method to get a random point between the inner and outer radius
    public Vector3 GetRandomPosition(Vector3 center, float innerRadius, float outerRadius) {
        // Generate a random angle in radians
        float randomAngle = Random.Range(0f, Mathf.PI * 2);

        // Generate a random distance between inner and outer radius
        float randomDistance = Random.Range(innerRadius, outerRadius);

        // Calculate the x and z coordinates using polar coordinates to Cartesian coordinates conversion
        float xOffset = Mathf.Cos(randomAngle) * randomDistance;
        float zOffset = Mathf.Sin(randomAngle) * randomDistance;

        // Return the new random position
        return new Vector3(center.x + xOffset, center.y, center.z + zOffset);
    }

    bool IsInFront(Vector3 target) {
        // Get the direction to the target
        Vector3 directionToTarget = (target - transform.position).normalized;

        // Get the forward direction of the player
        Vector3 forward = transform.forward;

        // Calculate the dot product between the others's forward direction and the direction to the target
        float dotProduct = Vector3.Dot(forward, directionToTarget);

        // If the dot product is greater than 0, the target is in front of the others
        return dotProduct > frontDotRequirement;
    }


    private void OnDrawGizmos() {
        if (pointOfInterest == Vector3.zero) return;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(pointOfInterest, innerRange);
        Gizmos.DrawWireSphere(pointOfInterest, currentOuterRange);

        if (randdomPosition == Vector3.zero) return;
        Gizmos.DrawSphere(randdomPosition, 0.5f);
    }
}
