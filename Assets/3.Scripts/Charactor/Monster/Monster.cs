using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;

public class Monster : Character
{
    [Header("[Monster]")]
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private CollisionDetector detector;

    private Transform target;

    [Header("[UI]")]
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Transform sliderTransform;

    protected override void Start()
    {
        base.Start();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        detector.AddCollisionDetectAction((transform, _, _) =>
        {
            target = transform;
            weapon.StartTrigger();
            detector.SetActive(false);
        });
    }

    private void Update()
    {
        UpdateUI();

        // 넉백 중        
        if (knockBack.IsKnockBacking)
        {
            if (!agent.isStopped) agent.isStopped = true;
            return;
        }

        // 넉백 중 아님
        if (!knockBack.IsKnockBacking)
        {
            if (agent.isStopped)
                agent.isStopped = false;

            if (movement.enabled)
                movement.enabled = false;
        }

        // 타켓 없을 경우
        if (!target)
        {
            movement.SetMoveDirection(Vector2.zero);
            return;
        }

        agent.speed = characterStat.MoveSpeed;
        agent.SetDestination(target.transform.position);

        // Vector2 moveDir = target.transform.position - transform.position;

        // if (moveDir.sqrMagnitude <= 1)
        // {
        //     movement.SetMoveDirection(Vector2.zero);
        //     return;
        // }

        // movement.SetMoveDirection(moveDir);
    }

    private void UpdateUI()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(sliderTransform.position);
        slider.value = currentHp / (float)characterStat.Health;
    }

    protected override void OnDead()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        detector.SetActive(true);
        currentHp = characterStat.Health;
        target = null;
        UpdateUI();
    }

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(this.gameObject);
    }
}