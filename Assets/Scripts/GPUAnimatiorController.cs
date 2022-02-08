using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class GPUAnimatiorController : MonoBehaviour
{
    public AnimationInfo[] m_AnimInfos;
    public float m_Speed = 1.0f; 

    MaterialPropertyBlock m_MaterialPropertyBlock;
    MeshRenderer m_MeshRenderer;
    AnimationInfo m_CurAnimInfo;
    Dictionary<string, AnimationInfo> m_AnimInfoDict;
    float m_PlayTime0 = 0;
    float m_PlayTime1 = 0;
    float m_BlendFrames = 0;
    bool m_Inited = false;
    AnimationTransformInfo m_TransformInfo;

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
            m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock, 0);
            float boundMax = float.MinValue;
            float boundMin = float.MaxValue;
            var meshFilter = GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            foreach (var vertex in mesh.vertices)
            {
                boundMin = Mathf.Min(boundMin, vertex.x, vertex.y, vertex.z);
                boundMax = Mathf.Max(boundMax, vertex.x, vertex.y, vertex.z);
            }
            m_MaterialPropertyBlock.SetFloat("_BoundMax0", boundMax);
            m_MaterialPropertyBlock.SetFloat("_BoundMin0", boundMin);
            m_MaterialPropertyBlock.SetFloat("_BlendFactor", 0);
            m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock, 0);
        }
    }

    private void Start()
    {
        Init();
        PlayDefaultAnimation();
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        if (m_TransformInfo == null)
        {
            float t = 0;
            if (m_CurAnimInfo.m_Loop)
            {
                t = (m_PlayTime0 % m_CurAnimInfo.m_AnimLength) / m_CurAnimInfo.m_AnimLength;//当前动画播到了第几帧
            }
            else
            {
                t = m_PlayTime0 / m_CurAnimInfo.m_AnimLength;
                t = Mathf.Min(1, t);
            }
            float index = (t * m_CurAnimInfo.m_AnimFrameNum + m_CurAnimInfo.m_AnimFrameOffset);//定位到贴图中的Y坐标
            float curFrameIndex = (index / m_CurAnimInfo.m_TotalFrames);//将Y坐标映射进01区间
            m_PlayTime0 += (Time.deltaTime * m_Speed);
            m_MaterialPropertyBlock.SetFloat("_FrameIndex0", curFrameIndex);
            m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock, 0);
        }
        else
        {
            var animInfo0 = m_TransformInfo.m_AnimInfo0;
            var animInfo1 = m_TransformInfo.m_AnimInfo1;

            float t0 = m_PlayTime0 / animInfo0.m_AnimLength;//当前动画播到了第几帧
            float frameNumber = t0 * animInfo0.m_AnimFrameNum;
            float index0 = (frameNumber + animInfo0.m_AnimFrameOffset);
            float frameIndex0 = (index0 / animInfo0.m_TotalFrames);//将Y坐标映射进01区间
            m_MaterialPropertyBlock.SetFloat("_FrameIndex0", frameIndex0);
            float blendFactor = 0;
            if (frameNumber >= m_TransformInfo.m_StartFrame) //开始混入第二动画
            {
                float t1 = m_PlayTime1 / animInfo1.m_AnimLength;
                float index1 = (t1 * animInfo1.m_AnimFrameNum + animInfo1.m_AnimFrameOffset);
                float frameIndex1 = (index1 / animInfo1.m_TotalFrames);
                m_MaterialPropertyBlock.SetFloat("_FrameIndex1", frameIndex1);
                m_PlayTime1 += Time.deltaTime * m_Speed;
            }
            blendFactor = (frameNumber - m_TransformInfo.m_StartFrame) / m_TransformInfo.m_BlendFrame;
            blendFactor = Mathf.Min(1, blendFactor);
            blendFactor = Mathf.Max(0, blendFactor);

            m_PlayTime0 += Time.deltaTime * m_Speed;
            m_MaterialPropertyBlock.SetFloat("_BlendFactor", blendFactor);
            m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock, 0);
            if (m_PlayTime1 > animInfo1.m_AnimLength) //混合结束
            {
                m_TransformInfo = null;
                var playTime = m_PlayTime0;
                Play(animInfo1.m_AnimName);
                if (!animInfo1.m_Loop)
                {
                    m_PlayTime0 = playTime; 
                }
            }
        }
    }

    void Init()
    {
        if (!m_Inited)
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
            m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock, 0);
            m_Inited = true;
        }
    }

    public void Play(AnimationTransformInfo transformInfo)
    {
        m_MaterialPropertyBlock.SetFloat("_BoundMax0", transformInfo.m_AnimInfo0.m_boundMax);
        m_MaterialPropertyBlock.SetFloat("_BoundMin0", transformInfo.m_AnimInfo0.m_boundMin);

        m_MaterialPropertyBlock.SetFloat("_BoundMax1", transformInfo.m_AnimInfo1.m_boundMax);
        m_MaterialPropertyBlock.SetFloat("_BoundMin1", transformInfo.m_AnimInfo1.m_boundMin);

        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock, 0);

        m_TransformInfo = transformInfo;
        m_CurAnimInfo = null;
        m_PlayTime0 = 0;
        m_PlayTime1 = 0;
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
        m_MaterialPropertyBlock.SetFloat("_BoundMax0", animInfo.m_boundMax);
        m_MaterialPropertyBlock.SetFloat("_BoundMin0", animInfo.m_boundMin);
        m_MaterialPropertyBlock.SetFloat("_BlendFactor", 0);
        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock, 0);
        m_PlayTime0 = 0; 
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

    void PlayDefaultAnimation()
    {
        for (int i = 0; i < m_AnimInfos.Length; i++)
        {
            if(m_AnimInfos[i].m_IsDefault)
            {
                Play(m_AnimInfos[i].m_AnimName);
                break;
            }
        }
    }
}
