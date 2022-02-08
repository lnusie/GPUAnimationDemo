using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class AnimatorParamInfo : ScriptableObject
{
    [Serializable]
    public class BaseParam
    {
        public string m_ParamName;
    }

    [Serializable]
    public class TriggerParam : BaseParam
    {
        public bool m_Value;
    }

    [Serializable]
    public class BooleanParam : BaseParam
    {
        public bool m_Value;
    }

    [Serializable]
    public class IntParam : BaseParam
    {
        public int m_Value;
    }

    [Serializable]
    public class FloatParam : BaseParam
    {
        public float m_Value;
    }

    public List<TriggerParam> m_TriggerParams;
    public List<BooleanParam> m_BooleanParams;
    public List<IntParam> m_IntParams;
    public List<FloatParam> m_FloatParams;

    private Dictionary<string, TriggerParam> m_TriggerParamDict;
    public Dictionary<string, TriggerParam> TriggerParamDict
    {
        get
        {
            if (m_TriggerParamDict == null)
            {
                CreateParamsDict(out m_TriggerParamDict, m_TriggerParams);
            }
            return m_TriggerParamDict;
        }
    }

    private Dictionary<string, BooleanParam> m_BooleanParamsDict;
    public Dictionary<string, BooleanParam> BooleanParamsDict
    {
        get
        {
            if (m_BooleanParamsDict == null)
            {
                CreateParamsDict(out m_BooleanParamsDict, m_BooleanParams);
            }
            return m_BooleanParamsDict;
        }
    }

    private Dictionary<string, IntParam> m_IntParamsDict;
    public Dictionary<string, IntParam> IntParamsDict
    {
        get
        {
            if (m_IntParamsDict == null)
            {
                CreateParamsDict(out m_IntParamsDict, m_IntParams);
            }
            return m_IntParamsDict;
        }
    }

    private Dictionary<string, FloatParam> m_FloatParamsDict;
    public Dictionary<string, FloatParam> FloatParamsDict
    {
        get
        {
            if (m_FloatParamsDict == null)
            {
                CreateParamsDict(out m_FloatParamsDict, m_FloatParams);
            }
            return m_FloatParamsDict;
        }
    }

    public void CreateParamsDict<T>(out Dictionary<string, T> paramsDict, List<T> paramasList) where T :BaseParam
    {
        paramsDict = new Dictionary<string, T>();
        if (paramasList == null) return;
        foreach (var param in paramasList)
        {
            if (!paramsDict.ContainsKey(param.m_ParamName))
            {
                paramsDict.Add(param.m_ParamName, param);
            }
        }
    }

    public bool GetTriggerValue(string paramName, out bool value)
    {
        if (!TriggerParamDict.ContainsKey(paramName))
        {
            value = false;
            return false;
        }
        value = TriggerParamDict[paramName].m_Value;
        return true;
    }

    public void SetTriggerValue(string paramName, bool value)
    {
        if (TriggerParamDict.ContainsKey(paramName))
        {
            TriggerParamDict[paramName].m_Value = value;
        }
    }

}
