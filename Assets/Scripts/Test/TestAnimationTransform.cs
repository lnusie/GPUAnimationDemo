using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimationTransform : MonoBehaviour
{
    public Animator m_Animator;
    public GPUAnimatiorController m_GPUAnimatiorController;
    public AnimationTransition m_AnimationTransformInfo;


    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_GPUAnimatiorController.SetTrigger("Attack");
            m_Animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.D)) 
        {
            m_GPUAnimatiorController.SetTrigger("Dash");
            m_Animator.SetTrigger("Dash");
        }
    }
}
