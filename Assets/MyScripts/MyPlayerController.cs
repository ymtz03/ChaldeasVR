using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : MonoBehaviour {

    float defaultMyHeight;
    const float speed = 0.2f;

    float rot_yaw, rot_pitch, rot_roll;

	// Use this for initialization
	void Start () {
        defaultMyHeight = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.C))
        {
            transform.Translate(Camera.main.transform.forward*speed);
            Vector3 pos = transform.position;
            pos.y = defaultMyHeight;
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate( speed, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(-speed, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0f, 0f,  speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0f, 0f, -speed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(0f,  speed, 0f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(0f, -speed, 0f);
        }

        if (Input.GetKey(KeyCode.J)){
            rot_yaw -= 1.0f;
        }
        if (Input.GetKey(KeyCode.L)){
            rot_yaw += 1.0f;
        }
        if (Input.GetKey(KeyCode.I)){
            rot_pitch -= 1.0f;
        }
        if (Input.GetKey(KeyCode.K)){
            rot_pitch += 1.0f;
        }
        if (Input.GetKey(KeyCode.O)){
            rot_roll -= 1.0f;
        }
        if (Input.GetKey(KeyCode.U)){
            rot_roll += 1.0f;
        }

        transform.rotation = Quaternion.Euler(0f, rot_yaw, 0f);
        transform.rotation *= Quaternion.Euler(rot_pitch, 0f, 0f);
        transform.rotation *= Quaternion.Euler(0f, 0f, rot_roll);
	}
}
