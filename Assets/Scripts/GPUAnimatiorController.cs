using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUAnimatiorController : MonoBehaviour
{
    public AnimationInfo[] m_AnimInfos;
    public float m_Speed = 1.0f;

    MaterialPropertyBlock m_MaterialPropertyBlock;
    MeshRenderer m_MeshRenderer;
    AnimationInfo m_CurAnimInfo;
    Dictionary<string, AnimationInfo> m_AnimInfoDict;
    float m_PlayTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock, 0);
        SetDefaultAnimInfo();
    }

    // Update is called once per frame
    void Update()
    {
        float t = (m_PlayTime % m_CurAnimInfo.m_AnimLength) / m_CurAnimInfo.m_AnimLength;
        float index = (t * m_CurAnimInfo.m_AnimFrameNum + m_CurAnimInfo.m_AnimFrameOffset);
        float curFrameIndex = (index / m_CurAnimInfo.m_TotalFrames);
        m_PlayTime += (Time.deltaTime * m_Speed);

        m_MaterialPropertyBlock.SetFloat("_CurFrameIndex", curFrameIndex);
        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock, 0);
    }

    public void Play(string animName)
    {
        if(m_CurAnimInfo != null && m_CurAnimInfo.m_AnimName == animName)
        {
            return;
        }

        AnimationInfo animInfo = GetAnimInfo(animName);
        if(animInfo == null)
        {
            return;
        }

        m_CurAnimInfo = animInfo;
        m_MaterialPropertyBlock.SetFloat("_BoundMax", animInfo.m_boundMax);
        m_MaterialPropertyBlock.SetFloat("_BoundMin", animInfo.m_boundMin);
        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock, 0);
        m_PlayTime = 0;
    }

    AnimationInfo GetAnimInfo(string animName)
    {
        if(m_AnimInfoDict == null)
        {
            m_AnimInfoDict = new Dictionary<string, AnimationInfo>();
            for (int i = 0; i < m_AnimInfos.Length; i++)
            {
                m_AnimInfoDict.Add(m_AnimInfos[i].m_AnimName, m_AnimInfos[i]);
            }
        }

        AnimationInfo animInfo = null;
        m_AnimInfoDict.TryGetValue(animName, out animInfo);
        return animInfo;
    }

    void SetDefaultAnimInfo()
    {
        for (int i = 0; i < m_AnimInfos.Length; i++)
        {
            if(m_AnimInfos[i].m_IsDefault)
            {
                m_CurAnimInfo = m_AnimInfos[i];
                break;
            }
        }
    }
}
