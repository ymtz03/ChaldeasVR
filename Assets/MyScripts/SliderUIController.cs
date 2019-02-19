using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderUIController : MonoBehaviour {

    [SerializeField]
    float SecondToFill = 1.0f;

    [SerializeField]
    UnityEngine.Events.UnityEvent m_event;

    bool IsHit;
    float GainPerSecond;

    UnityEngine.UI.Slider Slider;

	// Use this for initialization
	void Start () {
        IsHit = false;
        Slider = GetComponent<UnityEngine.UI.Slider>();
        GainPerSecond = Slider.maxValue / SecondToFill;
	}
	
	// Update is called once per frame
	void Update () {
        if (Slider.value >= Slider.maxValue) {
            Slider.value = 0f;
            m_event.Invoke();
        }

        if(IsHit){
            Slider.value += GainPerSecond * Time.deltaTime;
            IsHit = false;
        }else{
            Slider.value = 0f;
        }
	}

    public void Hit()
    {
        IsHit = true;
    }

}
