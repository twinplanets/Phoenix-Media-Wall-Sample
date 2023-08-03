using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skeleton
{
    public Vector3[] joints;
    public Vector3 position;
}

public class WsToSkeleton : MonoBehaviour
{
    [SerializeField] private Skeleton[] _skeletons;
    public GameObject[] debugSkeletons;

    private void LateUpdate()
    {
        if(debugSkeletons.Length > _skeletons.Length)
        {
            for (int i = debugSkeletons.Length-1; i > _skeletons.Length-1; i--)
            {
                Debug.Log(i);
                Destroy(debugSkeletons[i].gameObject);
            }
            System.Array.Resize(ref debugSkeletons, _skeletons.Length);
        }

        if (debugSkeletons.Length < _skeletons.Length)
        {
            System.Array.Resize(ref debugSkeletons, _skeletons.Length);
        }


        for (int i = 0; i < _skeletons.Length; i++)
        {
            if(debugSkeletons[i] == null)
            {
                debugSkeletons[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                debugSkeletons[i].AddComponent<WsSkeletonDebug>();
                debugSkeletons[i].GetComponent<WsSkeletonDebug>().InstantiateSkeleton(_skeletons[i]);
            }
            debugSkeletons[i].GetComponent<WsSkeletonDebug>().UpdateSkeleton(_skeletons[i]);
        }
    }

    public void UpdateWsSkeleton(byte[] bytes)
    {
        string wrappedArray = JsonArrayUtility.EncapsulateInWrapper(System.Text.Encoding.UTF8.GetString(bytes));
        _skeletons = JsonArrayUtility.FromJson<Skeleton>(wrappedArray);
    }
    

    public void OnOpen()
    {

    }

    public void OnClose()
    {

    }
}
