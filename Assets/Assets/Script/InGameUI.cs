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

        // TODO Timer, multiplier
        timer.text = string.Format(timerTemplate, 3, 6);
        multiplier.text = string.Format(multiplierTemplate, 1);

        smallBanner.text = string.Format(smallBannerTemplate, World.currentLevelIndex + 1);
        bigBanner.text = World.CurrentLevelName;
        StartCoroutine(HideBanners());
	}

    void LateUpdate () {
        score.text = string.Format(scoreTemplate, World.CurrentPoints);

        Vector2 progressSize = initialProgressSize;
        progressSize.x = progressSize.x * (World.CurrentLevelRequiredPoints - World.CurrentPoints) / World.CurrentLevelRequiredPoints;
        progress.rectTransform.sizeDelta = Vector2.MoveTowards(progress.rectTransform.sizeDelta, progressSize, Time.deltaTime * 100f);

        if (World.CurrentPoints > World.CurrentLevelRequiredPoints) {
            
        }

        switch(World.GameState) {
            //case 
            //case GameState.Playing: bigBanner.text = smallBanner.text = ""; pauseUI.SetActive(false); break;
            case GameState.Paused: bigBanner.text = "Game paused."; pauseUI.SetActive(true); break;
            case GameState.LevelComplete: bigBanner.text = "Level complete"; break;
            case GameState.GameOver: bigBanner.text = "You died"; break;
        }
    }

    IEnumerator HideBanners() {
        yield return new WaitForSeconds(8f);
        smallBanner.text = "";
        bigBanner.text = "";
    }

    public void QuitToMenu() {
        SceneManager.LoadSceneAsync("Menu");
    }
}
