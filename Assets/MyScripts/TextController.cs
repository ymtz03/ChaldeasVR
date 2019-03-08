using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour {
    // Singleton pattern

    [SerializeField]
    public UnityEngine.UI.Text text;

    static UnityEngine.UI.Text text_static;
    static float time_display;

	// Use this for initialization
    void Start () {
        text_static = text;
    }
	
	// Update is called once per frame
	void Update () {
        time_display -= Time.deltaTime;
        if(time_display < 0){ gameObject.SetActive(false); }
    }

    public static void SetText(string newMessage){
        if(text_static != null){
            text_static.text = newMessage;
            text_static.transform.parent.gameObject.SetActive(true);
            time_display = 3f;
        }
    }
}
