using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 1;
    public virtual void InitiateProjectile(Vector2 startPos, Vector2 endPos, Action onHitAction)
    {
        StartCoroutine(ProjectileTravel(startPos, endPos, onHitAction));
    }

    IEnumerator ProjectileTravel(Vector2 startPos, Vector2 endPos, Action onHitAction)
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
}
