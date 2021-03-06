using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShoot : ShootWithBullet
{
    [Header("[Shoot Stats]")]
    [SerializeField]
    private CircleShootStat[] shootStats;
    private CircleShootStat shootStat;
    private Stepper<CircleShootStat> statStepper;

    [Header("[Transform]")]
    [SerializeField]
    [Tooltip("총알 생성 위치")]
    private Transform spawnCircleCenterTransform;
    [SerializeField]
    [Tooltip("총알 생성 원 각도 기준 위치")]
    private Transform spawnCircleStandardTransform;
    [SerializeField]

    private Coroutine coroutine;

    private void Awake()
    {
        statStepper = new Stepper<CircleShootStat>(shootStats);
        UpdateShootStat();
    }

    [ContextMenu("Start Shoot")]
    public override void StartShoot()
    {
        if (isShooting) return;

        UpdateShootStat();

        isShooting = true;
        coroutine = StartCoroutine(ShootRoutine());
    }

    [ContextMenu("Stop Shoot")]
    public override void StopShoot()
    {
        if (!isShooting) return;
        if (!shootStat.Breakable) return;
        if (coroutine == null) return;

        isShooting = false;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator ShootRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(shootStat.BulletDeltaTime);
        int count = 0;

        if (shootStat.BulletDeltaTime == 0)
        {
            ShakeCamera();
            PlayAudio();
        }

        while (count < shootStat.BulletCount)
        {
            if (count >= shootStat.BulletIndexAtStart)
            {
                // 기준(시작) 각도 (standardAngle)
                Vector3 directionPoint = mouseDirectionMode ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : spawnCircleStandardTransform.position;
                Vector3 standardDir = directionPoint - spawnCircleCenterTransform.position;
                float standardAngle = Mathf.Atan2(standardDir.y, standardDir.x) * Mathf.Rad2Deg;
                standardAngle = (standardAngle + shootStat.BulletStartAngle + 360) % 360;

                // 생성원 중앙에서 생성 위치쪽을 바라보는 방향 (spawnDir)
                float spawnAngle = (standardAngle + (shootStat.BulletDeltaAngle * count)) % 360;
                float theta = spawnAngle * Mathf.PI / 180;
                Vector3 spawnDir = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));

                // 생성 위치
                Vector3 spawnPosition = spawnCircleCenterTransform.position + (spawnDir * shootStat.SpawnCircleRadius);

                // 총알 생성
                Bullet bullet = ObjectPooler.SpawnFromPool<Bullet>(bulletPrefab.name, spawnPosition);
                bullet.Init(new BulletInitInfo(transform, spawnDir, characterStat, attackLevel));
            }

            count++;

            if (shootStat.BulletDeltaTime > 0 && count < shootStat.BulletCount)
            {
                ShakeCamera();
                PlayAudio();
                yield return wait;
            }
        }

        isShooting = false;
        coroutine = null;
    }

    protected override void UpdateShootStat()
    {
        if (debugMode)
            statStepper = new Stepper<CircleShootStat>(shootStats);

        shootStat = statStepper.GetStep(attackLevel);
    }

    public override void SetCharacterStat(CharacterStat characterStat)
    {
        if (this.characterStat == characterStat) return;

        this.characterStat = characterStat;
        shootStat.SetCharacterStat(characterStat);
    }
}
