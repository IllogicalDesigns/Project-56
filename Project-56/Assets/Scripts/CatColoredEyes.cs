using UnityEngine;

public class CatColoredEyes : MonoBehaviour
{
    public Light light;
    public Material patrolMaterial;
    public Material InvestigateMaterial;
    public Material attackMaterial;

    public MeshRenderer eyeRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBehaviorState(Cat.CatBehavior newState) {
        if (newState == Cat.CatBehavior.Patrol) {
            light.color = Color.white;
            eyeRenderer.material = patrolMaterial;
        }
        else if (newState == Cat.CatBehavior.Investigate) {
            light.color = Color.yellow;
            eyeRenderer.material = InvestigateMaterial;
        }
        else if (newState == Cat.CatBehavior.Chase) {
            light.color = Color.red;
            eyeRenderer.material = attackMaterial;
        }
    }
}
