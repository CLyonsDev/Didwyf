using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

    Vector3 lookPos;
    RaycastHit hit;

    [SerializeField]
    LayerMask LM;

    Camera cam;

    // Use this for initialization
    void Start () {
        cam = GameObject.Find("PlayerCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //Vector3 pos = cam.ScreenToWorldPoint(new Vector3(100, 100, cam.nearClipPlane));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LM))
        {
            //Debug.Log("Hit at " + hit.point);
            lookPos = hit.point;
            Debug.DrawLine(cam.transform.position, hit.point, Color.red);
        }
        Vector3 lookingAt = new Vector3(lookPos.x, transform.position.y, lookPos.z);
        transform.LookAt(lookingAt);
        transform.Rotate(0, -90, 0);
	}
}
