using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform launchTransform;
    [SerializeField] float force;

    public bool canThrow = true;

    public bool infinateAmmo;
    public int ammo = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetThrowAllowance(bool allowedToThrow) {
        canThrow = allowedToThrow;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canThrow) { return; }

        if(Input.GetKeyDown(KeyCode.N)) {
            infinateAmmo = !infinateAmmo;
        }

        if (Input.GetMouseButtonDown(0) && (infinateAmmo || ammo > 0)) {
            if(!infinateAmmo) ammo--;
            var newPrefab = Instantiate(prefab, launchTransform.position, launchTransform.rotation) as GameObject;
            var rigidBody = newPrefab.GetComponent<Rigidbody>();
            rigidBody.AddForce(launchTransform.forward * force, ForceMode.Impulse);
        } else if(Input.GetMouseButtonDown(0) && ammo <= 0) {
            FindAnyObjectByType<Dialogue>().DisplayDialogue("Out of pebbles to throw...");
        }
    }

    private void OnDrawGizmos() {
        if (launchTransform == null) {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(launchTransform.position, launchTransform.forward * 2f);
    }
}
