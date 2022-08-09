using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to be attached to a Camera 
public class CameraController : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float push = 0f;
    [SerializeField] private float sensitivity, verticalLookRotation, horizontalLookRotation;
    private bool hasSetControls, hasResetControls;
    private Camera cam;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        push = 0f;
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if(ClickIdentifier.Instance.IsPointerOverUI())
        {
            return;
        } 
        if(ClickAndDrag.Instance.Target == null)
        {
            if(Input.GetMouseButton(0))
            {
                horizontalLookRotation += Input.GetAxis("Mouse X") * sensitivity;
                verticalLookRotation += Input.GetAxisRaw("Mouse Y") * sensitivity;
                transform.localEulerAngles = (Vector3.left * verticalLookRotation) + (Vector3.down * horizontalLookRotation);
                if(!hasSetControls)
                {
                    // UserInterface.Instance.SetCameraControls(true);
                    hasSetControls = true;
                }
            }
        }
        else
        {
            if(hasSetControls)
            {
                // UserInterface.Instance.SetCameraControls(false);
                hasSetControls = false;
                push = 0f;
            }
        }

    }
    public void Reset()
    {
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        verticalLookRotation = 0.0f;
        horizontalLookRotation = 0.0f;
        push = 0f;
    } 

    public void OnClickCamOutButton()
    {
        // Push Back
        push += 0.1f;
        var translation = Vector3.back * push;
        transform.Translate(translation, Space.World);
        Debug.Log("Clicked!");
    }

    public void OnClickCamInButton()
    {
        push += 0.1f;
        var translation = Vector3.forward * push;
        transform.Translate(translation, Space.World);
        

    }

    public void PushCameraOut(float push)
    {
        var translation = Vector3.back * push;
        transform.Translate(translation, Space.World);

    }

    public Vector3 GetScreenCenter(float z)
    {
        return cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, z));
    }

    public Vector3 GetScreenTop(float z)
    {
        return cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height + 10f , z));
    }
    
   
}
