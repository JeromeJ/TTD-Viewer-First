using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualBehaviour : MonoBehaviour
{

    public Transform m_transform;

    void Awake()
    {
        m_transform = GetComponent<Transform>();
    }
}
