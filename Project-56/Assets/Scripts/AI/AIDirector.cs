using UnityEngine;

public class AIDirector : MonoBehaviour
{
    [SerializeField] float timeNearPlayer;
    [SerializeField] float tooMuchTimeNearPlayer = 15f;
    [SerializeField] float nearPlayer = 15f;
    Transform player;
    GAgent catAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //if(!(catAgent.currentAction is Patrol)) {
        //    return;
        //}

        //if (Vector3.Distance(player.transform.position, transform.position) < nearPlayer) {
        //    timeNearPlayer += Time.deltaTime;
        //}

        //if (timeNearPlayer > tooMuchTimeNearPlayer) {
        //    catAgent.RemoveGoal(Cat.patrolGoal);
        //    if (catAgent.currentAction is Patrol)
        //        catAgent.Replan();
        //}
    }

    public void Rested() {
        timeNearPlayer = 0;
        catAgent.AddGoal(Cat.patrolGoal, null, 5, false);
    }
}
