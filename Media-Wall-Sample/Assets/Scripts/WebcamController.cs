using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebcamController : MonoBehaviour
{
    public int deviceIndex = 0;
    private bool hasUserAuthorization = false;
    
    private WebCamTexture webcamTexture;
    private Renderer rendererComponent;
    private RawImage rawImageComponent;

    IEnumerator Start()
    {
        //Loop Authorization Request until Authorized
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        //while (Application.HasUserAuthorization(UserAuthorization.WebCam) == false)
        //{
        //    yield return new WaitForSeconds(0.25f);
        //    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        //    Debug.Log("Authorisation Requested");
        //}
        hasUserAuthorization = Application.HasUserAuthorization(UserAuthorization.WebCam);

        hasUserAuthorization = true;

        Debug.Log(hasUserAuthorization);
        if (hasUserAuthorization)
        {
            // Check for Renderer
            rendererComponent = GetComponent<Renderer>();
            // Check for CanvasRenderer
            rawImageComponent = GetComponent<RawImage>();

            WebCamDevice[] devices = WebCamTexture.devices;
            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log(devices[i].name);
            }

            webcamTexture = new WebCamTexture(devices[2].name);

            // Perform swapping based on the availability of components
            if (rendererComponent != null)
            {
                // Check if webcam is available
                if (WebCamTexture.devices.Length > 0)
                {
                    rendererComponent.material.mainTexture = webcamTexture;
                    webcamTexture.Play();
                }
                else
                {
                    Debug.LogError("No webcam devices found.");
                }

            }
            else if (rawImageComponent != null)
            {
                if (WebCamTexture.devices.Length > 0) {
                    rawImageComponent.texture = webcamTexture;
                    webcamTexture.Play();
                }
                else
                {
                    Debug.LogError("No webcam devices found.");
                }
            }
        }
    }

}
