using UnityEngine;

public class Hover : MonoBehaviour
{

    [SerializeField] private float height, speed, waveCut;
    private Vector3 hover;
    private float reverse = -1.0f, originalSpeed;

    private void Start()
    {
        hover = transform.position;
        originalSpeed = speed;
    }

    private void OnEnable()
    {
        hover = transform.position;
        hover.y += height / 2;   
    }
    private void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, hover) <= waveCut)
        {
            // reverse direction
            height *= reverse;
            hover.y += height;
        }
        transform.position = Vector3.MoveTowards(transform.position, hover, speed * Time.deltaTime);
    }

    public void UpdateHoverPosition()
    {
        hover = transform.position;
    }
}
