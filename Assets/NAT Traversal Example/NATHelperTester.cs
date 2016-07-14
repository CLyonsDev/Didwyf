using UnityEngine;
using NATTraversal;
using UnityEngine.Networking;

public class NATHelperTester : MonoBehaviour
{
    public ushort directConnectPort = 7777;
    ulong hostGUID = 0;

    NATHelper natHelper;

    void Awake()
    {
        LogFilter.currentLogLevel = LogFilter.Debug;
        natHelper = GetComponent<NATHelper>();
    }

    void OnGUI()
    {
        if (!natHelper.isConnectedToFacilitator)
        {
            GUI.enabled = false;
        }

        if (!natHelper.isPunchingThrough && !natHelper.isListeningForPunchthrough)
        {
            if (GUI.Button(new Rect(10, 10, 150, 40), "Listen for Punchthrough"))
            {
                Debug.Log("Listening for punchthrough");
                StartCoroutine(natHelper.startListeningForPunchthrough(onHolePunchedServer));
            }
        }
        else if (natHelper.isListeningForPunchthrough)
        {
            if (GUI.Button(new Rect(10, 10, 150, 40), "Stop Listening"))
            {
                natHelper.StopListeningForPunchthrough();
            }
        }

        if (natHelper.isListeningForPunchthrough)
        {
            GUI.Label(new Rect(170, 10, 170, 20), "Host GUID");
            GUI.TextField(new Rect(170, 30, 200, 20), natHelper.guid.ToString());
        }
        else if (!natHelper.isPunchingThrough)
        {
            if (GUI.Button(new Rect(10, 60, 150, 40), "Punchthrough"))
            {
                Debug.Log("Trying to punch through");
                StartCoroutine(natHelper.punchThroughToServer(hostGUID, onHolePunchedClient));
            }

            GUI.Label(new Rect(170, 60, 170, 20), "Host GUID");
            ulong.TryParse(GUI.TextField(new Rect(170, 80, 200, 20), hostGUID.ToString()), out hostGUID);
        }

        if (GUI.Button(new Rect(10, 110, 150, 40), "Forward port"))
        {
            Debug.Log("Forward port: " + directConnectPort);
            natHelper.mapPort(directConnectPort, directConnectPort, Protocol.Both, "NAT Test", onPortMappingDone);
        }
    }

    void onHolePunchedServer(int portToListenOn)
    {
        Debug.Log("Start a server listening on this port: " + portToListenOn);
    }

    void onHolePunchedClient(int serverPort, int clientPort, bool success)
    {
        if (success)
        {
            Debug.Log("Start a socket on " + clientPort + " and connect to the server on " + serverPort);
        }
        else
        {
            Debug.Log("Punchthrough failed.");
        }
    }

    void onPortMappingDone(Open.Nat.Mapping mapping, bool isError)
    {
        if (isError)
        {
            Debug.Log("Port mapping failed");
        }
        else
        {
            Debug.Log("Port " + mapping.PublicPort + " mapped (" + mapping.Protocol + ")");
        }
    }
}
