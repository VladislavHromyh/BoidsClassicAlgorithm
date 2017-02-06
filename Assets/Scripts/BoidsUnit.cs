using UnityEngine;
using System.Collections;

public class BoidsUnit : MonoBehaviour
{
    public Transform Transform;
    public Vector3 m_Speed;

    private void Awake()  {
        Initialize();
    }

    private void Initialize() {
        Transform = GetComponent<Transform>();
    }
}
