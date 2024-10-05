using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPaused;
    public static PlayerMovement player;
    public int cheeseCollected;
    public int cheeseGoal = 3;

    [SerializeField] GameObject escapeTrigger;

    private void Awake() {
        player = FindAnyObjectByType<PlayerMovement>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        SetCursorLock(true);
    }

    private static void SetCursorLock(bool isLocked) {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    public void SetPausedState() {
        isPaused = !isPaused;

        SetCursorLock(!isPaused);
        player.SetMoveAllowance(!isPaused);
    }

    public void PlayerHasDied(Damage damage) {
        Debug.Log("Restarting the player has died");
        RestartLevel();
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CheeseCollected() {
        cheeseCollected++;
        FindAnyObjectByType<Dialogue>()?.DisplayDialogue("<Color=yellow>You picked up some cheese! (" + cheeseCollected + "/" + cheeseGoal+")");

        if(cheeseCollected >= cheeseGoal) {
            FindAnyObjectByType<Dialogue>()?.DisplayDialogue("<Color=yellow>All Cheese collected, Escape!");
            Debug.Log("All cheese collected!");

            escapeTrigger.SetActive(true);
        }
    }

    private void OnEnable() {
        var playerHealth = player.GetComponent<Health>();
        playerHealth.death += PlayerHasDied;
    }

    private void OnDisable() {
        var playerHealth = player.GetComponent<Health>();
        playerHealth.death -= PlayerHasDied;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) {
            SetPausedState();
        }
    }
}
