using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyInEditor : MonoBehaviour
{
    void Awake()
    {
        Destroy(this.gameObject);        
    }
}
