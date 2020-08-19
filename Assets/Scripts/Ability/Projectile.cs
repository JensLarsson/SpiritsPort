using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 1;
    public virtual void LinearTravel(Vector2 startPos, Vector2 endPos, Action onHitAction)
    {
        StartCoroutine(ProjectileTravelLinear(startPos, endPos, onHitAction));
    }
    public virtual void ArchTravel(Vector2 startPos, Vector2 endPos, Action onHitAction)
    {
        StartCoroutine(ProjectileTravelArch(startPos, endPos, onHitAction));
    }

    IEnumerator ProjectileTravelLinear(Vector2 startPos, Vector2 endPos, Action onHitAction)
    {
        float travelTime = Vector2.Distance(startPos, endPos) / projectileSpeed;
        float time = 0;
        transform.right = endPos - startPos;
        while (time < travelTime)
        {
            this.transform.position = Vector2.Lerp(startPos, endPos, time / travelTime);

            time += Time.deltaTime;
            yield return null;
        }
        onHitAction.Invoke();
        Destroy(this.gameObject);
    }
    IEnumerator ProjectileTravelArch(Vector2 startPos, Vector2 endPos, Action onHitAction)
    {
        float travelTime = Vector2.Distance(startPos, endPos) / projectileSpeed;
        float zPos = this.transform.position.z;

        float time = 0;
        transform.right = endPos - startPos;
        while (time < travelTime)
        {
            float f = time / travelTime;
            Vector3 pos = Vector2.Lerp(startPos, endPos, f);
            pos.z = zPos - Mathf.Sin(Mathf.PI * f);
            this.transform.position = pos;
            
            time += Time.deltaTime;
            yield return null;
        }
        onHitAction.Invoke();
        Destroy(this.gameObject);
    }

}
