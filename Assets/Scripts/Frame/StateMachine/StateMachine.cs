using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IStateMachineOwner { } // 宿主
public class StateMachine
{
    private IStateMachineOwner owner;

    // 建立一个字典来映射每一种状态的实例，因为只有new出实例这个状态才是真实存在的
    private Dictionary<Type, StateBase> stateDic = new Dictionary<Type, StateBase>(); 
    private StateBase currentState; // 当前状态
    public StateBase CurrentState { get => currentState; }

    public Type CurrentStateType { get => currentState.GetType(); } // 当前状态的类型
    public bool HasState { get => currentState != null; }

    public void Init(IStateMachineOwner owner)
    {
        this.owner = owner;
    }

    // T：新状态
    public bool ChangeState<T>(bool reCurrstate = false) where T: StateBase, new()
    {
        // 状态一致或者不用刷新，就不用切换
        // 刷新就是当前状态进行了一半，需要重新开始
        if (HasState && CurrentStateType == typeof(T) && !reCurrstate) return false;
        
        // 退出当前状态
        if (currentState != null)
        {
            currentState.Exit();
            MonoManager.Instance.RemoveUpdateListener(currentState.Update);
            MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
            MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        }

        // 进入新状态
        currentState = GetState<T>();
        currentState.Enter();
        MonoManager.Instance.AddUpdateListener(currentState.Update);
        MonoManager.Instance.AddLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.AddFixedUpdateListener(currentState.FixedUpdate);

        return true;
    }

    private StateBase GetState<T>() where T: StateBase, new()
    {
        if (!stateDic.TryGetValue(typeof(T), out StateBase state)) // 当前这个状态没有实例
        {
            state = new T();
            state.Init(owner); // 确定状态的宿主是谁
            stateDic.Add(typeof(T), state);
        }
        return state;
    }

    public void Stop()
    {
        currentState.Exit();
        MonoManager.Instance.RemoveUpdateListener(currentState.Update);
        MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        currentState = null;

        foreach (var item in stateDic.Values)
        {
            item.UnInit();
        }

        stateDic.Clear();
    }
}
