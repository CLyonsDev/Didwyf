using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {

    public float delay = 1.0f;
 
    public bool fade = false;
    public float fadeDelay = 0f;

    // Use this for initialization
    void Start () {
        Destroy(gameObject, delay);

        if (fade)
            StartCoroutine(FadeOut());
            FadeOut();
	}

    void Update()
    {
        if(fade)
        {
            //GetComponent<Text>().color = new Color(GetComponent<Text>().color.r, GetComponent<Text>().color.g, GetComponent<Text>().color.b, GetComponent<Text>().color.a * )
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeDelay);
        GetComponent<CanvasRenderer>().SetAlpha(1f);
        GetComponent<Graphic>().CrossFadeAlpha(0f, delay - fadeDelay, false);
    }
}
