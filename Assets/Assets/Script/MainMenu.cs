using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] GameObject resume, normalBottom, devToolBottom;

    void Start () {
        if (World.currentLives > 0 && World.currentLevelIndex > 0) { // TODO better condition; don't default-initialise in World?
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

    public void StartGame(int levelIndex = 0) {
        World.StartGame(levelIndex);
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
        string[] cmd = devToolBottom.GetComponentInChildren<InputField>().text.ToLower().Split(Level.OneEmpty, System.StringSplitOptions.RemoveEmptyEntries);
        switch (cmd[0]) {
            case "startfrom": StartGame(int.Parse(cmd[1])); break;
            case "turnwait": HeadOverlord.ResetTurnIntsWait = float.Parse(cmd[1]); break;
        }

        devToolBottom.GetComponentInChildren<InputField>().text = "";
        devToolBottom.SetActive(false);
        normalBottom.SetActive(true);
    }
}
