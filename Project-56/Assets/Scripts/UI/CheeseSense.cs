using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CheeseSense : MonoBehaviour
{
    private Transform closestCheese;
    private LineRenderer lineRenderer;
    [SerializeField] Color cheeseLineColor = Color.yellow;
    [SerializeField] float infrontDist = 1.5f;
    [SerializeField] Material sprite;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform home;
    public bool findHomeNow;

    [SerializeField] AudioClip sniffClip;

    Coroutine coroutine;

    void Start() {
        // If there's no LineRenderer attached to the GameObject, add one
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Set up the line renderer (you can modify these settings as needed)
        lineRenderer.material = sprite;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.startColor = cheeseLineColor;
        lineRenderer.endColor = cheeseLineColor;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // Initially no points
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) {
            AudioSource.PlayClipAtPoint(sniffClip, transform.position);
            if(coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(SenseCheese());
        }
    }

    void FindClosestCheeseObject() {
        FindAnyObjectByType<Dialogue>().DisplayDialogue("Recalculating...");
        // Get all objects with the "Cheese" script attached
        var allCheeses = FindObjectsOfType<Cheese>();

        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Cheese cheese in allCheeses) {
            // Calculate the distance between this object and the Cheese object
            float distanceToCheese = Vector3.Distance(cheese.transform.position, currentPosition);

            // If this distance is smaller than the previously stored one, update it
            if (distanceToCheese < closestDistance) {
                closestDistance = distanceToCheese;
                closestCheese = cheese.transform;
            }
        }

        if (closestCheese != null) {
            Debug.Log("Closest Cheese is at: " + closestCheese.transform.position);
        }
    }

    void FindHomeNow() {
        FindAnyObjectByType<Dialogue>().DisplayDialogue("Recalibrating nose for home...");
        closestCheese = home;
    }

    IEnumerator SenseCheese() {
        if (!findHomeNow)
            FindClosestCheeseObject();
        else
            FindHomeNow();

        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        bool pathFound = agent.CalculatePath(closestCheese.position, path);
        agent.enabled = false;
        //NavMesh.CalculatePath(transform.position, closestCheese.position, NavMesh.AllAreas, path);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position + transform.forward * infrontDist);
        lineRenderer.SetPosition(1, closestCheese.position);

        yield return new WaitForSeconds(0.15f);

        //// Wait until the path status is valid or invalid
        //while (!pathFound) {  //|| path.status == NavMeshPathStatus.PathPartial
        //    Debug.Log("Waiting for a valid path...");
        //    yield return new WaitForSeconds(0.15f);
        //    pathFound = agent.CalculatePath(closestCheese.transform.position, path);
        //}

        // Path is complete or valid
        if (path.status == NavMeshPathStatus.PathComplete) {
            Debug.Log("Valid path found!");

            // Set the number of points for the LineRenderer
            lineRenderer.positionCount = path.corners.Length;

            lineRenderer.SetPosition(0, transform.position + transform.forward * infrontDist);

            // Assign the path corners to the LineRenderer
            for (int i = 1; i < path.corners.Length; i++) {
                lineRenderer.SetPosition(i, path.corners[i] + Vector3.up);
            }
        } else if (path.status == NavMeshPathStatus.PathPartial) {
            Debug.Log("Partial path found!");

            // Set the number of points for the LineRenderer
            lineRenderer.positionCount = path.corners.Length + 1;

            lineRenderer.SetPosition(0, transform.position + transform.forward * infrontDist);

            // Assign the path corners to the LineRenderer
            for (int i = 1; i < path.corners.Length; i++) {
                lineRenderer.SetPosition(i, path.corners[i] + Vector3.up);
            }

            lineRenderer.SetPosition(path.corners.Length, closestCheese.position);
        }
        else {
            Debug.LogWarning("No valid path found.");
        }

        coroutine = null;
    }
}