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
    [SerializeField] private NetworkedShape lastSelectedShape;
    [SerializeField] private float selectionDuration;
    private float originalSelectionDuration;
    public bool HasShape;

    // modifying (user interface elements to connect)
    [SerializeField] private TMPro.TMP_InputField inputField;
    [SerializeField] private ColorPicker colorPicker;
    [SerializeField] private CameraController cameraController;

    // splitting
    private int numberOfShapesBeforeCurrentSplit;
    [SerializeField] private int splitWidth;
    [SerializeField] private float splitSpacingX, splitSpacingY;
    private int originalSplitWidth;
    private readonly List<char> lineBreaks = new List<char>() { '.', ',', '?', ':', ';', '!'};

    // category manager
    [SerializeField] private CatManager catManager;
    private bool hasCatToGive;

    // monitoring status
    [SerializeField] private TMPro.TMP_Text statusText;
    private readonly string statusTemplate = "selection: {0}\nduration: {1}";

    public void Init(TMPro.TMP_Text status)
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        colorPicker = FindObjectOfType<ColorPicker>();
        statusText = status;
        originalSelectionDuration = selectionDuration;
        originalSplitWidth = splitWidth;
    }
    private void Awake()
    {
        shapes = new List<NetworkedShape>();
        
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

        // connect the networked shape manager to the click and drag's current selection of a networked shape
        if(ClickAndDrag.Instance.CurrentNetworkedShape)
        {
            lastSelectedShape = shapes.First(s => s.Equals(ClickAndDrag.Instance.CurrentNetworkedShape));
            selectionDuration = originalSelectionDuration;
            return;
        }

        if(lastSelectedShape)
        {
            selectionDuration -= Time.deltaTime;
            if(selectionDuration <= 0.0f)
            {
                // forget about shape
                lastSelectedShape = null;
                selectionDuration = originalSelectionDuration;
                // bringAllFromHiding();
                
            }

            if(hasCatToGive)
            {
                addCatFormToCatManager();
            }

        }

        HasShape = lastSelectedShape != null;

         // status monitor
        statusText.text = string.Format(
            statusTemplate,
            lastSelectedShape == null ? "none" : lastSelectedShape.Type.ToString() + lastSelectedShape.GetIdentifierTextContent(),
            selectionDuration == originalSelectionDuration ? "full" : selectionDuration
        );



        // check input over network
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

    }

    #region COLOR

    private void colorShape(Color color)
    {
        if(!lastSelectedShape)
        {
            return;
        }
        lastSelectedShape.FacePaint(color);
    }

    public Color? GetLastSelectedShapeColor()
    {
        if(!lastSelectedShape)
        {
            return null;
        }
        return lastSelectedShape.GetCurrentFaceColor();
    }

    #endregion


    #region CAT
    public void MakeLastSelectedShapeCatForm()
    {
        if(lastSelectedShape)
        {
            lastSelectedShape.MakeACatOutOfMe();
            hasCatToGive = true;
            return;
        }
        Debug.Log("Networked Shape Manager: can't do anything like that without a selected shape!");

    }

    public void RemoveLastSelectedShapeCategory()
    {
        if(lastSelectedShape)
        {
            removeCatFormFromCatManager();
            lastSelectedShape.RemoveCatForm();
            return;
        }
        Debug.Log("Networked Shape Manager: can't do anything like that without a selected shape!");
    }

    private void addCatFormToCatManager()
    {
        if(!lastSelectedShape)
        {
            return;
        }
        var cat = lastSelectedShape.GetCatForm();
        catManager.AddCategory(cat);
        hasCatToGive = false;
        Debug.LogFormat("Networked Shape Manager: tried adding cat {0} to shape {1}!", cat.name, lastSelectedShape.name);
  
    }

    private void removeCatFormFromCatManager()
    {
        var cat = lastSelectedShape.GetCatForm();
        catManager.RemoveCategory(cat);
        Debug.LogFormat("Networked Shape Manager: removed cat {0} from shape {1}!", cat.name, lastSelectedShape.name);
    }

        

    public Category GetLastSelectedShapeCatForm()
    {
        if(lastSelectedShape)
        {
            return lastSelectedShape.GetCatForm();
        }
        Debug.Log("Shape Manager: can't do anything like that without a selected shape!");
        return null;
    }



    
    #endregion
}
