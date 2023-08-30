using System.Collections;
using UnityEngine;
using FluidMidi;

public class GameActivator : MonoBehaviour
{
    public Camera myOrthoCamera; // Drag and drop your orthographic camera here in the inspector
    public float lerpDuration = 2.0f; // Duration over which the lerp happens, 2 seconds by default

    public SongPlayer OutsideMusic, SchoolMusic;
    public GameObject TalTutor;
    public SwingingDoorScript SDS;
    public bool Active;

    public void OnTriggerEnter(Collider other) 
    {
        if (!Active)
        {
            OutsideMusic.Stop();
            SchoolMusic.Play();
            SDS.NBNeeded = 9999;
            TalTutor.SetActive(true);
            Active = true;
            SDS.requirementMet = false;
            SDS.trigger.isTrigger = false;
            SubtitleManager sm = FindObjectOfType<SubtitleManager>();
            StartCoroutine(LerpCameraSize(0.1f, 5f)); // Start the lerp
            sm.Add3DSubtitle("Oh, Hey there! welcome to my farm!", TalTutor.GetComponent<AudioSource>().clip.length, new Color32(242, 151, 31, 255), TalTutor.transform, TalTutor.GetComponent<AudioSource>());
        }
    }

    IEnumerator LerpCameraSize(float startSize, float endSize)
    {
        float elapsedTime = 0;

        while (elapsedTime < lerpDuration)
        {
            myOrthoCamera.orthographicSize = Mathf.Lerp(startSize, endSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        myOrthoCamera.orthographicSize = endSize; // Ensure it ends at the desired value
    }
}
