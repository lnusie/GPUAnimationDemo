using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationTransferConditionType
{
    Trigger,
    Bool,
    Int,
    Float
}

public enum AnimationTransferConditionCompareType
{
    Greater,
    Less,
    Equal,
    NoEqual,
}

[Serializable]
public class AnimationTransferCondition
{
    public AnimationTransferConditionType m_Type;
    public AnimationTransferConditionCompareType m_CompareType;
    public string m_FieldName;
    public int m_IntValue;
    public float m_FloatValue;
    public bool m_BoolValue;
}

[CreateAssetMenu]
public class AnimationTransformInfo : ScriptableObject
{
    public AnimationInfo m_AnimInfo0;
    public AnimationInfo m_AnimInfo1;

    public int m_StartFrame; //从Anim0的N帧开始融合
    public int m_BlendFrame;  //融合帧数

    public AnimationTransferCondition[] m_Conditions;

}
