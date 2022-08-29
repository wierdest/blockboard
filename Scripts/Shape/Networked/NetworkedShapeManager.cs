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
    // keeps track of the shapes
    [SerializeField] private List<NetworkedShape> shapes;

    // selecting
    [SerializeField] private Shape lastSelectedShape;
    [SerializeField] private float selectionDuration;
    private float originalSelectionDuration;
    public bool HasShape;

    // modifying (user interface elements to connect)
    [SerializeField] private TMPro.TMP_InputField inputField;
    [SerializeField] private ColorPicker colorPicker;
    [SerializeField] private CameraController cameraController;

    private void Awake()
    {
        shapes = new List<NetworkedShape>();
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void instantiateShapeOverNetwork(int prefabIndex)
    {
        cameraController.Reset();
        GameObject toInstatiate = prefabs[prefabIndex];
        NetworkObject tmp;
        tmp = Runner.Spawn(
            toInstatiate,
            cameraController.GetScreenTop(8f),
            toInstatiate.transform.rotation

        );
        tmp.GetComponent<NetworkedHover>().enabled = false;
        NetworkedShape shape = tmp.GetComponent<NetworkedShape>();
        shape.Move(cameraController.GetScreenCenter(8f));
        shapes.Add(shape);
    }
    public override void FixedUpdateNetwork()
    {
        // check input over network
        if(GetInput<InputData>(out var input) == false) return;


        /// process button states if we need those

        // var pressed = input.Buttons.GetPressed(ButtonsPreviousState);
        // var released = input.Buttons.GetReleased(ButtonsPreviousState);
        // ButtonsPreviousState = input.Buttons;

        // we can check 

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


    }
}
