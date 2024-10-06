using System.Collections;
using UnityEngine;

public class Goto : GAction
{
    [SerializeField] Transform restLocation;
    [SerializeField] float restTime = 2f;
    public override IEnumerator Perform() {
        yield return gAgent.Goto(restLocation);
        yield return new WaitForSeconds(restTime);
        CompletedAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddEffects(Cat.restGoal, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
