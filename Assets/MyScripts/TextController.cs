using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour {

    public UnityEngine.UI.Text welcomeLabel;
    public string defaultStr;
    int count;

    static string message = "Hello!!";

	// Use this for initialization
	void Start () {
        //defaultStr = welcomeLabel.text;
        count = 0;

        //UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(mainCamera, false);
	}
	
	// Update is called once per frame
	void Update () {
        //mainCamera.transform.Translate(0.0f, 0.0f, 0.01f); 

        if(Input.GetKey(KeyCode.B)){
            //welcomeLabel.text = "B : " + count.ToString();
            welcomeLabel.text = "B : " + Camera.main.transform.forward.x;
            Debug.Log("mainCamera.forward(x,y,z) = (" 
                      + Camera.main.transform.forward.x + ","
                      + Camera.main.transform.forward.y + ","
                      + Camera.main.transform.forward.z + ")");

            count++;
            //mainCamera.transform.Translate(-0.5f, 0f, 0f);
            //mainCamera.transform.position = new Vector3(-0.5f, 1.2f, 0f);
        }else{
            //welcomeLabel.text = defaultStr;
            float num = 1.2345f;
            welcomeLabel.text = "n " + num.ToString("f2");
            count = 0;
        }

        welcomeLabel.text = message;
	}

    public static void SetText(string newMessage){
        message = newMessage;
    }
}
