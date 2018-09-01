using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] private InputField inputField;
    [SerializeField] private Text isGyro;

    public void ChangeQuality () {
        int i = int.Parse(inputField.text);
        QualitySettings.SetQualityLevel(i, true);
    }

    public void ChangeAcceleroSpeed() {
        float f = float.Parse(inputField.text);
        AcceleroTry.speed = f;
    }

    public void StartGame() {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame() {
        Application.Quit();
    }

	// Use this for initialization
	void Start () {
        isGyro.text = "Gyro is" + (Input.isGyroAvailable ? " " : " not ") + "available.";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
