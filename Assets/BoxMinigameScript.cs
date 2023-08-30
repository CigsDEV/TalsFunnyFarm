using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluidMidi;
using FluidSynth;

public class BoxMinigameScript : MonoBehaviour
{
    private GameControllerScript GM;
    private SubtitleManager sm;

    public SongPlayer SchoolPlayer, LearnPlayer;

    [SerializeField]
    private BoxCollider box;
    [SerializeField]
    private GameObject TalTutor;
    [SerializeField]
    private AudioClip TalLol, Congrats;
    
    private Vector3 OriginalBoxPosition;
    [SerializeField]
    private AudioSource Taldio;
    public Vector3 Taleport;

    [Header("Game Stuff")]
    public GameObject Crate, Gate, Fruit;
    public bool Started;


    public void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!Started)
            {
                box.enabled = false;
                StartGame();
                Started = true;
                Gate.SetActive(true);

            }
            else
            {
                ResetBox();
            }
        }

        if (other.gameObject.name == "Crate")
        {
            WinGame();

            Destroy(other.gameObject);
        }
        
    }

    public void StartGame() 
    {
        SchoolPlayer.Pause();
        LearnPlayer.Play();
        Fruit.SetActive(false);
        TalTutor.transform.position = Taleport;
        Taldio.PlayOneShot(TalLol);
        Crate.SetActive(true);
        OriginalBoxPosition = Crate.transform.position;
        SubtitleManager sm = FindObjectOfType<SubtitleManager>();
        sm.Add3DSubtitle("Nuh uh uh! You can't just take fruit without working for it, Complete this puzzle, and you'll get what you want!", TalLol.length, new Color32(242, 151, 31, 255), TalTutor.transform, TalTutor.GetComponent<AudioSource>());
    }

    public void WinGame()
    {
        LearnPlayer.Stop();
        SchoolPlayer.Resume();
        Fruit.SetActive(true);
        Gate.SetActive(false);
        Taldio.PlayOneShot(Congrats);
        Started = true;
    }

    public void ResetBox()
    {
        Crate.transform.position = OriginalBoxPosition;
    }
}
