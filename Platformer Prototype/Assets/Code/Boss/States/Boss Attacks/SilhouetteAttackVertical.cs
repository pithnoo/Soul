using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilhouetteAttackVertical : BossAttackState
{
    public SilhouetteAttackVertical(Boss boss, BossStateMachine stateMachine, BossData bossData, string animBoolName, Transform attackPosition) : base(boss, stateMachine, bossData, animBoolName, attackPosition)
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
        boss.transform.position = boss.escapePoint.transform.position;
        boss.SpawnGameObject("vLauncherR", boss.vertical2);
        boss.SpawnGameObject("vLauncherL", boss.vertical1);
    }
}
