using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDrag : MonoBehaviour
{
    public static ClickAndDrag Instance;
    public GameObject Target;

    // offline shape
    private Hover hover;
    public Shape CurrentShape;

    // networked shape
    private NetworkedHover networkedHover;
    public NetworkedShape CurrentNetworkedShape;

    // network input provider
    [SerializeField] InputProvider networkInputProvider;


    private Vector3 offset, worldToScreenPos, currentScreenPosition, originalWorldPosition;
    private bool dragging, letGo;
    [SerializeField] private float forgetOffset, rotationCountLimit, rotationCount;

    
    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if(ClickIdentifier.Instance.IsPointerOverUI())
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            letGo = false;
            getClickedObject(out hit);

            if(Target)
            {
                originalWorldPosition = Target.transform.position;
                worldToScreenPos = Camera.main.WorldToScreenPoint(originalWorldPosition);
                updateCurrentScreenPosition();
                offset = originalWorldPosition - Camera.main.ScreenToWorldPoint(currentScreenPosition);
                dragging = true;
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            dragging = false;
            letGo = true;

        }

        if(hover)
        {
            hover.enabled = !dragging;
            if(letGo)
            {
                hover.UpdateHoverPosition();
            }
        }

        if(networkedHover)
        {
            networkedHover.enabled = !dragging;
            if(letGo)
            {
                networkedHover.UpdateHoverPosition();
            }
        }

        if(Target && dragging)
        {
            updateCurrentScreenPosition();
            Target.transform.position = Camera.main.ScreenToWorldPoint(currentScreenPosition) + offset;
            rotationCount += Time.fixedDeltaTime;
        }

        if(letGo)
        {
            if(CurrentShape)
            {
                if(rotationCount < rotationCountLimit)
                {
                    CurrentShape.Rotate();
                }
                CurrentShape = null;
            }

            if(CurrentNetworkedShape)
            {
                if(rotationCount < rotationCountLimit)
                {
                    CurrentNetworkedShape.ActiveFaceRotationIndex++;
                }
                CurrentNetworkedShape = null;

            }

            rotationCount = 0f;
            if(Target != null && Vector3.Distance(Target.transform.position, originalWorldPosition) <= forgetOffset)
            {
                Target = null;
                hover = null;
                letGo = false;
                CurrentShape = null;
                CurrentNetworkedShape = null;
            }
            
        }
    }

    private void updateCurrentScreenPosition()
    {
        currentScreenPosition.x = Input.mousePosition.x;
        currentScreenPosition.y = Input.mousePosition.y;
        currentScreenPosition.z = worldToScreenPos.z;
    }

    private void getClickedObject(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            Target = hit.collider.gameObject;
            if(Target.CompareTag("Shape"))
            {
                hover = Target.GetComponent<Hover>();
                CurrentShape = Target.GetComponent<Shape>();
                
            }

            if(Target.CompareTag("NetworkedShape"))
            {
                networkedHover = Target.GetComponent<NetworkedHover>();
                CurrentNetworkedShape = Target.GetComponent<NetworkedShape>();
                networkInputProvider.SelectNetworkedShape(CurrentNetworkedShape.Id);
            }
        }
    }
}
