using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour {

    [HideInInspector]
    public Image image;
    float tmpValue = 0;
    public float seconds;

	// Use this for initialization
	void Start () {
        image = this.GetComponent<Image>();
        image.color = new Vector4(Color.black.r, Color.black.g, Color.black.b, 0);

    }

    public void ChangeAlpha() {
        //image.color.a = alpha;

        tmpValue += (255 / seconds) * Time.deltaTime * 60;

        image.color = new Vector4(Color.black.r, Color.black.g, Color.black.b, tmpValue);
    }

    // Update is called once per frame
    void Update () {
	    
	}
}
