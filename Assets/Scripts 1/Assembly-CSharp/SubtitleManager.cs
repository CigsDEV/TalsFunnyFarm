using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    private void OnDestroy()
    {
        foreach(SubtitleScript subtitle in subtitles2d)
        {
            if (subtitle == null)
            {
                continue;
            }
            DestroySubtitle(subtitle);
        }
        subtitles2d.Clear();
        foreach(SubtitleScript subtitle in subtitles3d)
        {
            if (subtitle == null)
            {
                continue; 
            }
            DestroySubtitle(subtitle);
        }
        subtitles3d.Clear();
        foreach(SubtitleScript subtitle in pooledSubtitles)
        {
            if (subtitle == null)
            {
                continue;
            }
            DestroySubtitle(subtitle);
        }
        pooledSubtitles.Clear();
    }

    //UNCOMMENT THE PLAYERPREFS STUFF IF YOU WANT TO MAKE AN OPTION FOR THIS, OTHERWISE LEAVE IT AS IS
    public void AddChained3DSubtitle(string[] texts, float[] durations, Color[] colors, Transform producer, AudioSource producerAudio = null)
    {
        /*if (PlayerPrefs.GetInt("Subtitles") != 1)
        {
            return null;
        }*/

        base.StartCoroutine(ChainedSubtitleLoop(texts, durations, colors, producer, producerAudio));
    }

    public void AddChained2DSubtitle(string[] texts, float[] durations, Color[] colors)
    {
        /*if (PlayerPrefs.GetInt("Subtitles") != 1)
        {
            return null;
        }*/

        base.StartCoroutine(ChainedSubtitleLoop(texts, durations, colors, null, null));
    }

    public SubtitleScript Add3DSubtitle(string text, float duration, Color color, Transform producer, AudioSource producerAudio = null)
    {
        /*if (PlayerPrefs.GetInt("Subtitles") != 1)
        {
            return null;
        }*/

        SubtitleScript subtitle = AddSubtitle(text, duration, color);
        subtitle.is3d = true;
        subtitle.producer = producer;
        subtitle.producerAud = (producerAudio == null ? producer.gameObject.GetComponent<AudioSource>() : producerAudio);
        subtitles3d.Add(subtitle);
        return subtitle;
    }

    public SubtitleScript Add2DSubtitle(string text, float duration, Color color)
    {
        /*if (PlayerPrefs.GetInt("Subtitles") != 1)
        {
            return null;
        }*/

        SubtitleScript subtitle = AddSubtitle(text, duration, color);
        subtitle.is3d = false;
        subtitles2d.Add(subtitle);
        return subtitle;
    }

    private SubtitleScript AddSubtitle(string text, float duration, Color color)
    {
        SubtitleScript subtitle = null;
        if (TryGetPooledSubtitle() != null)
        {
            subtitle = TryGetPooledSubtitle();
            pooledSubtitles.Remove(subtitle);
            subtitle.infinite = (duration == float.PositiveInfinity);
            subtitle.duration = duration;
            subtitle.textColor = color;
            subtitle.text = text;
            subtitle.Recycle();
            subtitle.gameObject.SetActive(true);
            subtitle.gameObject.transform.SetAsLastSibling();
        }
        else
        {
            GameObject subtitleObject = Instantiate<GameObject>(subtitlePrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
            subtitleObject.transform.SetParent(subtitleHud, false);
            subtitle = subtitleObject.GetComponent<SubtitleScript>();
            subtitle.infinite = (duration == float.PositiveInfinity);
            subtitle.duration = duration;
            subtitle.textColor = color;
            subtitle.text = text;
            subtitle.sm = this;
            subtitleObject.transform.SetAsLastSibling();
        }

        return subtitle;
    }

    private SubtitleScript TryGetPooledSubtitle()
    {
        if (pooledSubtitles.Count > 0)
        {
            return pooledSubtitles[0];
        }
        return null;
    }

    private void AddToPool(SubtitleScript subtitle)
    {
        pooledSubtitles.Add(subtitle);
        subtitle.gameObject.SetActive(false);
    }

    public void RemoveSubtitle(SubtitleScript subtitle)
    {
        if (subtitle.is3d)
        {
            subtitles3d.Remove(subtitle);
        }
        else
        {
            subtitles2d.Remove(subtitle);
        }
        AddToPool(subtitle);
    }

    private void DestroySubtitle(SubtitleScript subtitle)
    {
        UnityEngine.Object.Destroy(subtitle.gameObject, 0f);
    }

    //wish i could just use a dictionary but cant because duplicate subtitles
    public void RemoveSubtitle(string subtitleText)
    {
        /*if (PlayerPrefs.GetInt("Subtitles") != 1)
        {
            return null;
        }*/

        List<SubtitleScript> subtitlesToClear = new List<SubtitleScript>();
        foreach(SubtitleScript subtitle in subtitles2d)
        {
            if (subtitle == null)
            {
                subtitlesToClear.Add(subtitle);
                continue;
            }
            if (subtitle.text == subtitleText)
            {
                RemoveSubtitle(subtitle);
                return;
            }
        }
        foreach(SubtitleScript subtitle in subtitlesToClear)
        {
            subtitles2d.Remove(subtitle);
        }
        subtitlesToClear.Clear();
        foreach(SubtitleScript subtitle in subtitles3d)
        {
            if (subtitle == null)
            {
                subtitlesToClear.Add(subtitle);
                continue;
            }
            if (subtitle.text == subtitleText)
            {
                RemoveSubtitle(subtitle);
                return;
            }
        }
        foreach(SubtitleScript subtitle in subtitlesToClear)
        {
            subtitles3d.Remove(subtitle);
        }
        subtitlesToClear.Clear();
    }

    public void StopChainedSubtitles()
    {
        base.StopAllCoroutines();
    }

    public void ToggleSubtitleVisibility()
    {
        subtitleHud.gameObject.SetActive(!subtitleHud.gameObject.activeSelf);
    }

    public void SetSubtitleVisibility(bool visible)
    {
        subtitleHud.gameObject.SetActive(visible);
    }

    /*public void Hide3DSubtitles(bool visible)
    {
        foreach(SubtitleScript subtitle in subtitles3d)
        {
            subtitle.gameObject.SetActive(visible);
        }
    }

    public void Hide2DSubtitles(bool visible)
    {
        foreach(SubtitleScript subtitle in subtitles2d)
        {
            subtitle.gameObject.SetActive(visible);
        }
    }*/

    private IEnumerator ChainedSubtitleLoop(string[] texts, float[] durations, Color[] colors, Transform producer = null, AudioSource producerAudio = null)
    {
        int ind = -1;
        while (ind < texts.Length)
        {
            ind++;
            if (ind >= texts.Length)
            {
                yield break;
            }
            SubtitleScript subtitle = null;
            if (producer == null)
            {
                subtitle = Add2DSubtitle(texts[ind], durations[ind], colors[ind]);
            }
            else
            {
                subtitle = Add3DSubtitle(texts[ind], durations[ind], colors[ind], producer, producerAudio);
            }
            while (subtitle.duration > 0f)
            {
                yield return null;
            }
        }
        yield break;
    }

    public float localTimeScale = 1f;

    public Transform subtitleHud;

    public Transform cameraTransform;

    public GameObject subtitlePrefab;

    public List<SubtitleScript> subtitles2d = new List<SubtitleScript>();

    public List<SubtitleScript> subtitles3d = new List<SubtitleScript>();

    public List<SubtitleScript> pooledSubtitles = new List<SubtitleScript>();
}
