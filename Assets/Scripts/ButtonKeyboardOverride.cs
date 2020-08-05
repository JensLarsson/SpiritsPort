using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonKeyboardOverride : MonoBehaviour
{
    [SerializeField] KeyCode key;
    Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            button.onClick.Invoke();
        }
    }
}
