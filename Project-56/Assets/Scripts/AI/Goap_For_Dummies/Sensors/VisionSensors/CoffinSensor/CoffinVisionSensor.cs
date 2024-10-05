using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR

using UnityEditor;

#endif // UNITY_EDITOR

public class CoffinVisionSensor : MonoBehaviour, IVisionSensor {
    public LayerMask detectionMask = ~0; // default to everything

    public float visionRange = 30f, visionAngle = 60f;

    public float MaxHeight = 10f;

    public float cosVisConeAngle { get; private set; } = 0f;

    public Vector3 eyeOffset;
    [SerializeField] Transform eyes;

    [SerializeField] private bool isDebug = true;

    public VisionCoffinDimensions primaryCoffin, secondaryCoffin;
    public VisionShoulderDimensions shoulderZone;

    private float peripheryMulti = 0.5f, primaryMulti = 1f;

    private void Awake() {
        cosVisConeAngle = Mathf.Cos(visionAngle * Mathf.Deg2Rad);
    }

    public float CanWeSeeTarget(GameObject candidateGameObject) {
        return CanWeSeeTarget(candidateGameObject, Vector3.zero);
    }

    public Vector3 visionPoint() {
        return eyes.position;
    }

    public Transform visionTransform() {
        return eyes;
    }

    public float CanWeSeeTarget(GameObject candidateGameObject, Vector3 offset) {
        var candidatePosition = candidateGameObject.transform.position + offset;
        
        if (!isValid(candidateGameObject)) return 0f;
        if (!isInRange(candidatePosition)) return 0f;
        if(!inHeightRange(candidatePosition)) return 0f;

        Vector3 direction = candidatePosition - visionPoint();
        bool lineOfSight = hasLineOfSight(candidateGameObject, candidatePosition, direction);
        if (!lineOfSight) return 0f;

        bool inSecondaryVisionCoffin = inSecondaryCoffin(candidatePosition);
        bool inRearVisionZone = inOverShoulderZone(candidatePosition);
        bool inPrimaryVisionCoffin = inPrimaryCoffin(candidatePosition);


        if (!inSecondaryVisionCoffin && !inPrimaryVisionCoffin && !inRearVisionZone) return 0f;
        if (inSecondaryVisionCoffin && !inPrimaryVisionCoffin) return peripheryMulti;
        if (!inSecondaryVisionCoffin && !inPrimaryVisionCoffin && inRearVisionZone) return peripheryMulti;

        return primaryMulti;
    }

    bool inHeightRange(Vector3 candidatePosition) {
        var minHeight = visionPoint().y - MaxHeight;
        var maxHeight = visionPoint().y + MaxHeight;

        var minVec = new Vector3(visionPoint().x, minHeight, visionPoint().z);
        var maxVec = new Vector3(visionPoint().x, maxHeight, visionPoint().z);

        DebugDraw(minVec, minVec + visionTransform().forward * visionRange, Color.white);
        DebugDraw(maxVec, maxVec + visionTransform().forward * visionRange, Color.white);

        if (candidatePosition.y < minHeight || candidatePosition.y > maxHeight) {
            return false;
        }

        return true;
    }

    bool isValid(GameObject candidateGameObject) {
        //Don't detect ourseleves
        if (candidateGameObject == gameObject) return false;

        DebugDraw(transform.position, candidateGameObject.transform.position, Color.gray);

        return true;
    }

    public bool isInRange(Vector3 candidatePosition) {
        if (Vector3.Distance(candidatePosition, visionPoint()) < visionRange) {
            DebugDraw(visionPoint(), candidatePosition, Color.white);
            return true;
        }

        return false;
    }

    public bool hasLineOfSight(GameObject candidateGameObject, Vector3 candidatePosition, Vector3 direction) {
        RaycastHit hit;
        if (Physics.Raycast(visionPoint(), direction.normalized, out hit, visionRange, detectionMask)) {
            if (hit.collider.gameObject == candidateGameObject) {
                DebugDraw(visionPoint(), candidatePosition, Color.green);
                return true;
            }
            else {
                DebugDraw(visionPoint(), candidatePosition, Color.grey);
                DebugDraw(visionPoint(), hit.point, Color.yellow);
            }
        }

        return false;
    }

    public bool inPrimaryCoffin(Vector3 candidatePosition) {
        return primaryCoffin.CheckCoffin(candidatePosition, visionTransform(), isDebug);
    }

    public bool inSecondaryCoffin(Vector3 candidatePosition) {
        secondaryCoffin.farDist = visionRange;

        return secondaryCoffin.CheckCoffin(candidatePosition, visionTransform(), isDebug);
    }

    public bool inOverShoulderZone(Vector3 candidatePosition) {
        if (Vector3.Distance(candidatePosition, visionPoint()) > Mathf.Abs(shoulderZone.farDist) + shoulderZone.farWidth) return false;

        bool shoulder = shoulderZone.CheckShoulderShape(candidatePosition, false, visionTransform(), isDebug);
        if (!shoulder) shoulder = shoulderZone.CheckShoulderShape(candidatePosition, true, visionTransform(), isDebug);

        return shoulder;
    }

    private void DebugDraw(Vector3 start, Vector3 end, Color color) {
        if (isDebug) {
            Debug.DrawLine(start, end, color);
        }
    }

    private void OnDrawGizmosSelected() {
        if (isDebug)
            Gizmos.DrawWireSphere(visionPoint(), visionRange);
    }
}
