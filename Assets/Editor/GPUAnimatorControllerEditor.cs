using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GPUAnimatiorController))]
public class GPUAnimatorControllerEditor : Editor
{
    GPUAnimatiorController controller;
    bool showAnimtions = true;

    private void OnEnable()
    {
        controller = (GPUAnimatiorController)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();

        showAnimtions = EditorGUILayout.Foldout(showAnimtions, "动画");
        if(showAnimtions)
        {
            for (int i = 0; i < controller.m_AnimInfos.Length; i++)
            {
                var animInfo = controller.m_AnimInfos[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(animInfo.m_AnimName);
                if (GUILayout.Button("播放"))
                {
                    controller.Play(animInfo.m_AnimName);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("speed");
        float speed = EditorGUILayout.FloatField(controller.m_Speed);
        controller.m_Speed = speed;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}
