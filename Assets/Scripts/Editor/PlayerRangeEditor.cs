using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CharacterBase))]
public class PlayerRangeEditor : Editor {

	void OnSceneGUI()
    {
        CharacterBase player = (CharacterBase)target; 

        Handles.color = new Color32(255, 88, 88, 255);
        Handles.DrawWireArc(player.transform.position, Vector3.up, Vector3.forward, 360, player.weaponRange);

        Handles.color = new Color32(219, 0, 110, 255);
        Handles.DrawWireArc(player.transform.position, Vector3.up, Vector3.forward, 360, player.grabRange);
    }
}
