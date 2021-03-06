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
    protected bool debugMode = false;
    [SerializeField]
    protected int attackLevel = 0;

    [Header("Sound")]
    [SerializeField]
    private AudioSource shootAudio;

    public abstract void StartShoot();
    public abstract void StopShoot();

    public abstract void SetCharacterStat(CharacterStat characterStat);

    protected abstract void UpdateShootStat();

    public int GetAttackLevel() => attackLevel;

    public virtual void SetAttackLevel(int level)
    {
        attackLevel = level;
    }

    protected bool mouseDirectionMode = false;
    public void ActiveMouseDirectionMode(bool enable)
    {
        mouseDirectionMode = enable;
    }

    public void SetDebugMode(bool active)
    {
        debugMode = active;
    }

    protected void PlayAudio()
    {
        if (shootAudio == null) return;
        shootAudio.PlayOneShot(shootAudio.clip);
    }

    protected void ShakeCamera()
    {
        if (characterStat.CharacterType == CharacterType.Player)
            TargetCamera.Shake(0.01f, 0.01f);
    }
}
