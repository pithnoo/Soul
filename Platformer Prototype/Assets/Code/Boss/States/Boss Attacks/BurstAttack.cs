using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstAttack : BossAttackState
{
    public BurstAttack(Boss boss, BossStateMachine stateMachine, BossData bossData, string animBoolName) : base(boss, stateMachine, bossData, animBoolName)
    {
    }
    public override void DoChecks()
    {
        base.DoChecks();
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void FinishAttack()
    {
        base.FinishAttack();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    public override void TriggerAttack()
    {
        base.TriggerAttack();

        SpawnProjectiles(bossData.numberOfProjectiles);
    }

    public void SpawnProjectiles(int numProjectiles)
    {
        float angleStep = 360f / numProjectiles;
        float angle = 0f;

        for (int i = 0; i <= bossData.numberOfProjectiles - 1; i++)
        {

            float projectileDirXposition = boss.transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * bossData.burstRadius;
            float projectileDirYposition = boss.transform.position.y  + Mathf.Cos((angle * Mathf.PI) / 180) * bossData.burstRadius;

            Vector3 projectileVector = new Vector3(projectileDirXposition, projectileDirYposition, 0f);
            Vector3 projectileMoveDirection = (projectileVector - boss.transform.position).normalized * bossData.burstProjectileSpeed;

            var proj = GameObject.Instantiate(bossData.projectile, boss.transform.position, Quaternion.identity);
            proj.transform.Rotate(0, 0, angle);
            proj.GetComponent<Rigidbody2D>().velocity =
                new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);

            angle += angleStep;
        }
    }   
}
