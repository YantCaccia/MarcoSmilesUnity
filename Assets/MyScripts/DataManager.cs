using UnityEngine;
using System.IO;


public class SaveLoadObject : MonoBehaviour
{
    public GameObject objToSave;
    public void saveObject()
    {
        // Salva l'oggetto e i suoi figli
        GameObject oggettoDaSalvare = GameObject.Find("NomeDelTuoGameObject");
        SerializableTransform serializableTransform = SaveGameObject(oggettoDaSalvare.transform);
        string jsonString = JsonUtility.ToJson(serializableTransform);
        File.WriteAllText("salvataggio.json", jsonString);

        // Ricarica l'oggetto in un'altra scena
        string jsonFile = File.ReadAllText("salvataggio.json");
        SerializableTransform loadedTransform = JsonUtility.FromJson<SerializableTransform>(jsonFile);
        Transform ricaricato = LoadGameObject(loadedTransform);
    }

    SerializableTransform SaveGameObject(Transform transform)
    {
        SerializableTransform serializableTransform = new SerializableTransform
        {
            name = transform.name,
            position = transform.position,
            rotation = transform.eulerAngles,
            scale = transform.localScale,
            children = new SerializableTransform[transform.childCount]
        };

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            serializableTransform.children[i] = SaveGameObject(childTransform);
        }

        return serializableTransform;
    }

    Transform LoadGameObject(SerializableTransform serializableTransform)
    {
        GameObject loadedObject = new GameObject(serializableTransform.name);
        Transform loadedTransform = loadedObject.transform;
        loadedTransform.position = serializableTransform.position;
        loadedTransform.eulerAngles = serializableTransform.rotation;
        loadedTransform.localScale = serializableTransform.scale;

        if (serializableTransform.children != null)
        {
            foreach (SerializableTransform childSerializableTransform in serializableTransform.children)
            {
                Transform loadedChild = LoadGameObject(childSerializableTransform);
                loadedChild.parent = loadedTransform;
            }
        }

        return loadedTransform;
    }
}