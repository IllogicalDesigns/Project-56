using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour {
    public Transform[] patrolPoints;

    private void OnDrawGizmosSelected() {
        if (patrolPoints.Length <= 1)
            return;

        // Loop through each point and draw lines between them
        for (int i = 0; i < patrolPoints.Length; i++) {
            // Draw a line from the current point to the next, looping back to the start
            var nextIndex = (i + 1) % patrolPoints.Length;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
        }
    }
}

