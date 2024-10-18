using UnityEngine;

public class HelpManager : MonoBehaviour
{
    [SerializeField] string sneakReminderText = "Left control or C to sneak";
    [SerializeField] Transform player;
    [SerializeField] Transform sneakReminderDistance;
    [SerializeField] float sneakDistance = 15f;
    [SerializeField] float sneakrCoolDown = 15f;
    float sneakTimer;

    [Space]
    [SerializeField] string sprintReminderText = "Left shift to run away!";
    Cat cat;
    [SerializeField] float sprintCoolDown = 15f;
    float sprintTimer;

    [Space]
    [SerializeField] string gpsReminderText = "Press F to find the closest cheese scent";
    [SerializeField] float gpsCoolDown = 60f;
    float gpsTimer = 30f;

    [Space]
    [SerializeField] string leanReminderText = "Press Q or E to lean around corners";
    Camera cam;
    [SerializeField] LayerMask mask = ~8;
    [SerializeField] Vector3 leanDectionOffset = Vector3.right;
    [SerializeField] float leanDetectionRange = 2f;
    [SerializeField] float leanCoolDown = 15f;
    [SerializeField] float leanTimer = 15f;

    Dialogue dialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogue = FindAnyObjectByType<Dialogue>();
        cat = FindAnyObjectByType<Cat>();
        player = GameManager.player.transform;

        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        RemindToSneak();
        RemindToSprint();
        RemindToGPS();
        RemindToLean();
    }

    private void RemindToLean() {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)) {
            leanTimer = leanCoolDown;
        }

        if (leanTimer > 0) {
            leanTimer -= Time.deltaTime;
            return;
        }

        bool front = Physics.Raycast(cam.transform.position, cam.transform.forward, leanDetectionRange, mask);
        bool right = Physics.Raycast(cam.transform.position + leanDectionOffset, cam.transform.forward, leanDetectionRange, mask);
        bool left = Physics.Raycast(cam.transform.position + -leanDectionOffset, cam.transform.forward, leanDetectionRange, mask);

        bool Display = !(front && left && right) && !(!front && !left && !right);

        Debug.DrawRay(cam.transform.position, cam.transform.forward * leanDetectionRange, Display ? Color.green : Color.red);
        Debug.DrawRay(cam.transform.position + leanDectionOffset, cam.transform.forward * leanDetectionRange, Display ? Color.green : Color.red);
        Debug.DrawRay(cam.transform.position + -leanDectionOffset, cam.transform.forward * leanDetectionRange, Display ? Color.green : Color.red);

        if (leanTimer <= 0 && Display) {
            dialogue.DisplayDialogue(leanReminderText);
            leanTimer = leanCoolDown;
        }
    }

    public void DelayGPSReminder() {
        gpsTimer = gpsCoolDown;
    }

    private void RemindToGPS() {
        if (Input.GetKeyDown(KeyCode.F)) {
            gpsTimer = gpsCoolDown;
        }

        if (gpsTimer <= 0) {
            dialogue.DisplayDialogue(gpsReminderText);
            gpsTimer = gpsCoolDown;
        }

        if (gpsTimer > 0) { gpsTimer -= Time.deltaTime; }
    }

    private void RemindToSprint() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            sprintTimer = sprintCoolDown;
        }

        if (cat != null && cat.currentAction is Attack) {
            dialogue.DisplayDialogue(sprintReminderText);
            sprintTimer = sprintCoolDown;
        }

        if (sprintTimer > 0) { sprintTimer -= Time.deltaTime; }
    }

    private void RemindToSneak() {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C)) {
            sneakTimer = sneakrCoolDown;
        }

        if (sneakTimer <= 0 && Vector3.Distance(player.position, sneakReminderDistance.position) < sneakDistance) {
            dialogue.DisplayDialogue(sneakReminderText);
            sneakTimer = sneakrCoolDown;
        }

        if (sneakTimer > 0) { sneakTimer -= Time.deltaTime; }
    }
}
