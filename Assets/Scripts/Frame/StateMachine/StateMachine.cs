using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IStateMachineOwner { } // ����
public class StateMachine
{
    private IStateMachineOwner owner;

    // ����һ���ֵ���ӳ��ÿһ��״̬��ʵ������Ϊֻ��new��ʵ�����״̬������ʵ���ڵ�
    private Dictionary<Type, StateBase> stateDic = new Dictionary<Type, StateBase>(); 
    private StateBase currentState; // ��ǰ״̬
    public StateBase CurrentState { get => currentState; }

    public Type CurrentStateType { get => currentState.GetType(); } // ��ǰ״̬������
    public bool HasState { get => currentState != null; }

    public void Init(IStateMachineOwner owner)
    {
        this.owner = owner;
    }

    // T����״̬
    public bool ChangeState<T>(bool reCurrstate = false) where T: StateBase, new()
    {
        // ״̬һ�»��߲���ˢ�£��Ͳ����л�
        // ˢ�¾��ǵ�ǰ״̬������һ�룬��Ҫ���¿�ʼ
        if (HasState && CurrentStateType == typeof(T) && !reCurrstate) return false;
        
        // �˳���ǰ״̬
        if (currentState != null)
        {
            currentState.Exit();
            MonoManager.Instance.RemoveUpdateListener(currentState.Update);
            MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
            MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        }

        // ������״̬
        currentState = GetState<T>();
        currentState.Enter();
        MonoManager.Instance.AddUpdateListener(currentState.Update);
        MonoManager.Instance.AddLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.AddFixedUpdateListener(currentState.FixedUpdate);

        return true;
    }

    private StateBase GetState<T>() where T: StateBase, new()
    {
        if (!stateDic.TryGetValue(typeof(T), out StateBase state)) // ��ǰ���״̬û��ʵ��
        {
            state = new T();
            state.Init(owner); // ȷ��״̬��������˭
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
