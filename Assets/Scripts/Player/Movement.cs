using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    float moveSpeed = 4;

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.MovePosition(transform.position + ((Vector3.forward * Input.GetAxis("Vertical")) + (Vector3.right * Input.GetAxis("Horizontal"))) * moveSpeed * Time.deltaTime);
	}
}
