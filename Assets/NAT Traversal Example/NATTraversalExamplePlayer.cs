using UnityEngine;
using UnityEngine.Networking;

/**
 * Most basic player possible. Use arrow keys to move around.
 */
public class NATTraversalExamplePlayer : NetworkBehaviour
{
    public GameObject otherOb;

    void Update()
    {
        if (!isLocalPlayer) return;

        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow)) dir = Vector3.up;
        else if (Input.GetKey(KeyCode.DownArrow)) dir = Vector3.down;
        else if (Input.GetKey(KeyCode.LeftArrow)) dir = Vector3.left;
        else if (Input.GetKey(KeyCode.RightArrow)) dir = Vector3.right;
        
        transform.position += dir * Time.deltaTime * 5;

        if (otherOb && dir != Vector3.zero)
        {
            otherOb.transform.position = transform.position + dir;
        }
    }

    [ClientRpc]
    public void RpcSetAssignedOb(NetworkInstanceId netID)
    {
        otherOb = ClientScene.FindLocalObject(netID);
    }
}
