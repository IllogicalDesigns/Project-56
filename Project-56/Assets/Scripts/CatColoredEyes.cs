using DG.Tweening;
using UnityEngine;

public class CatColoredEyes : MonoBehaviour
{
    public Light light;
    public Material patrolMaterial;
    public Material InvestigateMaterial;
    public Material attackMaterial;

    public MeshRenderer eyeRenderer;

    [Space]
    Cat cat;
    [SerializeField] Transform intrestTransform;
    [SerializeField] Transform homeTransform;

    [SerializeField] float xMove = 5f;
    [SerializeField] float duration = 5f;

    [SerializeField] Transform tailTransform;
    [SerializeField] float xTailMove = 5f;
    [SerializeField] float tailDuration = 5f;
    [SerializeField] Ease tailEase = Ease.Linear;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cat = GetComponent<Cat>();

        homeTransform.DOLocalMoveX(xMove, duration).SetLoops(-1, LoopType.Yoyo);
        tailTransform.DOLocalMoveX(xTailMove, tailDuration).SetLoops(-1, LoopType.Yoyo).SetEase(tailEase);
    }

    // Update is called once per frame
    void Update()
    {
        if(cat.topStim != null && !(cat.currentAction is Patrol)) {
            intrestTransform.position = cat.topStim.transform.position;
        } else {
            intrestTransform.position = homeTransform.position;
        }
    }

    public void SetBehaviorState(Cat.CatBehavior newState) {
        if (newState == Cat.CatBehavior.Patrol) {
            light.color = Color.white;
            eyeRenderer.material = patrolMaterial;
        }
        else if (newState == Cat.CatBehavior.Chase) {
            light.color = Color.magenta;
            eyeRenderer.material = patrolMaterial;
        }
        else if (newState == Cat.CatBehavior.Investigate) {
            light.color = Color.yellow;
            eyeRenderer.material = InvestigateMaterial;
        }
        else if (newState == Cat.CatBehavior.Attack) {
            light.color = Color.red;
            eyeRenderer.material = attackMaterial;
        }
    }
}
