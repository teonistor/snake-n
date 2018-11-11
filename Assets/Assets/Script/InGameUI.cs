using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class InGameUI : MonoBehaviour {

    [SerializeField] Text score, timer, multiplier, smallBanner, bigBanner;
    [SerializeField] Image progress;

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
            StartCoroutine(EndLevel());
        }
    }

    IEnumerator HideBanners() {
        yield return new WaitForSeconds(8f);
        smallBanner.text = "";
        bigBanner.text = "";
    }

    IEnumerator EndLevel() {
        bigBanner.text = "Level complete";
        yield return new WaitForSeconds(9f);
        SceneManager.LoadSceneAsync("Menu"); // TODO move this logic to World and make more logical
    }
}
