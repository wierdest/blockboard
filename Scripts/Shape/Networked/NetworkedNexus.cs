using Fusion;
using UnityEngine;

public class NetworkedNexus : NetworkBehaviour
{
    // the bond between shapes
    [Networked(OnChanged=nameof(OnChangedRoot))] public NetworkBehaviourId RootNetworkBehaviourId {get; set;}
    private NetworkBehaviourId? lastRootId;
    public GameObject Root;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;
    private bool isOn;

    protected static void OnChangedRoot(Changed<NetworkedNexus> changed)
    {
        changed.LoadNew();
        var newRoot = changed.Behaviour.RootNetworkBehaviourId;
        changed.Behaviour.setRootOverNetwork(newRoot);
        
    }
    private void setRootOverNetwork(NetworkBehaviourId id)
    {
        NetworkedShape netRoot;
        if(Runner.TryFindBehaviour<NetworkedShape>(id, out netRoot))
        {
            Root = netRoot.gameObject;
            Activate();
        }
        else
        {
            // null
            Debug.LogFormat("NetworkedNexus of {0} couldn't find Root!", gameObject.name);
            Deactivate();
        }

    }

    public void Reload()
    {
        setRootOverNetwork(RootNetworkBehaviourId);
    }


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
    public void DeactivateAndSetRootToNull()
    {
        isOn = false;
        Root = null;
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
