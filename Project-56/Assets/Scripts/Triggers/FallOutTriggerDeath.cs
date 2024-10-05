using UnityEngine;

public class FallOutTriggerDeath : MonoBehaviour
{
    [SerializeField] float falloutHeight = -10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < falloutHeight) {
            if(TryGetComponent<Health>(out Health health)) {
                health.ApplyDamage(new Damage(10000000));
            }
        }
    }
}
