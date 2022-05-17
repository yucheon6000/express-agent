using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shoot : MonoBehaviour, NeedCharacterStat
{
    protected CharacterStat characterStat = CharacterStat.Default;

    protected bool isShooting = false;
    public bool IsShooting => isShooting;

    [Header("[Attack Level (Debug)]")]
    [SerializeField]
    protected int attackLevel = 0;

    public abstract void StartShoot();
    public abstract void StopShoot();

    public abstract void SetCharacterStat(CharacterStat characterStat);

    protected abstract void UpdateShootStat();

    public int GetAttackLevel() => attackLevel;

    public virtual void SetAttackLevel(int level)
    {
        attackLevel = level;
    }
}
