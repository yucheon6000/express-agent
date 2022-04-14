using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, NeedCharacterStat
{
    private CharacterStat characterStat = CharacterStat.Default;

    [Header("[Attack Stat]")]
    [SerializeField]
    private AttackStat attackStat = AttackStat.Default;

    [Header("[Shoot]")]
    [SerializeField]
    private Shoot shoot;

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;
    private Coroutine coroutine;

    [ContextMenu("Start Attack")]
    public void StartAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        coroutine = StartCoroutine(AttackRoutine());
    }

    [ContextMenu("Stop Attack")]
    public void StopAttack()
    {
        if (!isAttacking) return;
        if (coroutine == null) return;

        isAttacking = false;
        shoot.StopShoot();
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator AttackRoutine()
    {
        bool firstShoot = true;
        bool waited = false;

        yield return new WaitForSeconds(attackStat.ShootDelayTimeAtStart);

        while (true)
        {
            // Shoot이 진행 중이면 한 프레임 쉼
            if (shoot.IsShooting)
            {
                yield return null;
                waited = true;
                continue;
            }

            if (attackStat.ShootDeltaTime > 0 && firstShoot == false)
                yield return new WaitForSeconds(attackStat.ShootDeltaTime);

            // 해당 분기가 없으면 시작과 동시에 끝나는 Shoot일 경우, 무한루프 문제 발생
            // 최소 한 프레임은 쉬어줘야 함
            else if (attackStat.ShootDeltaTime == 0 && waited == false && firstShoot == false)
            {
                yield return null;
                waited = true;
            }

            shoot.StartShoot();
            firstShoot = false;
            waited = false;
        }
    }

    public void SetCharacterStat(CharacterStat characterStat)
    {
        if (this.characterStat == characterStat) return;

        this.characterStat = characterStat;
        attackStat.SetCharacterStat(characterStat);
        shoot.SetCharacterStat(characterStat);
    }
}
