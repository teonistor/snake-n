using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] GameObject resume, normalBottom, devToolBottom;

    void Start () {
        if (World.currentLevelIndex > 0) { // TODO better condition
            resume.SetActive(true);
        }
    }
        //public void ChangeQuality () {
        //    int i = int.Parse(inputField.text);
        //    QualitySettings.SetQualityLevel(i, true);
        //}

        //public void ChangeAcceleroSpeed() {
        //    float f = float.Parse(inputField.text);
        //    GoproAcclr.speed = f;
        //}

     public void ContinueGame () {
        SceneManager.LoadSceneAsync(1);
    }

    public void StartGame() {
        World.currentLevelIndex = 0;
        World.currentEnergy = World.targetEnergy = 25;
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ActivateDevTool () {
        normalBottom.SetActive(false);
        devToolBottom.SetActive(true);
    }

    public void SubmitDevTool () {
        // devToolBottom.GetComponentInChildren<InputField>().text; //...
        devToolBottom.GetComponentInChildren<InputField>().text = "";
        devToolBottom.SetActive(false);
        normalBottom.SetActive(true);
    }
}
