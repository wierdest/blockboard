using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShapeType
{
    Sphere,
    Pyramid,
    Cube
}
public class Shape : MonoBehaviour
{
    public ShapeType Type;

    /// !!! Make sure both Lists above are always coherent !!! ///

    //// A Pyramid has one extra rotation angle: the start position shows TWO faces, LEFT /|\ RIGHT

    [SerializeField] private List<TMPro.TMP_Text> faceTexts; 
    [SerializeField] private List<Vector3> rotationAngles;

    /// !!! Make sure both Lists above are always coherent !!! ///

    // Rotation
    [SerializeField] private Vector3 activeAngle; 
    private Quaternion originalRotation;
    [SerializeField] private int activeFaceRotationIndex;
    private bool rotate, rotateBottomOrTop, resetRotation;
    [SerializeField] private float rotationSpeed;

    // Text
    [SerializeField] private TMPro.TMP_Text activeFaceText;
    public List<string> ActiveTextSplit;
    private bool hasUpdatedActiveTextSplitForPyramid;
    private readonly string contentIdString = " contains {0}...";

    // Color
    [SerializeField] private MeshRenderer meshRenderer;

    // Movement around world
    private bool move;
    [SerializeField] private Vector3 destination, posBeforeMove;
    [SerializeField] private float moveSpeed;
    private float originalMoveSpeed;
    private Hover hover;

    // Control states
    public bool IsHidden, IsSplit, IsPiece;

    // Nexus: connection between shapes
    private Nexus nexus;

    // Cat
    [SerializeField] private Category catForm;

    private void Awake()
    {
        originalRotation = transform.rotation;
        activeFaceText = Type == ShapeType.Pyramid ? null : faceTexts[activeFaceRotationIndex];
        hover = GetComponent<Hover>();
        originalMoveSpeed = moveSpeed;
        ActiveTextSplit = new List<string>();
        catForm = new Category();
        catForm.examples  = new List<string>();
    }

    public void Rotate()
    {
        if(rotate)
        {
            // Debug.Log("Shape is already rotating!");
            return;
        }

        // Debug.Log("Rotating shape'");
        activeFaceRotationIndex++;
        if(activeFaceRotationIndex > rotationAngles.Count - 1) 
        {
            activeFaceRotationIndex = 0;
            if(Type != ShapeType.Sphere)
            {
                transform.SetPositionAndRotation(transform.position, originalRotation);
                resetRotation = true;
                // Debug.Log("Reset rotation for Cube or Pyramid!");
            }
            
        }

        activeFaceText = (Type == ShapeType.Pyramid) ? (activeFaceRotationIndex == 0 ? null : faceTexts[activeFaceRotationIndex - 1]) : faceTexts[activeFaceRotationIndex];
        activeAngle = rotationAngles[activeFaceRotationIndex];

        if(resetRotation)
        {
            resetRotation = false;
            return;
        }
        rotate = true;
        rotateBottomOrTop = Type != ShapeType.Cube ? activeAngle.z != 0f : activeAngle.x != 0f;
    }

    public void Move(Vector3 dest, float speedMultiplier = 1)
    {
        posBeforeMove = transform.position;
        destination = dest;
        move = true;
        moveSpeed *= speedMultiplier;
    }

    public void MoveBack()
    {
        destination = posBeforeMove;
        move = true;
        moveSpeed *= 10f;
    }

    private void Update()
    {
        if(rotate)
        {
            if((Vector3.Distance(transform.rotation.eulerAngles, activeAngle)) <= 5f)
            {
                rotate = false;
                return;
            }
            transform.Rotate((rotateBottomOrTop ? (Type != ShapeType.Cube ? Vector3.forward : Vector3.right)  :  Vector3.up) * (rotationSpeed * Time.deltaTime));
        } 

        if(move)
        {
            if(Vector3.Distance(transform.position, destination) < .6f)
            {
                move = false;
                moveSpeed = originalMoveSpeed;
                hover.enabled = true;
            }
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }
    }

    #region TEXT
    public void Write(string str)
    {
        // Debug.LogFormat("Shape {0} WRITING! {1}", name, str);

        if(string.IsNullOrEmpty(str))
        {
            // resetTextToSample();
            return;
        }

        if(activeFaceText)
        {
            activeFaceText.text = str;
            return;
        }

        if(Type == ShapeType.Pyramid)
        {
            faceTexts[0].text = str;
            faceTexts[2].text = str;
            hasUpdatedActiveTextSplitForPyramid = false;
            return;
        }

    }
  
    public void FinishUpWriting(string str)
    {
        if(string.IsNullOrEmpty(str))
        {
            // resetTextToSample();
            return;
        }

        if(activeFaceText)
        {
            activeFaceText.text = str;
            return;
        }
      
        if(Type == ShapeType.Pyramid)
        {
            faceTexts[0].text = str;
            faceTexts[2].text = str;
            SplitActiveText();
            hasUpdatedActiveTextSplitForPyramid = true;
            writePyramidDefaultDisplay();
            return;
        }
        
    }

    public void SplitActiveText()
    {
       
        if(Type == ShapeType.Pyramid)
        {
            var str = string.Concat(faceTexts[2].text, faceTexts[0].text);
            if(!hasUpdatedActiveTextSplitForPyramid)
            {
                ActiveTextSplit = faceTexts[0].text.Split(new char[] {' '}).ToList();
                return;
            }

            if(activeFaceRotationIndex == 0)
            {
               
                ActiveTextSplit = str.Split(new char[] {' '}).ToList();
                return;
            }

            ActiveTextSplit = splitFaceText(activeFaceRotationIndex - 1);

            return;

        }

        if(activeFaceText)
        {
            ActiveTextSplit = splitFaceText(activeFaceRotationIndex);
            return;
        }
    }

