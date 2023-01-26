using System.Collections.Generic;
using UnityEngine;
using System;

public class ThisCanSave : MonoBehaviour
{
    [SerializeField] string id = string.Empty;

    public string Id => id;


    void Awake()
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("When you don't give id...", gameObject);
        }
    }

    [ContextMenu("GenerateGuid")]
    private void GenerateId() => id = Guid.NewGuid().ToString();

    public object CaptureState()
    {
        var state = new Dictionary<string, object>();

        foreach (var saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.CaptureState();
        }

        return state;
    }

    public void RestoreState(object state)
    {
        var stateDictionary = (Dictionary<string, object>)state;

        foreach (var saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();
            if (stateDictionary.TryGetValue(typeName, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }

}


/*
--- SAVE EXAMPLE ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveExample : MonoBehaviour,ISaveable
{
    [SerializeField] int lvl = 1;
    [SerializeField] int exp = 0;
    public object CaptureState()
    {
        return new SaveData
        {
            lvl = lvl,
            exp = exp

        };
        
    }
    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        lvl = saveData.lvl;
        exp = saveData.exp;
    }

    [System.Serializable] struct SaveData
    {
        public int lvl;
        public int exp;
    }
}


*/