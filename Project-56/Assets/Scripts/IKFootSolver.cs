using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IKFootSolver : MonoBehaviour {

    [SerializeField] Transform body = default;
    //[SerializeField] IKFootSolver otherFoot = default;
    //[SerializeField] float speed = 1;
    //[SerializeField] float stepDistance = 4;
    ////[SerializeField] float forceStepDistance = 8;
    //[SerializeField] float stepLength = 4;
    //[SerializeField] float stepHeight = 1;

    //[SerializeField] Transform Target;
    //[SerializeField] Transform normal;
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    [Space]
    [SerializeField] float maxDistance = 4;
    [SerializeField] Vector3 rayOffset = Vector3.up;
    [SerializeField] Vector3 footOffset = default;
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform bonePose;
    float bodyFootDif;
    public bool onGround;

    [SerializeField] float stepDistance = 2f;
    Vector3 lastPos;

    //[SerializeField] float wait;
    //[SerializeField] float speedSpeed = 1f;

    private void Start() {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;

        bodyFootDif = body.transform.position.y - transform.position.y;

        //yield return new WaitForSeconds(wait);
        //DOTween.To(() => footOffset.y, x => footOffset.y = x, stepHeight, speedSpeed).SetLoops(-1, LoopType.Yoyo);
        //transform.DOLocalMoveY(transform.position.y + stepHeight, speedSpeed).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame

    void Update() {
        //if(Vector3.Distance(lastPos, transform.position) > stepDistance) {
            PlaceFoot();

            MoveHip();
        //}
    }

    private void MoveHip() {
        if (transform.position.y > body.transform.position.y) {
            transform.position = new Vector3(body.transform.position.x, body.transform.position.y - bodyFootDif, body.transform.position.z);
            onGround = false;
        }
        else {
            onGround = true;
        }
    }

    private void PlaceFoot() {
        Debug.DrawRay(bonePose.position + rayOffset, -Vector3.up * maxDistance, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(bonePose.position + rayOffset, -Vector3.up, out hit, maxDistance, terrainLayer)) {
            transform.position = footOffset + hit.point;  //Place foot on ground, given some offset
            transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);    //Match foot to normal of surface
            lastPos = transform.position;
            Debug.DrawLine(bonePose.position + rayOffset, footOffset + hit.point, Color.green);
            onGround = true;
        }
        else {
            onGround = false;
        }
    }

    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.25f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentPosition, 0.25f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(currentNormal, 0.25f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(lastPos, 0.25f);
    }



    public bool IsMoving() {
        return lerp < 1;
    }



}
