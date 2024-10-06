using System.Collections;
using UnityEngine;

public class Rest : GAction {
    [SerializeField] Transform restLocation;
    [SerializeField] float restTime = 5f;
    [SerializeField] float speed = 2f;
    public override IEnumerator Perform() {
        gAgent.agent.speed = speed;
        yield return gAgent.Goto(restLocation);
        yield return new WaitForSeconds(restTime);
        gAgent.AddGoal(Cat.patrolGoal, null, 5, true);
        GetComponent<AIDirector>().Rested();
        CompletedAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        AddEffects(Cat.restGoal, null);
    }

    // Update is called once per frame
    void Update() {

    }
}

