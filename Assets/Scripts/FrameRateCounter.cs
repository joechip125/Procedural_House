using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FrameRateCounter : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI textDisplay;

    private int frames;
    private float duration;

    private void Update()
    {
        var frameDuration = Time.unscaledDeltaTime;
        frames += 1;
        duration += frameDuration;
        textDisplay.SetText($"FPS\n{1/frameDuration:0}");
    }
}
