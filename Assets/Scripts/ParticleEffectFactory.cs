using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectFactory : MonoBehaviour
{
    public static void StartEffectOnScene(GameObject effect, Vector2 position)
    {
        Instantiate(effect, position, Quaternion.identity);
    }
}
