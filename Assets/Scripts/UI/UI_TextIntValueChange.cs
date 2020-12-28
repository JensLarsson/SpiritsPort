using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TextIntValueChange : MonoBehaviour
{
    [SerializeField] Text text;
    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }
    }
    public void ChangeValue(int val)
    {
        text.text = val.ToString();
    }
}
