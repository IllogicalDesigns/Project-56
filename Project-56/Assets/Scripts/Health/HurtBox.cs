using UnityEngine;

[RequireComponent (typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HurtBox : MonoBehaviour
{
    Rigidbody rigid;
    Collider col;

    [SerializeField] bool disableOnDamage;
    [SerializeField] string requiredTag = "";
    [SerializeField] int damage = 50;
    //[SerializeField] float rate = 0.1f;
    //float timer;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid.isKinematic = true;
        col.isTrigger = true;
    }
    
    void OnTriggered(Collider other) {
        //timer = rate;

        if (other.TryGetComponent<Health>(out Health health)) {
            health.ApplyDamage(new Damage(damage));
        }

        if (disableOnDamage) {
            //timer = 0;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        //timer -= Time.deltaTime;

        //if (timer > 0)
        //    return;

        if (requiredTag != "" && other.CompareTag(requiredTag)) {
            OnTriggered(other);
        } else if(requiredTag == "") {
            OnTriggered(other);
        }
    }
}
