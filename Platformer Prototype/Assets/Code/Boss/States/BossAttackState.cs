using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : BossState
{
    protected bool isAnimationFinished;

    public BossAttackState(Boss boss, BossStateMachine stateMachine, BossData bossData, string animBoolName) : base(boss, stateMachine, bossData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }
    public override void Enter()
    {
        base.Enter();
        isAnimationFinished = false;
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    public virtual void TriggerAttack(){

    }

    public virtual void FinishAttack(){
        isAnimationFinished = true;
    }
}
