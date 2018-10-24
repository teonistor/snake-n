using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScrollTexture : MonoBehaviour {

    static IDictionary<string, bool> sharedMaterialMap = new Dictionary<string, bool>();
    bool scrolling;

    [SerializeField] float xSpeed = 1.5f, ySpeed = 0.6f;
    [SerializeField][Tooltip("If provided, material will be shared and only one instance active")] string sharedMaterialIdentifier;

    internal void Start () {
        if (sharedMaterialIdentifier == null || sharedMaterialIdentifier.Length == 0)
            StartCoroutine(Scroll(GetComponent<Renderer>().material));

        else if (!(sharedMaterialMap.Keys.Contains(sharedMaterialIdentifier) && sharedMaterialMap[sharedMaterialIdentifier])) {
            sharedMaterialMap[sharedMaterialIdentifier] = true;
            StartCoroutine(Scroll(GetComponent<Renderer>().sharedMaterial));
        }
    }

    internal void Stop () {
        scrolling = false;
    }

    IEnumerator Scroll (Material materialToScroll) {
        int frames = 30;
        WaitForSeconds wait = new WaitForSeconds(1f / frames);
        scrolling = true;

        while (scrolling) {
            Vector2 offset = materialToScroll.mainTextureOffset;
            offset.x = offset.x.ClockPlus(xSpeed / frames);
            offset.y = offset.y.ClockPlus(ySpeed / frames);
            materialToScroll.mainTextureOffset = offset;
            yield return wait;
        }
    }
}

internal static class ClockAddition {
    internal static float ClockPlus (this float lhs, float rhs, float limit = 1f) {
        float result = lhs + rhs;
        while (result > limit) result -= limit;
        return result;
    }
}
