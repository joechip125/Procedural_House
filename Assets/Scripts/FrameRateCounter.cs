using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FrameRateCounter : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI textDisplay;
    
    [SerializeField, Range(0.1f, 2f)]
    float sampleDuration = 1f;

    private int frames;
    private float duration, bestDuration = float.MaxValue, worstDuration;

    private void Update()
    {
        var frameDuration = Time.unscaledDeltaTime;
        frames += 1;
        duration += frameDuration;
        
        if (frameDuration < bestDuration) 
        {
            bestDuration = frameDuration;
        }
        if (frameDuration > worstDuration) 
        {
            worstDuration = frameDuration;
        }
        if (duration >= sampleDuration)
        {
            textDisplay.SetText($"FPS\n{frames/duration:0}\n{1f/bestDuration:0}\n{1f/worstDuration:0}");
            frames = 0;
            duration = 0f;
        }
    }
}
