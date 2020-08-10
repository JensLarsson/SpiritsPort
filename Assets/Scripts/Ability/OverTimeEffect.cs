using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New OverTimeEffect", menuName = "Ability/OverTimeEffect")]
public class OverTimeEffect : ScriptableObject
{
    [SerializeField] Sprite icon;
    [SerializeField] int TurnsOfEffect = 1;
    [SerializeField] int damagePerTrun = 1;
    [SerializeField] bool snares = false;
    [SerializeField] GameObject tickParticleEffect;

    private void Awake()
    {
        TurnsRemaining = TurnsOfEffect;
    }
    public void OnEndOfTurn(BoardUnitBaseClass unit)
    {
        if (tickParticleEffect != null)
        {
            ParticleEffectFactory.StartEffectOnScene(tickParticleEffect, unit.Position);
        }
        TurnsRemaining--;
        unit.DealDamage(damagePerTrun);
    }

    public void OnStartOfturn(BoardUnitBaseClass unit)
    {
        if (snares)
        {
            unit.SetSnared(true);
        }
    }



    public Sprite Icon => icon;
    public int TurnsRemaining { get; private set; }
}
