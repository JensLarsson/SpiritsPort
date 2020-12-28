using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_BetterSlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] UnityEvent<int> action;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    public void Invoke()
    {
        action.Invoke(Mathf.RoundToInt(slider.value));
    }

}
