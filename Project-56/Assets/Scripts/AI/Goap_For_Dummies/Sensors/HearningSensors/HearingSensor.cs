using UnityEngine;
using UnityEngine.AI;

public class HearingSensor : MonoBehaviour, IHearingSensor {
    public float hearingRange = 30f;
    public float ofScreenHearingRange = 10f;
    [SerializeField] private Color hearingColor = new Color(1f, 1f, 0f, 0.25f);

    [Space]
    [SerializeField] private LayerMask playerSightTestMask = ~0;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform tailTransform;
    [SerializeField] Transform headTransform;
    public bool onScreen;
    [SerializeField] float dotRequirement = 0.25f;

    [Space]
    [SerializeField] private bool isDebug = false;
    float debugDistance;

    private bool OnScreen() {
        onScreen = IsInFront(playerTransform);

        if (onScreen) {
            onScreen = !Physics.Linecast(playerTransform.position, transform.position, playerSightTestMask);
            if (onScreen) return true;

            onScreen = !Physics.Linecast(playerTransform.position, tailTransform.position, playerSightTestMask);
            if (onScreen) return true;

            onScreen = !Physics.Linecast(playerTransform.position, headTransform.position, playerSightTestMask);
            if (onScreen) return true;
        }

        return onScreen;
    }

    private void Update() {
        onScreen = OnScreen();
    }


    // Method to check if the target is in front of the others
    //This does the calculation to see if transform.position is in front of the other
    //The other should be the player, so in theory that means we are in front of them and can
    //use the wider hearing range, so we nerf when we are off screen for the player
    bool IsInFront(Transform target) {
        // Get the direction to the target
        Vector3 directionToTarget = (transform.position - target.position).normalized;

        // Get the forward direction of the player
        Vector3 otherForward = target.forward;

        // Calculate the dot product between the others's forward direction and the direction to the target
        float dotProduct = Vector3.Dot(otherForward, directionToTarget);

        // If the dot product is greater than 0, the target is in front of the others
        return dotProduct > dotRequirement;
    }

    private bool InRange(Vector3 sourcePosition, float hearingDistance) {
        var distToSound = Vector3.Distance(sourcePosition, transform.position);

        if (distToSound > hearingDistance) return false;
        
        return true;
    }

    private float DistanceOnNavMesh(Vector3 sourcePosition) {
        float totalDist = 0f;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, sourcePosition, NavMesh.AllAreas, path);

        for (int i = 0; i < path.corners.Length - 1; i++) {
            totalDist += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            Debug.DrawLine(path.corners[i], path.corners[i + 1], totalDist > debugDistance ? Color.red : Color.green, 1f);
        }

        return totalDist;
    }

    public bool CanWeHear(GameObject candidateAudioSource, float overrideHearingDist = 0) {
        Vector3 sourcePosition = candidateAudioSource.transform.position;
        onScreen = OnScreen(); //if we are not onScreen lower the hearing ability of the sensor

        if (overrideHearingDist == 0) {
            overrideHearingDist = GetHearingRangeBasedOnOnScreen();
        }

        debugDistance = overrideHearingDist;

        //Quick check to see that we are in range of the max distance
        if (!InRange(sourcePosition, overrideHearingDist)) {
            Debug.Log("Sound:" + candidateAudioSource.name + " heard " + "NA" + " units away. overrideHearingDist is" + overrideHearingDist + " SoundHeard:" + false);
            return false;
        }

        float totalDist = DistanceOnNavMesh(sourcePosition);

        bool isHeard = totalDist < overrideHearingDist;

        if (isDebug)
            Debug.Log("Sound:" + candidateAudioSource.name + " heard " + totalDist + " units away. overrideHearingDist is" + overrideHearingDist + " SoundHeard:" + isHeard);

        return isHeard;
    }

    private float GetHearingRangeBasedOnOnScreen() {
        return (onScreen ? hearingRange : ofScreenHearingRange);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = hearingColor;
        Gizmos.DrawWireSphere(transform.position, GetHearingRangeBasedOnOnScreen());
    }
}
