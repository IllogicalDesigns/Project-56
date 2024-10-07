using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Patrol : GAction {
    [SerializeField] PatrolPath patrolPath;
    [SerializeField] float speed = 1.5f;

    [SerializeField] AudioSource patrolSoundd;

    public override IEnumerator Perform() {
        if (patrolPath == null) {
            Debug.Log("No patrol path assigned to Patrol for " + gameObject.name);
            yield break;
        }

        patrolSoundd.Play();

        gAgent.agent.speed = speed;

        Transform[] queue = GetReorderedQueueOfPoints();

        foreach (var point in queue) {
            yield return gAgent.Goto(point.transform.position);
        }

        patrolSoundd.Stop();

        CompletedAction();
    }

    private Transform[] GetReorderedQueueOfPoints() {
        int closestIndex = GetClosestPointIndex(transform.position, patrolPath.patrolPoints);
        Transform[] reorderedPatrolPoints = new Transform[patrolPath.patrolPoints.Length];

        for (int i = 0; i < patrolPath.patrolPoints.Length; i++) {
            int index = (i + closestIndex) % patrolPath.patrolPoints.Length;
            reorderedPatrolPoints[i] = patrolPath.patrolPoints[index];
        }

        // Use the reordered array as your new queue
        Transform[] queue = reorderedPatrolPoints;
        return queue;
    }

    int GetClosestPointIndex(Vector3 pos, Transform[] points) {
        float minDistance = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < points.Length; i++) {
            float distance = Vector3.Distance(pos, points[i].position);
            if (distance < minDistance) {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public override void Interruppted() {
        patrolSoundd.Stop();
        base.Interruppted();
    }


    // Start is called before the first frame update
    void Start() {
        AddEffects(Cat.patrolGoal);
    }
}
