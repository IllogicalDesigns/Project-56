using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VisionSensorTests {
    private GameObject sensorObject;
    private CoffinVisionSensor visionSensor;
    private GameObject targetObject;

    private VisionCoffinDimensions primaryCoffin;
    private VisionCoffinDimensions secondaryCoffin;
    private VisionShoulderDimensions shoulderZone;


    [SetUp]
    public void Setup() {
        // Create the vision sensor object
        sensorObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sensorObject.name = "VisionSensor";
        visionSensor = sensorObject.AddComponent<CoffinVisionSensor>();
        visionSensor.eyeOffset = Vector3.zero; // Set any required values here

        // Create ScriptableObject instances for the coffins
        primaryCoffin = ScriptableObject.CreateInstance<VisionCoffinDimensions>();
        secondaryCoffin = ScriptableObject.CreateInstance<VisionCoffinDimensions>();
        shoulderZone = ScriptableObject.CreateInstance<VisionShoulderDimensions>();

        // Set values for the coffins
        primaryCoffin.farWidth = 1.875f;
        primaryCoffin.farDist = 11.25f;
        primaryCoffin.midDist = 3f;
        primaryCoffin.midWidth = 3.75f;
        primaryCoffin.closeWidth = 0.75f;
        primaryCoffin.closeDist = 0f;

        secondaryCoffin.farWidth = 2.5f;
        secondaryCoffin.farDist = 15f;
        secondaryCoffin.midDist = 4f;
        secondaryCoffin.midWidth = 5f;
        secondaryCoffin.closeWidth = 1f;
        secondaryCoffin.closeDist = 0f;

        shoulderZone.closeWidth = 2f;
        shoulderZone.closeDist = 0f;
        shoulderZone.farWidth = 2f;
        shoulderZone.farDist = -1f;

        // Assign the coffins to the VisionSensor
        visionSensor.primaryCoffin = primaryCoffin;
        visionSensor.secondaryCoffin = secondaryCoffin;
        visionSensor.shoulderZone = shoulderZone;

        // Create the target object that will be "seen" by the VisionSensor
        targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube); // Example target
        targetObject.name = "TargetObject";
    }

    [TearDown]
    public void Teardown() {
        // Clean up after each test
        Object.Destroy(sensorObject);
        Object.Destroy(targetObject);
    }

    [UnityTest]
    public IEnumerator VisionSensor_CantSeeTargetAboveHeight() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(0f, 15f, 10f); // Inside vision range
        visionSensor.visionRange = 20f; // Make sure the target is within range
        visionSensor.MaxHeight = 10f;
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(0f, visibility, "Target should be fully hidden in primary coffin within range, but over max height.");
    }

    [UnityTest]
    public IEnumerator VisionSensor_CantSeeTargetBelowHeight() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(0f, -15f, 10f); // Inside vision range
        visionSensor.visionRange = 20f; // Make sure the target is within range
        visionSensor.MaxHeight = 10f;
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(0f, visibility, "Target should be fully hidden in primary coffin within range, but under min height.");
    }

    [UnityTest]
    public IEnumerator VisionSensor_CanSeeTargetWithinRange() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(0f, 0f, 10f); // Inside vision range
        visionSensor.visionRange = 20f; // Make sure the target is within range
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(1f, visibility, "Target should be fully visible in primary coffin.");
    }

    [UnityTest]
    public IEnumerator VisionSensor_CanPartiallySeeTargetWithinRange() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(-3f, 0f, 12.9f); // Inside vision range
        visionSensor.visionRange = 20f; // Make sure the target is within range
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(0.5f, visibility, "Target should be partially visible in second coffin.");
    }

    [UnityTest]
    public IEnumerator VisionSensor_CanTargetBeDetectInShoulderArea() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(-1.5f, 0f, -0.5f); // Inside vision range
        visionSensor.visionRange = 20f; // Make sure the target is within range
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(0.5f, visibility, "Target should be partially visible in shoulder area.");
    }

    [UnityTest]
    public IEnumerator VisionSensor_CanTargetBeDetectInFlippedShoulderArea() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(1.5f, 0f, -0.5f); // Inside vision range
        visionSensor.visionRange = 20f; // Make sure the target is within range
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(0.5f, visibility, "Target should be fully visible in primary coffin.");
    }

    [UnityTest]
    public IEnumerator VisionSensor_CannotSeeTargetOutOfRange() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(50f, 0f, 0f); // Outside vision range
        visionSensor.visionRange = 30f;
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(0f, visibility, "Target should not be visible when out of range.");
    }

    [UnityTest]
    public IEnumerator VisionSensor_CannotSeeTargetWithoutLineOfSight() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(10f, 0f, 0f); // Inside vision range

        // Place an obstacle between the sensor and the target
        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.transform.position = new Vector3(5f, 0f, 0f); // In the middle between sensor and target

        visionSensor.visionRange = 20f;
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(0f, visibility, "Target should not be visible because it is obstructed by an obstacle.");

        // Clean up
        Object.Destroy(obstacle);
    }

    [UnityTest]
    public IEnumerator VisionSensor_CannotSeeTargetWithoutCoffin() {
        // Arrange
        sensorObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(0f, 0f, -10f); // Inside vision range

        visionSensor.visionRange = 20f;
        visionSensor.detectionMask = LayerMask.GetMask("Default");

        // Act
        yield return null; // Wait for the next frame to ensure objects are properly initialized
        float visibility = visionSensor.CanWeSeeTarget(targetObject);

        // Assert
        Assert.AreEqual(0f, visibility, "Target should not be visible because it is obstructed by an obstacle.");
    }
}
