using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererMiddleman : MonoBehaviour
{
    public Renderer GetMeshRenderer => renderer;
    [SerializeField] Renderer renderer;
}
