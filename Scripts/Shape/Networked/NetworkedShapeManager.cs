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
    private NetworkObject networkObject;

    // these should be networked versions of our shapes
    [SerializeField] private List<GameObject> prefabs; // order should mirror enum ShapeTypes

    // selecting
    [Networked(OnChanged=nameof(OnChangedShapeSelection))] public NetworkBehaviourId LastSelectedShapeId {get; set;} 
    private NetworkedShape lastSelectedShape;
    // keeps track of shapes for easy disposal
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

    // the nexus button monitors the selected shapes's nexus existence
    [SerializeField] private SwitchButton nexusButton;

    // category stuff & printer stuff is offline at present
    
    [SerializeField] private CatManager catManager;
    private bool hasCatToGive;

    public void OnInit(TMPro.TMP_Text tmpText, SwitchButton switchButton, CatManager sceneCatManager)
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        originalSelectionDuration = selectionDuration;
        originalSplitWidth = splitWidth;
        networkObject = GetComponent<NetworkObject>();
        statusText = tmpText;
        nexusButton = switchButton;
        catManager = sceneCatManager;
        
    }


    #region ADD & REMOVE 

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
                shapes.Add(shape);
                
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
            if(lastSelectedShape.IsSplit)
            {
                removeAllNexusToSelectedPiece();
            }

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

    #endregion

    #region SPLIT

    private void instatiatePieceShapeOnNetwork(int prefabIndex, string text, int indexCount, int xCount, int yCount, bool nexusBreak)
    {
        // reset camera
        cameraController.Reset();
        // instantiate a copy of the selected shape
        GameObject toInstantiate = (prefabIndex == -1 ? prefabs[(int)lastSelectedShape.Type] : prefabs[0]) as GameObject;

        // size the object
        Vector3 size = toInstantiate.GetComponent<Collider>().bounds.size;

        // position in a grid
        var rowOffset = (Vector3.right * (size.x + splitSpacingX)) * xCount;
        var colOffset = (Vector3.down * (size.y + splitSpacingY)) * yCount;
        var z = prefabIndex == -1 ? 10f : 15f;

        NetworkObject tmp;
        tmp = Runner.Spawn(
            toInstantiate, 
            cameraController.GetScreenTop(z) + colOffset + rowOffset, 
            toInstantiate.transform.rotation,
            Runner.LocalPlayer,
            (runner, o) => {
                NetworkedShape shape = o.GetComponent<NetworkedShape>();
                shape.IsPiece = true;
                shape.OnInit();
                shape.ActiveFaceTextString = text;
                shape.Move(cameraController.GetScreenCenter(z) + colOffset +  rowOffset);
                shapes.Add(shape);  
            }
        );
        // shapes.Last<NetworkedShape>().ActiveFaceTextString = text;
        NetworkedNexus nexus = tmp.GetComponent<NetworkedNexus>();
        var selectedShapeIndex = shapes.FindIndex(0, shapes.Count - 1, s => s.Equals(lastSelectedShape));
        var nexusRootIndex = !nexusBreak ? numberOfShapesBeforeCurrentSplit + (indexCount - 1) : selectedShapeIndex;
        nexus.RootNetworkBehaviourId = shapes[nexusRootIndex].Id;
        
    }

    private void splitShape()
    {
        if(!lastSelectedShape)
        {
            return;
        }

        if(lastSelectedShape.IsPiece)
        {
            lastSelectedShape.RemoveRootCloud();
        }

        lastSelectedShape.SplitActiveText();
        
        if(lastSelectedShape.ActiveTextSplit.Count <= 0)
        {
            Debug.LogFormat("Networked Shape Manager: shape {0} has ZERO strings in ActiveTextSplit!", lastSelectedShape.gameObject.name);
            return;
        }

        // hideAllButSelected();

        // grid positioning variables to instantiate the chip shape

        int xCount = 0, yCount = 0;
        // bool to store conditions that lead to a nexus break between shapes
        bool nexusBreak = false;
        numberOfShapesBeforeCurrentSplit = shapes.Count;

        // this is a split based on the active text split
        var split = lastSelectedShape.ActiveTextSplit;
        Debug.LogFormat("SPLIT LENGTH: {0}", split.Count);
        var numberOfTokens = split.Count;
        bool splitAtCharLevel = numberOfTokens == 1;

        for(int i = 0; i < (splitAtCharLevel ? split[0].Length : numberOfTokens); i++)
        {
            string str = splitAtCharLevel ? split[0][i].ToString() : split[i];
            Debug.LogFormat("String {0}", str);

            if(string.IsNullOrEmpty(str))
            {
                break;
            }

            var wasLastLinePunctuationBreak = !splitAtCharLevel ? (i != 0 && lineBreaks.Contains(split[i - 1].Last<char>())) : false;

            if(xCount >= splitWidth || wasLastLinePunctuationBreak)
            {
                xCount = 0;
                yCount++;
            }

            nexusBreak = i == 0 || wasLastLinePunctuationBreak || yCount != 0 && xCount == 0;
            instatiatePieceShapeOnNetwork(splitAtCharLevel ? (int)ShapeType.Sphere : -1, str, i, xCount, yCount, nexusBreak);
            xCount++;
        }
        
        lastSelectedShape.IsSplit = true;
        cameraController.PushCameraOut(5f);
        // Debug.LogFormat("Shape Manager: shape {0} instantiated {1} piece shapes!", lastSelectedShape.gameObject.name, xCount, nexusBreak);
    }

    #endregion

    #region NEXUS

    private void removeAllNexusToSelectedPiece()
    {
        foreach(NetworkedShape s in shapes)
        {
            if(s.IsPiece && lastSelectedShape)
            {
                s.RemoveRootCloud(lastSelectedShape.Id);
            }
        }
    }

    

    private void changeNexus()
    {
        if(!lastSelectedShape)
        {
            // just to be safe, if we don't have a last selected shape,
            // the nexus button hides itself
            return;
        }

        if(lastSelectedShape.HasNexus())
        {
            lastSelectedShape.RemoveRootCloud();
            
            return;
        }
        // lastSelected Shape has no nexus
        
        // test if it's the only one:

        if(shapes.Count <= 1)
        {
            // make the magnet button disappear: already taken care of by its own button script
            return;
        }
        
        // we automatically add the lastSelectedShape's nexus to the previous shape in the shapes array
        // i.e.: previous shape is root for selected shape
        // so, we need to test if it's the first item in the list.
        int selectedIndex = shapes.FindIndex(0, shapes.Count(), s => s.Equals(lastSelectedShape));
        if(selectedIndex == 0)
        {
            return;
        }
        // if it's not we're in the clear to add a nexus
        lastSelectedShape.AddNexus(shapes[selectedIndex - 1].Id);

    }
    #endregion
    
    
    #region CAT
    public Color? GetLastSelectedShapeColor()
    {
        if(!lastSelectedShape)
        {
            return null;
        }
        return lastSelectedShape.GetCurrentFaceColor();
    }

    public void MakeLastSelectedShapeCatForm()
    {
        if(lastSelectedShape)
        {
            lastSelectedShape.MakeACatOutOfMe();
            hasCatToGive = true;
            return;
        }
        Debug.Log("Shape Manager: can't do anything like that without a selected shape!");

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
        Debug.LogFormat("Networked Shape Manager: tried adding cat {0} to shape {1}!", cat.Name, lastSelectedShape.name);
  
    }

    private void removeCatFormFromCatManager()
    {
        var cat = lastSelectedShape.GetCatForm();
        catManager.RemoveCategory(cat);
        Debug.LogFormat("Networked Shape Manager: removed cat {0} from shape {1}!", cat.Name, lastSelectedShape.name);
    }

        

    public Category GetLastSelectedShapeCatForm()
    {
        if(lastSelectedShape)
        {
            return lastSelectedShape.GetCatForm();
        }
        Debug.Log("Networked Shape Manager: can't do anything like that without a selected shape!");
        return null;
    }
    #endregion



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
                Debug.Log("Networked Shape Manager: Adding a Shape!");
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

        // removing shapes
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
        
        // host monitor status
        if(Runner.IsServer && networkObject.HasInputAuthority)
        {
            statusText.text = string.Format(
                statusTemplate,
                lastSelectedShape == null ? "none" : lastSelectedShape.Type.ToString() + lastSelectedShape.GetIdentifierTextContent(),
                selectionDuration == originalSelectionDuration ? "full" : selectionDuration
            );

        }

        if(!lastSelectedShape)
        {
            // if at this point we don't have a selection, we can return
            return;
        }

        // only host stuff
        if(networkObject.HasInputAuthority)
        {
            selectionDuration -= Time.deltaTime;
            if(selectionDuration <= 0.0f)
            {
                // forget about shape
                lastSelectedShape = null;
                selectionDuration = originalSelectionDuration;
                return;
            }
            // text
            if(!input.InputText.Equals(lastSelectedShape.ActiveFaceTextString))
            {
                lastSelectedShape.ActiveFaceTextString = input.InputText;
            }
            // color
            if(!input.SelectedShapeColor.Equals(lastColor))
            {
                lastColor = input.SelectedShapeColor;
                lastSelectedShape.LastSetColor = lastColor;
            }

            // nexus button monitoring
            nexusButton.Switch(lastSelectedShape.HasNexus());

            if(hasCatToGive)
            {
                addCatFormToCatManager();
            }
            
        }

        // these buttons require a lastSelectedShape
        /// 5
        if(input.Buttons.IsSet(NetworkedBoardButtons.Split))
        {
            splitShape();
            
        }
        /// 6
        if(input.Buttons.IsSet(NetworkedBoardButtons.Nexus))
        {
            changeNexus();
        }
        
    }

}
