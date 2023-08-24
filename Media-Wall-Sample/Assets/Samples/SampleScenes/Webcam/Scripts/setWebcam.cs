using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setWebcam : MonoBehaviour
{
    public void SetWebcam(int i)
    {
        TwinPlanets.WebcamManager.Instance.deviceIndex = (ushort)i;
    }
}
