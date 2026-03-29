using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableItem : MonoBehaviour
{
    public float weight = 10f; 
    public bool isBeingCarried = false;
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdParent)
    {
        isBeingCarried = true;
        rb.isKinematic = true;
        col.enabled = false;
        transform.SetParent(holdParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        isBeingCarried = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
        rb.AddForce(transform.forward * 2f, ForceMode.Impulse); // Optional toss forward
    }
}
