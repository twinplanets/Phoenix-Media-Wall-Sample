using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebcamController : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private Renderer rendererComponent;
    private RawImage rawImageComponent;

    IEnumerator Start()
    {
    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            // Check for Renderer
            rendererComponent = GetComponent<Renderer>();
            // Check for CanvasRenderer
            rawImageComponent = GetComponent<RawImage>();

            webcamTexture = new WebCamTexture();

            WebCamDevice[] devices = WebCamTexture.devices;
            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log(devices[i].name);
            }

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
