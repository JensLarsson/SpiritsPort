using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TRASH_TextGetValueFromSlider : MonoBehaviour
{
    public Slider slider;
    public Text text;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = Mathf.RoundToInt(slider.value).ToString();
    }
}
