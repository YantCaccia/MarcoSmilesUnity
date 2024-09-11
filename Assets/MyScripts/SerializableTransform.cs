using UnityEngine;

[System.Serializable]
public class SerializableTransform
{
    public GameObject objtoSave;
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public SerializableTransform[] children;
}