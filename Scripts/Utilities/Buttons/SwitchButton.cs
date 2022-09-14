using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField] private Sprite off, on;
    [SerializeField] private Image buttonImage;
    private bool state;

    private void Update()
    {
        
        buttonImage.sprite =  state ? on : off;
    }

    public void Switch(bool value)
    {
        state = value;
    }

    public void On()
    {
        state = true;
    }

    public void Off()
    {
        state = false;
    }
}
