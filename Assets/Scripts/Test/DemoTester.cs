using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoTester : MonoBehaviour
{
    public GameObject m_NormalPrefab;
    public GameObject m_GPUAnimationPrefab;

    public Button m_Btn1;
    public Button m_Btn2;

    public int m_Col = 10;
    public int m_Row = 10;

    private List<GameObject> m_LoadedModels = new List<GameObject>();

    private void Start()
    {
        m_Btn1.onClick.AddListener(LoadNormalModels);
        m_Btn2.onClick.AddListener(LoadGPUAnimationModels);
    }

    void LoadNormalModels()
    {
        ClearModels();
        for (int i = 0; i < m_Col; i++)
        {
            for (int j = 0; j < m_Row; j++)
            {
                var go = GameObject.Instantiate(m_NormalPrefab);
                go.transform.position = new Vector3(i * 1, 1, j * 1);
                m_LoadedModels.Add(go);
            }
        }
    }

    void LoadGPUAnimationModels()
    {
        ClearModels();
        for (int i = 0; i < m_Col; i++)
        {
            for (int j = 0; j < m_Row; j++)
            {
                var go = GameObject.Instantiate(m_GPUAnimationPrefab);
                go.transform.position = new Vector3(i * 1, 1, j * 1);
                m_LoadedModels.Add(go);
            }
        }
    }

    void ClearModels()
    {
        for (int i = 0; i < m_LoadedModels.Count; i++)
        {
            GameObject.DestroyImmediate(m_LoadedModels[i]);
        }
        m_LoadedModels.Clear();
    }

    private void OnDestroy()
    {
        m_Btn1.onClick.RemoveAllListeners();
        m_Btn2.onClick.RemoveAllListeners();
    }


}
