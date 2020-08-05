using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] Text text;
    int currentlyDissplayedValue = 0;
    float elapse = 0;
    float lifeTime = 0;
    object key = new object();
    Transform target;
    Coroutine coroutine;
    public void ShowDamage(Transform unit, int damage, float time)
    {
        target = unit;
        lock (key)
        {
            currentlyDissplayedValue += damage;
            text.text = currentlyDissplayedValue.ToString();
            elapse = 0;
            lifeTime = time;
        }
        if (coroutine == null)
        {
            coroutine = StartCoroutine(DestructionTimer());
        }
    }

    IEnumerator DestructionTimer()
    {
        while (elapse < lifeTime)
        {
            if (target != null)
            {
                this.transform.position = target.position;
            }
            elapse += Time.deltaTime;
            yield return null;
        }

        lock (key)
        {
            DamageIndicatorController.Instance.Unsubscribe(target);
            Destroy(this.gameObject);
        }
    }
}
