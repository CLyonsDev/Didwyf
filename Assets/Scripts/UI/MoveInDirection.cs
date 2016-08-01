using UnityEngine;
using System.Collections;

public class MoveInDirection : MonoBehaviour {

    public Vector3 speed = Vector3.one;

	// Update is called once per frame
	void FixedUpdate () {
        transform.position += speed * Time.deltaTime;
	}
}
