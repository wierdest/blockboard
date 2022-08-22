using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    // the bond between shapes
    public GameObject Root;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;
    private bool isOn;

    public void Activate()
    {
        if(Root == null)
        {
            Debug.LogFormat("Shape {0}'s Nexus is null.", name);
            return;
        }
        offset = transform.position - Root.transform.position;
        isOn = true;
        // Debug.LogFormat("Shape {0} activated nexus. Offset is {1}!", gameObject.name, offset);
    }

    public void Deactivate()
    {
        isOn = false;
    }

    private void Update()
    {
        if(Root && isOn)
        {
            transform.position = Vector3.MoveTowards(transform.position, Root.transform.position + offset, speed * Time.deltaTime);
        }
    }

    
}
