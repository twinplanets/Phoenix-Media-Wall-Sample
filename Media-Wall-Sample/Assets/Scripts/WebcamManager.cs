using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviourSingleton<WebcamManager>
{
    public int deviceIndex = 3;
    public bool requireDeviceAuthorisation = false;
    private bool hasUserAuthorization = false;

    private WebCamTexture webcamTexture;
    public Renderer[] rendererComponents;
    public RawImage[] rawImageComponents;

    IEnumerator Start()
    {
        //Loop Authorization Request until Authorized
        if(requireDeviceAuthorisation)
        {
            while (Application.HasUserAuthorization(UserAuthorization.WebCam) == false)
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
                Debug.Log("Authorisation Requested");
                hasUserAuthorization = Application.HasUserAuthorization(UserAuthorization.WebCam);
                Debug.Log("Authorised: " + hasUserAuthorization);
                yield return new WaitForSeconds(0.25f);
            }
        }

        //List all devices
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
        }

        webcamTexture = new WebCamTexture(devices[deviceIndex].name);

        foreach (var item in rendererComponents)
        {
            item.material.mainTexture = webcamTexture;
        }

        foreach (var item in rawImageComponents)
        {
            item.texture = webcamTexture;
        }

        webcamTexture.Play();
    }
}
