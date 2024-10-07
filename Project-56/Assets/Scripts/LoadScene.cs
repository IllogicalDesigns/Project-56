using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    Image image;

    string levelToLoad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransitionToLevel(string lvlString) {
        levelToLoad = lvlString;
        image = GameObject.Find("fadeImage").GetComponent<Image>();
        image.DOFade(1f, 1f);
        Invoke("loadTheLevel", 1f);
    }

    public void LoadLeve(int lvlInt) {
        SceneManager.LoadScene(lvlInt);
    }

    public void LoadLeve(string lvlString) {
        levelToLoad = lvlString;
        loadTheLevel();
    }

    public void loadTheLevel() {
        SceneManager.LoadScene(levelToLoad);
    }
}
