using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    string dataPath = null;

    private void Awake()
    {
        dataPath = Application.persistentDataPath + "/save32.txt";
    }

    [ContextMenu("Save")]
    private void Save()
    {
        var state = LoadFile();
        CaptureState(state);
        SaveFile(state);
    }

    [ContextMenu("Load")]
    private void Load()
    {
        var state = LoadFile();
        RestoreState(state);
    }

    private void SaveFile(object state)
    {
        FileStream stream = new FileStream(dataPath, FileMode.Create);


        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, state);

        stream.Close();

    }
    private Dictionary<string, object> LoadFile()
    {
        if (!File.Exists(dataPath))
        {
            return new Dictionary<string, object>();
        }
        else
        {
            FileStream stream = new FileStream(dataPath, FileMode.Open);

            var formatter = new BinaryFormatter();


            Dictionary<string, object> data = formatter.Deserialize(stream) as Dictionary<string, object>;

            stream.Close();

            return data;
        }
    }

    private void CaptureState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<ThisCanSave>())
        {
            state[saveable.Id] = saveable.CaptureState();
        }
    }

    private void RestoreState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<ThisCanSave>())
        {
            if (state.TryGetValue(saveable.Id, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }
}


public interface ISaveable
{
    object CaptureState();
    void RestoreState(object state);
}
