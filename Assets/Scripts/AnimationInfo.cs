using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AnimationInfo : ScriptableObject
{
    public bool m_IsDefault = false;
    public string m_AnimName;
    public float m_boundMin = 0;
    public float m_boundMax = 0;
    public Texture2D m_posTex;
    public bool m_Loop;
    public float m_AnimLength;
    public int m_TotalFrames = 0;
    public int m_AnimFrameNum = 0;
    public int m_AnimFrameOffset = 0;
    public AnimationTransformInfo[] m_AnimationTransformInfos;

}
