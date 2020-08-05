using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class BetterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Color mouseOverColour = Color.gray;

    Action mouseDownAction = () => { };
    Image image;
    bool mouseOverChage;
    [SerializeField] Text text;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void AddAction(Action action)
    {
        mouseDownAction += action;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDownAction.Invoke();
        if (mouseOverChage) image.color = Color.black;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mouseOverChage) image.color = mouseOverColour;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouseOverChage) image.color = Color.white;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (mouseOverChage) image.color = Color.white;
    }

    public void SetImage(Sprite sprite, bool mouseOverIdicator = true)
    {
        image.sprite = sprite;
        mouseOverChage = mouseOverIdicator;
    }

    public void SetText(string s) => text.text = s;
}
