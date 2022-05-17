using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Bullet : MonoBehaviour
{
    [Header("[Bullet Stat]")]
    [SerializeField]
    protected BulletStat[] bulletStats;
    protected BulletStat bulletStat;
    protected Stepper<BulletStat> stepper;

    [Header("[Movement]")]
    [SerializeField]
    protected Movement movement;

    protected BulletInitInfo info;

    protected virtual void Awake()
    {
        stepper = new Stepper<BulletStat>(bulletStats);
    }

    protected virtual void Start()
    {
        if (movement)
            movement.SetStat(bulletStat);

        bulletStat = stepper.GetStep(0);
    }

    public virtual void Init(BulletInitInfo info)
    {
        bulletStat = stepper.GetStep(info.attackLevel);
        bulletStat.SetCharacterStat(info.characterStat);
        if (movement)
            movement.SetStat(bulletStat);
        this.info = info;
    }

    protected Vector3 ConvertMoveDirection(Vector2 moveDirection)
    {
        float standardAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        standardAngle = (standardAngle + 360) % 360;

        float newAngle = Random.Range(standardAngle - bulletStat.Scatter, standardAngle + bulletStat.Scatter);
        newAngle = (newAngle + 360) % 360;

        float theta = newAngle * Mathf.PI / 180;
        Vector3 newMoveDir = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));

        return newMoveDir.normalized;
    }

    protected void RotateBullet(Vector2 moveDirection)
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void Update()
    {
        Vector3 pos = transform.position;
        if (pos.y < -200 || pos.y > 200 || pos.x > 200 || pos.x < -200)
            gameObject.SetActive(false);
    }

    protected virtual void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Player -> Monster
        // Monster -> Player
        if (IsEnemy(other))
        {
            OnTriggerEnterEnemy(other);
        }

        // Bullet -> Obstacle
        else if (IsObstacle(other))
        {
            OnTriggerEnterObstacle();
        }
    }

    protected bool IsEnemy(Collider2D other)
    {
        return bulletStat.CharacterStat.CharacterType == CharacterType.Player && other.tag.Equals(MonsterCollision.TAG)
            || bulletStat.CharacterStat.CharacterType == CharacterType.Monster && other.tag.Equals(PlayerCollision.TAG);
    }

    protected bool IsObstacle(Collider2D other)
    {
        return other.CompareTag("Obstacle");
    }

    protected virtual void OnTriggerEnterEnemy(Collider2D enemy)
    {
        CharacterCollision enemyCollision = enemy.GetComponent<CharacterCollision>();
        HitEnemy(enemyCollision);
        gameObject.SetActive(false);
    }

    protected virtual void OnTriggerEnterObstacle()
    {
        HitObstacle();
        gameObject.SetActive(false);
    }

    protected virtual void HitEnemy(CharacterCollision enemy)
    {
        enemy.Hit(
            bulletStat.Attack,
            bulletStat.KnockBack,
            transform.position
        );
    }

    protected virtual void HitObstacle()
    {
    }

    public CharacterType GetOwner() => bulletStat.CharacterStat.CharacterType;
    public Transform GetOwnerTransform() => info.ownerTransform;
}

public class BulletInitInfo
{
    public Transform ownerTransform;
    public Vector3 moveDirection;
    public CharacterStat characterStat;
    public int attackLevel;

    public BulletInitInfo(Transform ownerTransform, Vector3 moveDirection, CharacterStat characterStat, int attackLevel)
    {
        this.ownerTransform = ownerTransform;
        this.moveDirection = moveDirection;
        this.characterStat = characterStat;
        this.attackLevel = attackLevel;
    }
}
