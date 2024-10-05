using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.TestTools;
using Unity.AI.Navigation;

public class HearingTests {
    private GameObject sensorObject;
    private HearingSensor hearingSensor;
    private GameObject sourceObject;
    private GameObject planeObject;
    private NavMeshSurface navMeshSurface;
    private GameObject obstacleObject;


    [SetUp]
    public void SetUp() {
        sensorObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sensorObject.name = "HearingSensor";

        hearingSensor = sensorObject.AddComponent<HearingSensor>();

        sourceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sourceObject.name = "audioSource";

        planeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        navMeshSurface = planeObject.AddComponent<NavMeshSurface>();
        navMeshSurface.agentTypeID = 0;
        planeObject.transform.localScale = Vector3.one * 5f;
        planeObject.transform.position = new Vector3(0, 0, 0);

        obstacleObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacleObject.name = "obstacle";
        obstacleObject.transform.localScale = new Vector3(10,2,1);
        obstacleObject.transform.position = new Vector3(0, 1, 0);
        obstacleObject.SetActive(false);

        navMeshSurface.BuildNavMesh();
    }

    [TearDown]
    public void TearDown() {
        // Clean up objects
        Object.Destroy(sensorObject);
        Object.Destroy(sourceObject);
        Object.Destroy(planeObject);
        Object.Destroy(obstacleObject);
    }

    [UnityTest]
    public IEnumerator HearingSensor_CanHearSound_WhenInRange() {
        sensorObject.transform.position = new Vector3(0, 0, 0);
        sourceObject.transform.position = new Vector3(0, 0, 20);

        hearingSensor.hearingRange = 25f;
        hearingSensor.onScreen = true;

        // Wait for one frame to ensure everything is set up correctly
        yield return null;

        var canHear = hearingSensor.CanWeHear(sourceObject);

        Assert.IsTrue(canHear, "The sensor should hear the sound when in range.");
    }

    [UnityTest]
    public IEnumerator HearingSensor_CannotHearSound_WhenOutOfRange() {
        sensorObject.transform.position = new Vector3(0, 0, 0);
        sourceObject.transform.position = new Vector3(0, 0, 30f);

        hearingSensor.hearingRange = 25f;
        hearingSensor.onScreen = true;

        // Wait for one frame to ensure everything is set up correctly
        yield return null;

        var canHear = hearingSensor.CanWeHear(sourceObject);

        Assert.IsTrue(!canHear, "Source can't be heard as the source is out of range");
    }

    [UnityTest]
    public IEnumerator HearingSensor_CanHearSound_WhenNotOnScreenAndInOnScreenRange() {
        sensorObject.transform.position = new Vector3(0, 0, 0);
        sensorObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        sourceObject.transform.position = new Vector3(0, 0, 5f);

        hearingSensor.hearingRange = 25f;
        hearingSensor.ofScreenHearingRange = 10f;
        hearingSensor.onScreen = false;

        // Wait for one frame to ensure everything is set up correctly
        yield return null;

        var canHear = hearingSensor.CanWeHear(sourceObject);

        Assert.IsTrue(canHear, "Source can be heard when in off screen range and off screen");
    }

    [UnityTest]
    public IEnumerator HearingSensor_CannotHearSound_WhenOffScreenAndOutOfOffScreenRange() {
        sensorObject.transform.position = new Vector3(0, 0, 0);
        sensorObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        sourceObject.transform.position = new Vector3(0, 0, 15f);

        hearingSensor.hearingRange = 25f;
        hearingSensor.ofScreenHearingRange = 10f;
        hearingSensor.onScreen = false;

        // Wait for one frame to ensure everything is set up correctly
        yield return null;

        var canHear = hearingSensor.CanWeHear(sourceObject);

        Assert.IsTrue(!canHear, "Source can't be heard because, its off screen and out of off screen range");
    }

    [UnityTest]
    public IEnumerator HearingSensor_CanHearWhenObstacleButStillInRange() {
        sensorObject.transform.position = new Vector3(0, 0, -5f);
        sensorObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        sourceObject.transform.position = new Vector3(0, 0, 5f);

        hearingSensor.hearingRange = 25f;
        hearingSensor.ofScreenHearingRange = 10f;
        hearingSensor.onScreen = true;

        obstacleObject.SetActive(true);

        navMeshSurface.BuildNavMesh();

        // Wait for one frame to ensure everything is set up correctly
        yield return null;

        var canHear = hearingSensor.CanWeHear(sourceObject);

        Assert.IsTrue(canHear, "HearingSensor_CanHearWhenObstacleButStillInRange");
    }

    [UnityTest]
    public IEnumerator HearingSensor_CantHearWhenObstacleMakesOutOfRange() {
        sensorObject.transform.position = new Vector3(0, 0, -10f);
        sensorObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        sourceObject.transform.position = new Vector3(0, 0, 10f);

        hearingSensor.hearingRange = 22f;  //Distance on nav is 22.9f, so lower range
        hearingSensor.ofScreenHearingRange = 10f;
        hearingSensor.onScreen = true;

        obstacleObject.SetActive(true);

        navMeshSurface.BuildNavMesh();

        // Wait for one frame to ensure everything is set up correctly
        yield return null;

        var canHear = hearingSensor.CanWeHear(sourceObject);

        Assert.IsTrue(!canHear, "HearingSensor_CantHearWhenObstacleMakesOutOfRange");
    }
}

