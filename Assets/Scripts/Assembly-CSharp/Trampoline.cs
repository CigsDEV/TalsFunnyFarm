using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trampoline : MonoBehaviour
{
    private SubtitleManager sm;
    private AudioSource MyAudio;

    [Tooltip("Strength of the upward bounce when the player hits the trampoline.")]
    public float bounceStrength = 10f; // You can adjust this in the inspector for different bounce effects.

    private void Start() {
        sm = FindObjectOfType<SubtitleManager>();
        MyAudio = base.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Gravity gravityComponent = other.GetComponent<Gravity>();
        if (gravityComponent)
        {
            gravityComponent.SetYVelocity(bounceStrength);
            MyAudio.Play();
            sm.Add3DSubtitle("*BOING*", MyAudio.clip.length, Color.red, base.transform, MyAudio);
            
        }
    }
}
