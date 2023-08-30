using System;
using UnityEngine;

public class SubtitleSpawner : MonoBehaviour
{
    private void Start()
    {
        audDevice = base.gameObject.GetComponent<AudioSource>();
        sm = FindObjectOfType<SubtitleManager>();
    }

    private void OnEnable()
    {
        if (sm == null)
        {
            sm = FindObjectOfType<SubtitleManager>();
        }
        if (audDevice == null)
        {
            audDevice = base.gameObject.GetComponent<AudioSource>();
        }
        sm.Add3DSubtitle(text, audDevice.loop ? float.PositiveInfinity : audDevice.clip.length, color, base.transform);
    }

    private void OnDisable()
    {
        if (sm == null)
        {
            return;
        }
        sm.RemoveSubtitle(text);
    }

    public string text;

    public Color color = new Color(1f, 1f, 1f, 1f);

    private AudioSource audDevice;

    private SubtitleManager sm;
}