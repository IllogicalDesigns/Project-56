using UnityEngine;

public class PounceSensor : MonoBehaviour
{
    public Transform playerTransform; // Assign the player transform in the inspector
    public float navMeshCheckRadius = 1.0f; // Adjust based on your NavMesh setup
    public bool onMesh;

    private void Start() {
        playerTransform = GameManager.player.transform;
    }

    private void Update() {
        onMesh = IsPlayerOnCatNavMesh();
    }

    // Check if the player is on the cat's NavMesh
    bool IsPlayerOnCatNavMesh() {
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(playerTransform.position, out hit, navMeshCheckRadius, UnityEngine.AI.NavMesh.AllAreas)) {
            return true;
        }
        return false;
    }
}
