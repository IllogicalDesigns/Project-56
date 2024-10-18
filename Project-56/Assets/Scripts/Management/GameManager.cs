using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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

    [SerializeField] TextMeshProUGUI godModeText;

    bool canDie = true;

    string levelToLoad;

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

    public void TogglePausedState() {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);

        SetCursorLock(!isPaused);
        player.SetMoveAllowance(!isPaused);
        cat.enabled = !isPaused;
        if (thrower) thrower.SetThrowAllowance(!isPaused);
    }

    public void PlayerHasDied(Damage damage) {
        if (!canDie) {
            var playerHealth = player.GetComponent<Health>();
            playerHealth.health = playerHealth.maxHealth;
            return;
        }

        Debug.Log("Restarting the player has died");
        var load = GetComponent<LoadScene>();
        var imageObj = GameObject.Find("fadeImage");
        var img = imageObj.GetComponent<Image>();
        img.color = Color.black;
        load.TransitionToLevel(SceneManager.GetActiveScene().name);
        //RestartLevel();
    }

    public void Escaped() {
        GetComponent<LoadScene>().TransitionToLevel("End");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel() {
        var load = GetComponent<LoadScene>();
        load.TransitionToLevel(SceneManager.GetActiveScene().name);
    }

    public void CheeseCollected() {
        cheeseCollected++;
        FindAnyObjectByType<Dialogue>()?.DisplayDialogue("<Color=yellow>You picked up some cheese! (" + cheeseCollected + "/" + cheeseGoal+")");
        FindAnyObjectByType<HelpManager>()?.DelayGPSReminder();
        Health playerHealth = player.GetComponent<Health>();
        if(playerHealth != null) { playerHealth.ApplyHeal(new Damage(cheeseHeal)); }

        if(cheeseCollected >= cheeseGoal) {
            FindAnyObjectByType<Dialogue>()?.DisplayDialogue("<Color=yellow>All Cheese collected, Time to go home!");
            FindAnyObjectByType<CheeseSense>().findHomeNow = true;
            FindAnyObjectByType<Patrol>().patrolAroundTV = true;

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
            TogglePausedState();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            canDie = !canDie;
            if(godModeText != null) godModeText.gameObject.SetActive(!canDie);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            for (int i = 0; i < cheeseGoal+1; i++) {
                CheeseCollected();
            }
        }
    }
}
