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
        Invoke("loadTheLevel", 1f);

        var imageObj = GameObject.Find("fadeImage");
        if (imageObj != null) {
            image = imageObj.GetComponent<Image>();

            if(image != null)
                image.DOFade(1f, 1f);
        }
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
