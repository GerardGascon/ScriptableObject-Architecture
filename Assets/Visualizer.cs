using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class Visualizer : MonoBehaviour {
    [SerializeField] BoolReference boolVariable;
    [SerializeField] FloatReference floatVariable;
    [SerializeField] Vector2Reference vector2Variable;
    [SerializeField] Vector3Reference vector3Variable;
    [SerializeField] IntReference intVariable;
}