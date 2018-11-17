using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class InGameUI : MonoBehaviour {

    [SerializeField] Text score, timer, multiplier, smallBanner, bigBanner;
    [SerializeField] Image progress;
    [SerializeField] GameObject pauseUI;

    private string scoreTemplate, timerTemplate, multiplierTemplate, smallBannerTemplate;
    private Vector2 initialProgressSize;
    
	void Start () {
        scoreTemplate = score.text;
        timerTemplate = timer.text;
        multiplierTemplate = multiplier.text;
        smallBannerTemplate = smallBanner.text;
        initialProgressSize = progress.rectTransform.sizeDelta;

        smallBanner.text = string.Format(smallBannerTemplate, World.currentLevelIndex + 1);
        bigBanner.text = World.CurrentLevelName;
	}

    void LateUpdate () {
        score.text = string.Format(scoreTemplate, World.CurrentPoints);
        multiplier.text = string.Format(multiplierTemplate, World.currentLives);
        int remainingTime = Mathf.Max(0, (int)((World.CurrentLevelTimeLimit - Time.time) / World.BaseTimeScale + 1));
        timer.text = string.Format(timerTemplate, remainingTime / 60, remainingTime % 60);

        Vector2 progressSize = initialProgressSize;
        progressSize.x = progressSize.x * (World.CurrentLevelRequiredPoints - World.CurrentPoints) / World.CurrentLevelRequiredPoints;
        progress.rectTransform.sizeDelta = Vector2.MoveTowards(progress.rectTransform.sizeDelta, progressSize, Time.deltaTime * 100f);

        switch(World.GameState) {
            //case 
            case GameState.Playing: bigBanner.text = smallBanner.text = ""; pauseUI.SetActive(false); break;
            case GameState.Paused: bigBanner.text = "Game paused."; pauseUI.SetActive(true); break;
            case GameState.LevelComplete: bigBanner.text = "Level complete"; break;
            case GameState.GameOver: bigBanner.text = "You died"; break;
        }
    }

    // Button action
    public void QuitToMenu() {
        SceneManager.LoadSceneAsync("Menu");
    }
}
