using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkedShapeManager : NetworkBehaviour
{
    /// Hey there! I'm the networked shape manager! I manage shapes over the network
    /// I'm a NetworkBehaviour from Fusion!
    /// I'm to be attached to a User Prefab, a Fusion NetworkObject!
    [Networked] public NetworkButtons ButtonsPreviousState { get; set; }

    // these should be networked versions of our shapes
    [SerializeField] private List<GameObject> prefabs; // order should mirror enum ShapeTypes

    // selecting
    [Networked(OnChanged=nameof(OnChangedShapeSelection))] public NetworkBehaviourId LastSelectedShapeId {get; set;} 
    private NetworkedShape lastSelectedShape;
    // keeps track o shapes for easy disposal
    [SerializeField] private List<NetworkedShape> shapes; 
    
    [SerializeField] private float selectionDuration;
    private float originalSelectionDuration;
    // stores last text
    private NetworkString<_512> lastOnGoingTextInput, lastFinishedTextInput;
    [SerializeField] private TMPro.TMP_InputField inputField;
    // stores last color 
    private Color lastColor;
    [SerializeField] private CameraController cameraController;
    // splitting
    private int numberOfShapesBeforeCurrentSplit;
    [SerializeField] private int splitWidth;
    [SerializeField] private float splitSpacingX, splitSpacingY;
    private int originalSplitWidth;
    private readonly List<char> lineBreaks = new List<char>() { '.', ',', '?', ':', ';', '!'};


    // monitoring status
    [SerializeField] private TMPro.TMP_Text statusText;
    private readonly string statusTemplate = "selection: {0}\nduration: {1}";

    public void OnInit(TMPro.TMP_Text status)
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        statusText = status;
        originalSelectionDuration = selectionDuration;
        originalSplitWidth = splitWidth;
    }

    private void instantiateShapeOverNetwork(int prefabIndex)
    {
        cameraController.Reset();
        GameObject toInstatiate = prefabs[prefabIndex];
        NetworkObject tmp;
        tmp = Runner.Spawn(
            toInstatiate,
            cameraController.GetScreenTop(8f),
            toInstatiate.transform.rotation,
            Runner.LocalPlayer,
            (runner, o) => {
                NetworkedShape shape = o.GetComponent<NetworkedShape>();
                shape.OnInit();
                shape.Move(cameraController.GetScreenCenter(8f));
                
            }
        );
    }

    private void removeLast()
    {
        if(shapes.Count <= 0)
        {
            Debug.Log("Networked Shape Manager: Nothing to remove!");
            return;

        }

        if(lastSelectedShape)
        {
            shapes.Remove(lastSelectedShape);
            Runner.Despawn(lastSelectedShape.GetComponent<NetworkObject>());
            lastSelectedShape = null;
            selectionDuration = originalSelectionDuration;
            return;
            
        }

        var last = shapes.Last();
        shapes.Remove(last);
        Runner.Despawn(last.GetComponent<NetworkObject>());
    }

    private void removeAll()
    {
        foreach(NetworkedShape shape in shapes)
        {
            Runner.Despawn(shape.GetComponent<NetworkObject>());
        }
        lastSelectedShape = null;
        shapes.Clear();
        
    }

    protected static void OnChangedShapeSelection(Changed<NetworkedShapeManager> changed)
    {
        changed.LoadNew();
        changed.Behaviour.tryToFindNetworkedShape();
    }

    private void tryToFindNetworkedShape()
    {
        Runner.TryFindBehaviour<NetworkedShape>(LastSelectedShapeId, out lastSelectedShape);
        if(lastSelectedShape)
        {
            if(!shapes.Any(s => s.Equals(lastSelectedShape)))
            {
                Debug.Log("Netowrked Shape Manager: Adding a Shape!");
                shapes.Add(lastSelectedShape);
            }
            
        }
    }

    public override void FixedUpdateNetwork()
    {

        if(GetInput<InputData>(out var input) == false) return;

        // the first check is for selection
        if(!input.SelectedShapeId.Equals(LastSelectedShapeId) && input.SelectedShapeId.IsValid)
        {
            LastSelectedShapeId = input.SelectedShapeId;
        }

        /// process button states if we need those

        // var pressed = input.Buttons.GetPressed(ButtonsPreviousState);
        // var released = input.Buttons.GetReleased(ButtonsPreviousState);
        // ButtonsPreviousState = input.Buttons;

        // we can check for adding shapes:
        /// 0 
        if(input.Buttons.IsSet(NetworkedBoardButtons.AddSphere))
        {
            // spawn sphere
            Debug.Log("Spawning a sphere!");
            instantiateShapeOverNetwork((int)ShapeType.Sphere);
        }
        /// 1
        if(input.Buttons.IsSet(NetworkedBoardButtons.AddPyramid))
        {
            // spawn pyramid
            Debug.Log("Spawning a pyramid!");
            instantiateShapeOverNetwork((int)ShapeType.Pyramid);
        }
        /// 2
        if(input.Buttons.IsSet(NetworkedBoardButtons.AddCube))
        {
            // spawn cube
            Debug.Log("Spawning a cube!");
            instantiateShapeOverNetwork((int)ShapeType.Cube);

        }
        /// 3
        if(input.Buttons.IsSet(NetworkedBoardButtons.RemoveLast))
        {
            removeLast();
        }
        /// 4
        if(input.Buttons.IsSet(NetworkedBoardButtons.RemoveAll))
        {
            removeAll();
        }
        

    
        if(lastSelectedShape)
        {
			// status monitor if we host
			if(Runner.IsServer)
			{
				selectionDuration -= Time.deltaTime;
				if(selectionDuration <= 0.0f)
				{
					// forget about shape
					lastSelectedShape = null;
					selectionDuration = originalSelectionDuration;
				}
			}
            if(!input.SelectedShapeColor.Equals(lastColor))
            {
                lastColor = input.SelectedShapeColor;
                lastSelectedShape.LastSetColor = lastColor;
            }

            lastSelectedShape.ActiveFaceTextString = input.InputText;
        }

        // host monitor status
        if(Runner.IsServer)
        {
            statusText.text = string.Format(
                statusTemplate,
                lastSelectedShape == null ? "none" : lastSelectedShape.Type.ToString() + lastSelectedShape.GetIdentifierTextContent(),
                selectionDuration == originalSelectionDuration ? "full" : selectionDuration
            );
        }
    }

   

}
