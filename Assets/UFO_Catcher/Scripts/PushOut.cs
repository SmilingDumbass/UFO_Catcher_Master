using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushOut : MonoBehaviour
{
    public float pushForce;

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.transform.GetComponent<Rigidbody>();
        Vector3 direction = transform.forward;
        direction.y = 0;
        direction = direction.normalized;
        rb.AddForce(direction * pushForce);
    }
}
