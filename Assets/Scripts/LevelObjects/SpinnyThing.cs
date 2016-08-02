using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpinnyThing : NetworkBehaviour {

    [SyncVar] public int currentSymbol;
    public int numSymbols = 3;

    public void Spin()
    {
        //transform.Rotate(new Vector3(0, 121, 0));

        if (currentSymbol < numSymbols - 1)
        {
            currentSymbol++;
        }
        else
            currentSymbol = 0;

        switch (currentSymbol)
        {
            case 0:
                transform.rotation = Quaternion.Euler(new Vector3(0, -60, 0));
                break;
            case 1:
                transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                break;
            case 2:
                transform.rotation = Quaternion.Euler(new Vector3(0, 60, 0));
                break;
        }
    }
}
