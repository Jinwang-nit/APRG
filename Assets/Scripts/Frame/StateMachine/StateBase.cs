using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 状态基类
public abstract class StateBase
{
    
    public virtual void Init(IStateMachineOwner owner) // 初始化，状态第一次创建的时候调用
    {

    }

    public virtual void UnInit() { } // 反初始化,销毁状态

    public virtual void Enter() // 进入状态
    {

    }

    public virtual void Exit() // 退出状态
    {

    }

    public virtual void Update()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual void FixedUpdate()
    {

    }
}
