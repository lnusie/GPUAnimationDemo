using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimationTransform : MonoBehaviour
{
    public Animator m_Animator;
    public GPUAnimatiorController m_GPUAnimatiorController;
    public AnimationTransformInfo m_AnimationTransformInfo;


    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            m_GPUAnimatiorController.Play(m_AnimationTransformInfo);
            m_Animator.SetTrigger("DashAttack");
        }
    }
}
