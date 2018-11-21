using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StateType
{
    Vertigo,        //眩晕
    DOT,            //DOT伤害
    Decelerate,     //减速状态
    //Temporary,      //临时属性状态，有属性增加的问题，暂时不做
    PGCD,           //普攻CD状态
    S1CD,           //一技能CD状态
    S2CD,           //二技能CD状态
    S3CD,           //三技能CD状态
    SXCD,           //闪现CD状态
    GZCD,           //钩子CD状态
}

public class State  {

    public string name;
    public StateType stateType;
    public float totalTime;//总共时长
    public float remainTime;//剩余时长

    public float deceleratePercent;//减速百分比

    public float dotNum;//DOT伤害数值

    public int plusAttackNum;//临时攻击力
    public float plusDamageReduct;//临时伤害减免
    public float plusAttackSpeed;//临时攻速

    public static State CreatVertigo(string name,float time)//创建眩晕状态
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.Vertigo;
        state.totalTime = time;
        state.remainTime = state.totalTime;

        return state;
    }

    public static State CreatDecelerate(string name, float dePercent, float time)//创建减速状态
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.Decelerate;
        state.deceleratePercent = dePercent;
        state.totalTime = time;
        state.remainTime = state.totalTime;

        return state;
    }

    public static State CreatDOT(string name,float dotNum ,float time)//创建DOT状态
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.DOT;
        state.dotNum = dotNum;
        state.totalTime = time;
        state.remainTime = state.totalTime;

        return state;
    }

    //public static State CreatTemporary(string name, int plusAttackNum, float plusDamageReduct, float plusAttackSpeed,float time)//创建临时增益状态状态
    //{
    //    State state = new State();
    //    state.name = name;
    //    state.stateType = StateType.Temporary;
    //    state.plusAttackNum = plusAttackNum;
    //    state.plusDamageReduct = plusDamageReduct;
    //    state.plusAttackSpeed = plusAttackSpeed;
    //    state.totalTime = time;
    //    state.remainTime = state.totalTime;

    //    return state;
    //}






    public static State CreatPGCD(string name, float time)
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.PGCD;
        state.totalTime = time;
        state.remainTime = state.totalTime;
        return state;
    }
    public static State CreatS1CD(string name, float time)
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.S1CD;
        state.totalTime = time;
        state.remainTime = state.totalTime;
        return state;
    }
    public static State CreatS2CD(string name, float time)
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.S2CD;
        state.totalTime = time;
        state.remainTime = state.totalTime;
        return state;
    }
    public static State CreatS3CD(string name, float time)
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.S3CD;
        state.totalTime = time;
        state.remainTime = state.totalTime;
        return state;
    }
    public static State CreatSXCD(string name, float time)
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.SXCD;
        state.totalTime = time;
        state.remainTime = state.totalTime;
        return state;
    }
    public static State CreatGZCD(string name, float time)
    {
        State state = new State();
        state.name = name;
        state.stateType = StateType.GZCD;
        state.totalTime = time;
        state.remainTime = state.totalTime;
        return state;
    }


    /// <summary>
    /// 给单位状态列表添加状态
    /// </summary>
    /// <param name="u"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public static Unit AddState(Unit u,State state)
    {
        u.states.Add(state);
        return u;
    }



    public static State CopyState(State original)
    {
        State newState = new State();
        newState.name = original.name;
        newState.stateType = original.stateType;
        newState.totalTime = original.totalTime;
        newState.remainTime = original.remainTime;
        newState.deceleratePercent = original.deceleratePercent;
        newState.dotNum = original.dotNum;
        newState.plusAttackNum = original.plusAttackNum;
        newState.plusDamageReduct = original.plusDamageReduct;
        newState.plusAttackSpeed = original.plusAttackSpeed;

        return newState;
    }











}
