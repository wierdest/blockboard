using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyAndSunRotation : MonoBehaviour
{
    [SerializeField] private Material sky;
    [SerializeField] private Transform sun;
    [SerializeField] private float speed;
    private Quaternion originalSunRotation;
    private float originalSkyRotation, skyRate;
    private bool hasReset;

    private void Awake()
    {
        originalSkyRotation = sky.GetFloat("_Rotation");
        originalSunRotation = sun.transform.rotation;
    }


    private void Update()
    {
        var sunRate = Time.deltaTime * speed;
        sun.RotateAround(sun.position, sun.up, -sunRate);
        skyRate += (sunRate - (sunRate * 0.4f)) ;
        sky.SetFloat("_Rotation", skyRate);

        if(ClickAndDrag.Instance.CurrentShape != null || ClickAndDrag.Instance.CurrentNetworkedShape != null)
        {
            if(!hasReset)
            {
                // Debug.LogFormat("Resetting! This is originalSkyRot {0}, this is current {1}", originalSkyRotation, sky.GetFloat("_Rotation"));
                reset();
                hasReset = true;
            }
        }
        else
        {
            hasReset = false;
        }
        
    }

    private void reset()
    {
        skyRate = 0f;
        sky.SetFloat("_Rotation", skyRate);
        sun.SetPositionAndRotation(sun.transform.position, originalSunRotation);
    }
}
