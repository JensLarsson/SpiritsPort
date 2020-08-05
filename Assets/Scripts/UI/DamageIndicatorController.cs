using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorController : MonoBehaviour
{
    [SerializeField] DamageIndicator indicatorPrefab;
    public static DamageIndicatorController Instance;
    Dictionary<Transform, DamageIndicator> activeIndicators = new Dictionary<Transform, DamageIndicator>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void ShowDamage(Transform target, int damage)
    {
        if (activeIndicators.TryGetValue(target, out var indicator))
        {
            indicator.ShowDamage(target, damage, 1);
        }
        else
        {
            DamageIndicator newIndicator = Instantiate(indicatorPrefab);
            newIndicator.ShowDamage(target, damage, 1);
            activeIndicators.Add(target, newIndicator);
            newIndicator.transform.position = target.position;
        }
    }

    public void Unsubscribe(Transform target)
    {
        activeIndicators.Remove(target);
    }
}
