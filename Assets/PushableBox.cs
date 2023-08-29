using UnityEngine;

public class PushableBox : MonoBehaviour
{
    public float pushStrength = 2f;
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            Vector3 pushDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            rb.AddForce(pushDirection * pushStrength, ForceMode.Force);
        }
    }
}
