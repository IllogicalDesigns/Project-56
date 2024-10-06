using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPaused;
    public static PlayerMovement player;
    Throw thrower;
    public int cheeseCollected;
    public int cheeseGoal = 3;
    public int cheeseHeal = 50;

    [SerializeField] GameObject escapeTrigger;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] NavMeshAgent cat;

    private void Awake() {
        player = FindAnyObjectByType<PlayerMovement>();
        thrower = FindAnyObjectByType<Throw>();
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

        pauseMenu.SetActive(isPaused);

        SetCursorLock(!isPaused);
        player.SetMoveAllowance(!isPaused);
        cat.enabled = !isPaused;
        if (thrower) thrower.SetThrowAllowance(!isPaused);
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
        Health playerHealth = player.GetComponent<Health>();
        if(playerHealth != null) { playerHealth.ApplyHeal(new Damage(cheeseHeal)); }

        if(cheeseCollected >= cheeseGoal) {
            FindAnyObjectByType<Dialogue>()?.DisplayDialogue("<Color=yellow>All Cheese collected, Escape!");
            Debug.Log("All cheese collected!");

            escapeTrigger.SetActive(true);
        }
    }

    private void OnEnable() {
        if (player == null) return;
        var playerHealth = player.GetComponent<Health>();

        if (playerHealth != null)
            playerHealth.death += PlayerHasDied;
    }

    private void OnDisable() {
        if (player == null) return;
        var playerHealth = player.GetComponent<Health>();

        if(playerHealth != null)
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
