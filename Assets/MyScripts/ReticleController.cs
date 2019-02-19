using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleController : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var ray = new Ray(Camera.main.transform.position,
                          Camera.main.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20.0f))
        {
            transform.position = hit.point;// + new Vector3(0, 0.1f, 0);

            GetComponent<Renderer>().enabled = true;

            var suic = hit.collider.GetComponent<SliderUIController>();
            if(suic){
                suic.Hit();
            }
            /*
            if (hit.transform.gameObject.name == "Platform1")
            {
                transform.position = hit.point + new Vector3(0, 0.1f, 0);
            }*/
        }else{
            GetComponent<Renderer>().enabled = false;

            transform.position 
                = Camera.main.transform.position
                + Camera.main.transform.forward * 20.0f;
        }
	}
}
