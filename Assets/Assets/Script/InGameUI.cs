using UnityEngine.UI;
using UnityEngine;

public class InGameUI : MonoBehaviour {

    [SerializeField] Text score, timer, multiplier;
    [SerializeField] Image progress;

    private string scoreTemplate, timerTemplate, multiplierTemplate;
    private Vector2 initialProgressSize;
    
	void Start () {
        scoreTemplate = score.text;
        timerTemplate = timer.text;
        multiplierTemplate = multiplier.text;
        initialProgressSize = progress.rectTransform.sizeDelta;
	}

    void LateUpdate () {
    }
}
