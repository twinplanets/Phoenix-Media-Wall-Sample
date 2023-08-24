using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TwinPlanets
{
    public class WebcamManager : MonoBehaviourSingleton<WebcamManager>
    {
        public ushort deviceIndex
        {
            get {return _deviceIndex; }
            set { _deviceIndex = value;
                StartWebcam(); }
        }
        [SerializeField] private ushort _deviceIndex = 1;
        WebCamDevice[] devices;

        public bool requireDeviceAuthorisation = false;
        private bool hasUserAuthorization = false;

        public bool cycleCameras = false;
        public float cycleDelay = 15f;

        private WebCamTexture webcamTexture;
        public Renderer[] rendererComponents;
        public RawImage[] rawImageComponents;

        public Text debugDeviceText;

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
            devices = WebCamTexture.devices;
            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log(devices[i].name);
            }

            if (devices.Length > 0)
            {
                if(cycleCameras)
                {
                    StartWebcam();
                    StartCoroutine(CycleCamerasCoroutine());
                }
                else
                {
                    StartWebcam();
                }
            }
            else
            {
                Debug.LogError("No webcam devices found.");
            }
        }

        private void StartWebcam()
        {
            if (webcamTexture != null)
            {
                webcamTexture.Stop();
            }

            Debug.Log("Starting webcam device: " + devices[GetDeviceIndex()]);

            webcamTexture = new WebCamTexture(devices[GetDeviceIndex()].name);

            foreach (var item in rendererComponents)
            {
                item.material.mainTexture = webcamTexture;
            }

            foreach (var item in rawImageComponents)
            {
                item.texture = webcamTexture;
            }

            if(debugDeviceText != null)
            {
                debugDeviceText.text = webcamTexture.deviceName;
            }

            webcamTexture.Play();
        }

        public ushort GetDeviceIndex()
        {
            return (ushort)(_deviceIndex % devices.Length);
        }

        private IEnumerator CycleCamerasCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(cycleDelay); // Wait for 30 seconds

                _deviceIndex++;
                StartWebcam();
            }
        }
    }
}

