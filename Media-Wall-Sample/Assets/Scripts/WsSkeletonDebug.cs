using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WsSkeletonDebug : MonoBehaviour
{
    [SerializeField] private Skeleton _skeleton;
    public GameObject[] debugObjects;
    public Vector3 scaleFactor;

    public void Start()
    {
        if(scaleFactor == Vector3.zero) scaleFactor = new Vector3(0.01f, 0.01f, 0.01f);
    }
    public void InstantiateSkeleton(Skeleton skeleton)
    {
        _skeleton = skeleton;
        System.Array.Resize<GameObject>(ref debugObjects, 35);
        for (int i = 34; i >= 0; i--)
        {
            debugObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugObjects[i].transform.parent = transform;
        }
    }

    public void UpdateSkeleton(Skeleton skeleton)
    {
        _skeleton = skeleton;
        for (int i = 0; i < 35; i++)
        {
            debugObjects[i].transform.position = Vector3.Scale(skeleton.joints[i], scaleFactor);
        }
        transform.position = Vector3.Scale(_skeleton.position, scaleFactor);
    }


}
