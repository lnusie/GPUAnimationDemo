using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationTransitionConditionType
{
    Trigger,
    Bool, 
    Int,
    Float
}

public enum AnimationTransitionConditionCompareType
{
    Greater,
    Less,
    Equal,
    NoEqual,
}

[Serializable]
public class AnimationTransitionCondition
{
    public AnimationTransitionConditionType m_Type;
    public AnimationTransitionConditionCompareType m_CompareType;
    public string m_ParamName;
    public int m_IntValue;
    public float m_FloatValue;
    public bool m_BoolValue;
}

[CreateAssetMenu]
public class AnimationTransition : ScriptableObject
{
    public AnimationInfo m_AnimInfo0;
    public AnimationInfo m_AnimInfo1;

    public int m_StartFrame; //从Anim0的N帧开始融合
    public int m_BlendFrame;  //融合帧数

    public AnimationTransitionCondition[] m_Conditions;

}
