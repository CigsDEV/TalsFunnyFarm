using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleScript : MonoBehaviour
{
    private void Start()
    {
        Recycle();
    }

    public void Recycle()
    {
        tmpTxt.text = this.text;
        tmpTxt.color = this.textColor;
        //33 -> 24 font
        //60 -> 16 font
        //110 -> 8 font
        if (this.text.Length <= 24)
        {
            tmpTxt.fontSize = 32;  
        }
        else if (this.text.Length <= 50)
        {
            tmpTxt.fontSize = 24;  
        }
        else if (this.text.Length <= 98)
        {
            tmpTxt.fontSize = 16;  
        }
        else
        {
            tmpTxt.fontSize = 8;
        }
        aspectRatio = (float)Screen.width / (float)Screen.height;
        //we just set the scale and position to the 2d pos for minor optimization
        bg.localScale = new Vector3(0.6717044f, 0.6717044f, 0.6717044f);
        bg.anchoredPosition = new Vector3(0f, -266.66f / aspectRatio, 0f); //-150
    }

    private void LateUpdate()
    {
        if (!infinite)
        {
            if (duration > 0f)
            {
                duration -= Time.unscaledDeltaTime * sm.localTimeScale;
            }
            else
            {
                sm.RemoveSubtitle(this);
                return;
            }
        }

        if (!is3d)
        {
            return;
        }

        //save these vals for reuse so we reduce overhead
        Vector3 camPos = sm.cameraTransform.position;
        Vector3 proPos = producer.position;
        float distance = Vector3.Distance(proPos, camPos);
        float maxDist = producerAud.maxDistance;
        float minDist = producerAud.minDistance;
        float spatial = producerAud.spatialBlend;
        //hide and return early in the case of log rolloff
        if (distance > maxDist && spatial > 0.5f)
        {
            bg.localScale = new Vector3(0f, 0f, 1f);
            return;
        }
        //calculate where to put the subtitle by via position
        float circ = Mathf.Atan2(camPos.z - proPos.z, camPos.x - proPos.x) * 57.29578f + sm.cameraTransform.rotation.eulerAngles.y + 180f;
        //offset for panning
        float circOffset = 100f * producerAud.panStereo;
        //default width
        float quickWidth = 248.88f / aspectRatio;
        //calculated with using spread
        float circWidth = Mathf.Lerp(quickWidth, -quickWidth, producerAud.spread / 360f); //140
        //set position using spatial blending, trig, and previously calculated stuff
        //also should figure out how to make this properly account for being upside-down
        bg.anchoredPosition = new Vector3((Mathf.Cos(circ * 0.017453292f) * circWidth * spatial) + circOffset, Mathf.Lerp(-266.66f / aspectRatio, Mathf.Sin(circ * 0.017453292f) * circWidth, spatial), 0f);
        float rolloffScale = 1f;
        switch (producerAud.rolloffMode)
        {
            case AudioRolloffMode.Custom:
                //its just an animation curve
                rolloffScale = producerAud.GetCustomCurve(AudioSourceCurveType.CustomRolloff).Evaluate(distance / maxDist);
                break;
            case AudioRolloffMode.Linear:
                //linear is linear tho
                rolloffScale = Mathf.Lerp(1f, 0f, (distance / maxDist) - (minDist / maxDist));
                break;
            case AudioRolloffMode.Logarithmic:
                //rollofffactor cant be gotten so we just use 1
                float rolloffFactor = 1f;
                rolloffScale = minDist * (1f/(1f + rolloffFactor * (distance - 1f)));
                break;
        }
        //multiply in volume and clamp scale
        rolloffScale = Mathf.Clamp01(producerAud.volume * rolloffScale);
        //revenge of the spatial blend
        float calculatedScale = Mathf.Lerp(1f, rolloffScale, spatial);
        //resize to be smaller
        calculatedScale *= 0.6717044f;
        //and now we actually set it
        bg.localScale = new Vector3(calculatedScale, calculatedScale, 0.6717044f);
    }

    public float duration = 1f;

    public string text = "Dummy Text";

    public bool is3d = false;

    public bool infinite = false;

    public Color textColor = new Color(1f, 1f, 1f, 1f);

    public Transform producer;

    public AudioSource producerAud;

    public SubtitleManager sm;

    public TMP_Text tmpTxt;

    public RectTransform bg;

    private float aspectRatio;
}