    private List<string> splitFaceText(int faceTextIndex)
    {
        return faceTexts[faceTextIndex].text.Split(new char[] {' '}).ToList();
    }

    private void writePyramidDefaultDisplay()
    {
        // clear texts
        faceTexts[0].text = "";
        faceTexts[2].text = "";

        int firstHalfLength = ActiveTextSplit.Count / 2;
        var count = 0;
    
        foreach(string word in ActiveTextSplit)
        {
            count++;
            if(count <= firstHalfLength)
            {
                faceTexts[2].text += word + " ";
            }
            else
            {
                faceTexts[0].text += word + " ";
            }
        }
    }

    private void resetTextToSample()
    {
        foreach(TMPro.TMP_Text t in faceTexts)
        {
            t.text = "Sample text";
        }
        Debug.LogFormat("Shape {0} has reset all faces to SAMPLE TEXT!", name);
    }

    public string GetIdentifierTextContent()
    {
        
        if(activeFaceText)
        {
            var str =  activeFaceText.text;

            if(str.Equals("Sample text"))
            {
                return string.Format(contentIdString, "nothing");
            }

            var split = str.Split(" ");
            return string.Format(contentIdString, split[0]);
        }

        return string.Format(contentIdString, "nothing");
    }

    #endregion
    
    #region COLOR

    public void FacePaint(Color color)
    {
        // Debug.LogFormat("Shape {0} COLORING! index q {1}", name, activeFaceRotationIndex);
        if(activeFaceText)
        {
            int faceMaterial = Type == ShapeType.Pyramid ? PyramidMaterialMapping(activeFaceText.name) : activeFaceRotationIndex;
        
            meshRenderer.materials[faceMaterial].color = color;
            return;
        }

        if(Type == ShapeType.Pyramid)
        {
            meshRenderer.materials[0].color = color;
            return;
        }
    }

    public Color GetCurrentFaceColor()
    {
        if(activeFaceText)
        {
            int faceMaterial = Type == ShapeType.Pyramid ? PyramidMaterialMapping(activeFaceText.name) : activeFaceRotationIndex;
            return meshRenderer.materials[faceMaterial].color;
        }

        return meshRenderer.materials[0].color;
    }

    private int PyramidMaterialMapping(string textName)
    {
        // We should have payed more attention when mapping the materials in the mesh production phase
        if(textName.Contains("Right"))
        {
            return 3;

        }
        if(textName.Contains("Front"))
        {
            return 2;
        }

        if(textName.Contains("Bottom"))
        {
            return 1;
        }
        return 0;
    }


    #endregion

    #region NEXUS
    public void RemoveRootCloud()
    {
        if(TryGetComponent<Nexus>(out nexus))
        {
            nexus.Deactivate();
            nexus.Root = null;
        }
    }

    public void RemoveRootCloud(GameObject testRoot)
    {
        if(TryGetComponent<Nexus>(out nexus))
        {
            if(!nexus.Root)
            {
                return;
            }
            if(nexus.Root.Equals(testRoot))
            {
                nexus.Deactivate();
                nexus.Root = null;
                // Debug.LogFormat("Shape: {0} is free from root cloud {1}", name, testRoot.name);
            }
        }
      
    }
    #endregion

    #region CAT

    public void MakeACatOutOfMe()
    {
        if(!activeFaceText && Type == ShapeType.Pyramid)
        {
            Debug.Log("I'm a Pyramid flipped at special position! You can't make a cat out of me!");
         
        }

        if(!activeFaceText)
        {
            Debug.Log("Nothing to make cat out of me!, sorry!");
          
        }

        string catName = activeFaceText.text;
        
        if(catName.Equals("Sample text") || string.IsNullOrEmpty(catName))
        {
            Debug.Log("Nothing to make cat out of me!, sorry!");
            return;
        }
        catForm = new Category();
        catForm.name = catName;
        catForm.color = GetCurrentFaceColor();
        catForm.examples = new List<string>();

        // make cat examples out of pyramid ( really??? )

        if(Type == ShapeType.Pyramid)
        {
            if(strIsOkForCat(catName, faceTexts[1].text))
            {
                catForm.examples.Add(faceTexts[1].text);
                
            }
            if(strIsOkForCat(catName, faceTexts[3].text))
            {
                catForm.examples.Add(faceTexts[3].text);
                
            }
            if(strIsOkForCat(catName, faceTexts[2].text))
            {
                catForm.examples.Add(faceTexts[2].text);
                
            }
            if(strIsOkForCat(catName, faceTexts[0].text))
            {
                catForm.examples.Add(faceTexts[0].text);
                
            }
            return;
        }

        // make cat examples out of sphere or cube

        foreach(TMPro.TMP_Text  t in faceTexts)
        {
            if(strIsOkForCat(catName, t.text))
            {
                catForm.examples.Add(t.text);
            }
        }
    }

    private bool strIsOkForCat(string catName, string str)
    {
        return !string.IsNullOrEmpty(str) && !str.Equals(catName) && !str.Equals("Sample text");
    }

    public Category GetCatForm()
    {
        if(catForm == null)
        {
            MakeACatOutOfMe();
        }
        return catForm;
    }

    public void RemoveCatForm()
    {
        catForm = null;
        // Debug.LogFormat("Shape {0} removed its cat form!");
    }


    #endregion 

}
