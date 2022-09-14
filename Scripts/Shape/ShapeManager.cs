using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShapeManager : MonoBehaviour
{
    /// Hey, there! I'm the shape manager. I manage shapes! I make them interact with UI and Cats!

    // these are the shapes I'm working with! 
    /// I can Add them from the prefab list, or I can split them and create more of the same!
    [SerializeField] private List<GameObject> prefabs; // order should mirror enum ShapeTypes
    [SerializeField] private List<Shape> shapes;

    /// here's the data needed for each task I'm responsible for!

    // selecting
    [SerializeField] private Shape lastSelectedShape;
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


    
    private void Awake()
    {
        shapes = new List<Shape>();
        shapes = Transform.FindObjectsOfType<Shape>().ToList();
        // Debug.LogFormat("Shape Manager Awake: found {0} initial shapes to work with! Good luck!", shapes.Count);
        originalSelectionDuration = selectionDuration;
        originalSplitWidth = splitWidth;
    }

    private void Start()
    {
        colorPicker.SetOnValueChangeCallback(colorShape);
    }

    private void Update()
    {
        // connect the shape manager to the click and drag's current selection of shape
        if(ClickAndDrag.Instance.CurrentShape)
        {
            lastSelectedShape = shapes.First(s => s.Equals(ClickAndDrag.Instance.CurrentShape));
            selectionDuration = originalSelectionDuration;
            // hideAllButSelected();
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
    }
    
    // this here shape manager's job is to make user interface elements interact with the shapes, so

    // the first step is to connect the text InputField
    #region TEXT
    public void OnTextInputValueChange()
    {
        
        string value = inputField.text;

        if(string.IsNullOrEmpty(value))
        {
            return;
        }

        if(!lastSelectedShape)
        {
            
            // var nonPieces = shapes.Where(s => s.IsPiece);
            // if(nonPieces.Count() > 0)
            // {
            //     return;
            // }

            // foreach(Shape s in shapes)
            // {
            //     s.Write(value);
            // }
            // return;

            return;
        }

        lastSelectedShape.Write(value);

    }

    public void OnTextInputEndEdit()
    {
        string value = inputField.text;

        if(string.IsNullOrEmpty(value))
        {
            return;
        }

        if(!lastSelectedShape)
        {           
            foreach(Shape s in shapes.Where(s => !s.IsPiece))
            {
                s.FinishUpWriting(value);
            }
            return;
        }
        
        lastSelectedShape.FinishUpWriting(value);
    }

    #endregion

    // the second step is to connect the color picker
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

    // the third step is to connect the add shapes buttons and the remove button as well
    #region ADD & REMOVE SHAPES
    
    // in order to add shapes, we need to instantiate them, obvs
    private void instatiateShapeOnClick(int prefabIndex)
    {
        cameraController.Reset();
        GameObject toInstatiate = prefabs[prefabIndex];
        GameObject tmp;
        tmp = Instantiate(prefabs[prefabIndex], cameraController.GetScreenTop(8f), toInstatiate.transform.rotation);
        tmp.GetComponent<Hover>().enabled = false;
        Shape shape = tmp.GetComponent<Shape>();
        shape.Move(cameraController.GetScreenCenter(8f));
        shapes.Add(shape);

    }
    
    public void OnClickAddSphere()
    {
        cameraController.PushCameraOut(3f);
        instatiateShapeOnClick((int)ShapeType.Sphere);
    }

    public void OnClickAddPyramid()
    {
        cameraController.PushCameraOut(3f);
        instatiateShapeOnClick((int)ShapeType.Pyramid);
    }

    public void OnClickAddCube()
    {
        cameraController.PushCameraOut(3f);
        instatiateShapeOnClick((int)ShapeType.Cube);
    }

    public void OnClickRemoveButton()
    {
        foreach(Shape s in shapes)
        {
            Destroy(s.gameObject);
        }

        lastSelectedShape = null;
        shapes.Clear();
    }
    public void OnClickUndoButton()
    {
        if(shapes.Count <= 0)
        {
            // Debug.Log("Shape Manager: nothing to remove, honey!");
            return;
        }

        // default behaviour is to remove last to first
        // if we have a shape selected, we remove that one instead
        if(lastSelectedShape)
        {
            if(lastSelectedShape.IsSplit)
            {
                removeAllNexusToSelectedPiece();
            }
            shapes.Remove(lastSelectedShape);
            Destroy(lastSelectedShape.gameObject);
            lastSelectedShape = null;
            selectionDuration = originalSelectionDuration;
            // bringAllFromHiding();
            return;
        }

        var last = shapes.Last();
        shapes.Remove(last);
        Destroy(last.gameObject);

        // clear all cats if there's no more shapes left
        // if(shapes.Count <= 0)
        // {
        //     if(catManager.GetCatsCount() > 0)
        //     {
        //         catManager.ClearCats();
        //     }
        // }
    }

    #endregion
    
    // the fourth step is to implement the break-shape-into-pieces functionality
    #region SPLITTIN'
    public void OnClickSplitButton()
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
            Debug.LogFormat("Shape Manager: shape {0} has ZERO strings in ActiveTextSplit!", lastSelectedShape.gameObject.name);
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
        var numberOfTokens = split.Count;
        bool splitAtCharLevel = numberOfTokens == 1;

        for(int i = 0; i < (splitAtCharLevel ? split[0].Length : numberOfTokens); i++)
        {
            string str = splitAtCharLevel ? split[0][i].ToString() : split[i];

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
            instatiateShapeOnSplit(splitAtCharLevel ? (int)ShapeType.Sphere : -1, str, i, xCount, yCount, nexusBreak);
            xCount++;
        }
        
        lastSelectedShape.IsSplit = true;
        cameraController.PushCameraOut(5f);
        // Debug.LogFormat("Shape Manager: shape {0} instantiated {1} piece shapes!", lastSelectedShape.gameObject.name, xCount, nexusBreak);
    }
    
    private void instatiateShapeOnSplit(int prefabIndex, string text, int indexCount, int xCount, int yCount, bool nexusBreak)
    {
        // reset camera
        cameraController.Reset();
        // instantiate a copy of the selected shape
        GameObject toInstantiate = prefabIndex == -1 ? lastSelectedShape.gameObject :  prefabs[prefabIndex] as GameObject;
        // temp storage
        GameObject tmp;

        // size the object
        Vector3 size = toInstantiate.GetComponent<Collider>().bounds.size;

        // position in a grid
        var rowOffset = (Vector3.right * (size.x + splitSpacingX)) * xCount;
        var colOffset = (Vector3.down * (size.y + splitSpacingY)) * yCount;
        var z = prefabIndex == -1 ? 10f : 15f;

        tmp = Instantiate(
            toInstantiate, 
            cameraController.GetScreenTop(z) + colOffset + rowOffset, 
            toInstantiate.transform.rotation
        ) as GameObject;

        Shape shape = tmp.GetComponent<Shape>();
        shape.Write(text);
        shape.IsPiece = true;
        shape.Move(cameraController.GetScreenCenter(z) + colOffset + rowOffset);
        shapes.Add(shape);

        Nexus nexus = tmp.GetComponent<Nexus>();
        var selectedShapeIndex = getLastSelectedShapeIndex();
        var nexusRootIndex = !nexusBreak ? numberOfShapesBeforeCurrentSplit + (indexCount - 1) : selectedShapeIndex;
        nexus.Root = shapes[nexusRootIndex].gameObject;
        nexus.Activate();
    }


    private int getLastSelectedShapeIndex()
    {
        return shapes.FindIndex(0, shapes.Count - 1, s => s.Equals(lastSelectedShape));
    }

    #endregion

   
    // the fifth step is to connect the category button & manager for interesting printing purposes ! we're done! :)
    #region CAT
    // a category can be put in the corpus for later.

    public void MakeLastSelectedShapeCatForm(bool isCorpus)
    {
        if(lastSelectedShape)
        {
            lastSelectedShape.MakeACatOutOfMe(isCorpus);
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
        Debug.Log("Shape Manager: can't do anything like that without a selected shape!");
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
        Debug.LogFormat("Shape Manager: tried adding cat {0} to shape {1}!", cat.Name, lastSelectedShape.name);
  
    }

    private void removeCatFormFromCatManager()
    {
        var cat = lastSelectedShape.GetCatForm();
        if(cat == null)
        {
            Debug.Log("Can't remove a null category!");
            return;
        }
        catManager.RemoveCategory(cat);
        Debug.LogFormat("Shape Manager: removed cat {0} from shape {1}!", cat.Name, lastSelectedShape.name);
    }


    #endregion

    // the fifth step is to be able to edit the connection between shapes, the NEXUS
    #region NEXUS

    private void removeAllNexusToSelectedPiece()
    {
        foreach(Shape s in shapes)
        {
            if(s.IsPiece && lastSelectedShape)
            {
                s.RemoveRootCloud(lastSelectedShape.gameObject);
            }
        }
    }

    public bool LastSelectedShapeHasNexus()
    {
        return lastSelectedShape != null && lastSelectedShape.HasNexus();
    }

    public void RemoveLastSelectedShapeNexus()
    {
        lastSelectedShape.RemoveRootCloud();
    }

    public void OnClickNexusButton()
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
        lastSelectedShape.AddNexus(shapes[selectedIndex - 1].gameObject);

    }
    #endregion

    
}
