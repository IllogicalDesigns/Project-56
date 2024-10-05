using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Trigger : MonoBehaviour {
    Rigidbody rigid;
    Collider col;

    [SerializeField] bool disableOnTrigger;
    [SerializeField] string requiredTag = "";

    public delegate void OntriggerEntered(Collider other);
    public event OntriggerEntered onTriggerEnter;

    public delegate void OntriggerStayed(Collider other);
    public event OntriggerStayed onTriggerStay;

    public delegate void OntriggerExited(Collider other);
    public event OntriggerExited onTriggerExit;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        rigid.isKinematic = true;
        col.isTrigger = true;
    }

    void OnTriggeredEntered(Collider other) {
        onTriggerEnter?.Invoke(other);
        DisableTrigger();
    }

    void OnTriggeredStay(Collider other) {
        onTriggerStay?.Invoke(other);
        DisableTrigger();
    }

    void OnTriggeredExited(Collider other) {
        onTriggerExit?.Invoke(other);
        DisableTrigger();
    }

    private void DisableTrigger() {
        if (disableOnTrigger) {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (requiredTag != "" && other.CompareTag(requiredTag)) {
            OnTriggeredEntered(other);
        }
        else if (requiredTag == "") {
            OnTriggeredEntered(other);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (requiredTag != "" && other.CompareTag(requiredTag)) {
            OnTriggeredStay(other);
        }
        else if (requiredTag == "") {
            OnTriggeredStay(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (requiredTag != "" && other.CompareTag(requiredTag)) {
            OnTriggeredExited(other);
        }
        else if (requiredTag == "") {
            OnTriggeredExited(other);
        }
    }
}

