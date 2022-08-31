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

    protected static void OnChangedShapeSelection(Changed<NetworkedShapeManager> changed)
    {
        changed.LoadNew();
        changed.Behaviour.tryToFindNetworkedShape();
    }

    private void tryToFindNetworkedShape()
    {
        Runner.TryFindBehaviour<NetworkedShape>(LastSelectedShapeId, out lastSelectedShape);
        Debug.Log("Check 2");
    }

    public override void FixedUpdateNetwork()
    {

        if(GetInput<InputData>(out var input) == false) return;

        /// process button states if we need those

        // var pressed = input.Buttons.GetPressed(ButtonsPreviousState);
        // var released = input.Buttons.GetReleased(ButtonsPreviousState);
        // ButtonsPreviousState = input.Buttons;

        // we can check for adding shapes:
        if(input.Buttons.IsSet(NetworkedBoardButtons.AddCube))
        {
            // spawn cube
            Debug.Log("Spawning a cube!");
            instantiateShapeOverNetwork((int)ShapeType.Cube);

        }

        if(input.Buttons.IsSet(NetworkedBoardButtons.AddPyramid))
        {
            // spawn pyramid
            Debug.Log("Spawning a pyramid!");
            instantiateShapeOverNetwork((int)ShapeType.Pyramid);
        }

        if(input.Buttons.IsSet(NetworkedBoardButtons.AddSphere))
        {
            // spawn sphere
            Debug.Log("Spawning a sphere!");
            instantiateShapeOverNetwork((int)ShapeType.Sphere);
        }

        if(!input.SelectedShapeId.Equals(LastSelectedShapeId) && input.SelectedShapeId.IsValid)
        {
            LastSelectedShapeId = input.SelectedShapeId;
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
					// bringAllFromHiding();	
				}

				statusText.text = string.Format(
					statusTemplate,
					lastSelectedShape == null ? "none" : lastSelectedShape.Type.ToString() + lastSelectedShape.GetIdentifierTextContent(),
					selectionDuration == originalSelectionDuration ? "full" : selectionDuration
				);
			}

            if(!input.SelectedShapeColor.Equals(lastColor))
            {
                lastColor = input.SelectedShapeColor;
                lastSelectedShape.LastSetColor = lastColor;
            }

            lastSelectedShape.ActiveFaceTextString = input.InputText;
            
        }
    }

   

}
