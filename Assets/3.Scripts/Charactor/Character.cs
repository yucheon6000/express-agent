using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("[Character Stat]")]
    [SerializeField]
    protected CharacterStat characterStat;
    public CharacterStat CharacterStat => characterStat;

    [SerializeField]
    protected int currentHp;
    public int CurrentHp => currentHp;

    protected bool isDead = false;
    public bool IsDead => isDead;

    [Header("[Movement]")]
    [SerializeField]
    protected Movement movement;

    [Header("[Knock Back]")]
    [SerializeField]
    protected KnockBack knockBack;
    [SerializeField]
    protected Transform knockBackPivot;

    [Header("[Weapon]")]
    [SerializeField]
    protected Weapon weapon;

    public virtual Vector3 TargetPosition => transform.position;

    protected virtual void Awake()
    {
        currentHp = characterStat.Health;
        knockBack = GetComponent<KnockBack>();
    }

    protected virtual void Start()
    {
        movement.SetStat(characterStat);
        weapon?.SetCharacterStat(CharacterStat);
    }

    public virtual void Hit(float attack, float knockBack, Vector3 hitPosition)
    {
        IncreaseHp(-attack);
        KnockBack(hitPosition, knockBack);
    }

    protected virtual void KnockBack(Vector2 hitPosition, float knockBack)
    {
        if (!this.knockBack || characterStat.KnockBackResistance >= 1) return;

        float force = knockBack - (knockBack * characterStat.KnockBackResistance);
        force = Mathf.Max(force, 0);

        if (force == 0) return;

        Vector2 dir = knockBackPivot ? (Vector2)knockBackPivot.position - hitPosition : (Vector2)transform.position - hitPosition;

        this.knockBack.StartKnockBack(dir, force);
    }

    public virtual void IncreaseHp(float amount)
    {
        currentHp += (int)amount;
        if (currentHp <= 0) OnDead();
    }

    protected abstract void OnDead();
}
