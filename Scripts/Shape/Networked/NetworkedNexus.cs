using Fusion;
using UnityEngine;

public class NetworkedNexus : NetworkBehaviour
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

    public override void FixedUpdateNetwork()
    {
        if(Root && isOn)
        {
            transform.position = Vector3.MoveTowards(transform.position, Root.transform.position + offset, speed * Runner.DeltaTime);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
