using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.magenta;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.alertRadius);

        Handles.color = Color.yellow;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.autoDetectRadius);

        Handles.color = Color.red;

        foreach(Transform visibleTarget in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.transform.position);
        }
    }
}

[CustomEditor (typeof(EnemyAI))]
public class RangeDisplayers : Editor
{
    void OnSceneGUI()
    {
        EnemyAI ai = (EnemyAI)target;

        Handles.color = Color.blue;
        Handles.DrawWireArc(ai.transform.position, Vector3.up, Vector3.forward, 360, ai.attackRange);
    }
}
