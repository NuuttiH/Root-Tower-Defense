using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerStat { None, Damage, Cooldown, Range }

[CreateAssetMenu(menuName="TowerData")]
public class TowerData : ScriptableObject
{
    
    [Header("Tower settings")]
    [SerializeField] private int _baseDamage = 11;
    [SerializeField] private float _baseAttackCooldown = 2.7f;
    [SerializeField] private float _baseRange = 3.8f;
    
    [Header("Ingame data")]
    public int damage;
    public float attackCooldown;
     public float range;
    
    public float damageMod;
    public float attackCooldownMod;
    public float rangeMod;

    public void Init()
    {
        damage = _baseDamage;
        attackCooldown = _baseAttackCooldown;
        range = _baseRange;
        damageMod = 1f;
        attackCooldownMod = 1f;
        rangeMod = 1f;
    }

    public void Recalculate()
    {
        float modifier = GameManager.BuffModifier;
        damage = (int) (_baseDamage * modifier * damageMod);
        attackCooldown = _baseAttackCooldown * (1f / (modifier * attackCooldownMod));
        range = _baseRange * modifier * attackCooldownMod;
    }

    public void Adjust(TowerStat stat, float modifier)
    {
        switch(stat)
        {
            case TowerStat.Damage:
                damageMod += modifier;
                damage = (int) (damage * damageMod * GameManager.BuffModifier);
                break;
            case TowerStat.Cooldown:
                attackCooldownMod += modifier;
                attackCooldown = _baseAttackCooldown * (1f / (attackCooldownMod * GameManager.BuffModifier));
                break;
            case TowerStat.Range:
                rangeMod += modifier;
                range = _baseRange * attackCooldownMod * GameManager.BuffModifier;
                break;
            default:
                break;
        }
    }
}
