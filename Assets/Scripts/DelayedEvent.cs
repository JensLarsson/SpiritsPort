using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    [SerializeField] float delay = 0;
    [SerializeField] UnityEvent action;
    void Start()
    {
        ActionDelayer.RunAfterDelay(delay, () => { action.Invoke(); });
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
