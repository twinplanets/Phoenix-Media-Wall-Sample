using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMotionObject : MotionObject
{
    ////Render simple geometry to debug the skeleton data
    protected override void UpdateMotionObject()
    {
        base.UpdateMotionObject();
        if (Skeletons.Length > _skeletons.Length)
        {
            //Destroy extra skeletons
            for (int i = Skeletons.Length - 1; i > _skeletons.Length - 1; i--)
            {
                Debug.Log(i);
                Destroy(Skeletons[i].gameObject);
            }
            System.Array.Resize(ref Skeletons, _skeletons.Length);
        }

        //Exapnd array if too short
        if (Skeletons.Length < _skeletons.Length)
        {
            System.Array.Resize(ref Skeletons, _skeletons.Length);
        }

        //Create parent for each skeleton and attach WS Skeleton Debugging component
        for (int i = 0; i < _skeletons.Length; i++)
        {
            if (Skeletons[i] == null)
            {
                Skeletons[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Skeletons[i].AddComponent<SkeletonDebug>();
                Skeletons[i].GetComponent<SkeletonDebug>().InstantiateSkeleton(_skeletons[i]);
            }
            Skeletons[i].GetComponent<SkeletonDebug>().UpdateSkeleton(_skeletons[i]);
        }
    }

}

//Creates A skeleton with Spheres represeneting Bone Positions
public class SkeletonDebug : MonoBehaviour
{
    [SerializeField] private Skeleton _skeleton;
    public GameObject[] debugObjects;
    public Vector3 scaleFactor;
    public Vector3 forward;

    public void Start()
    {
        if (scaleFactor == Vector3.zero) scaleFactor = new Vector3(0.01f, 0.01f, 0.01f);
    }
    public void InstantiateSkeleton(Skeleton skeleton)
    {
        _skeleton = skeleton;

        //Create each bone and name them
        System.Array.Resize<GameObject>(ref debugObjects, 35);
        for (int i = 0; i < 35; i++)
        {
            debugObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugObjects[i].transform.parent = transform;
            debugObjects[i].name = $"Bone: {i + 1}";
        }
    }

    public void UpdateSkeleton(Skeleton skeleton)
    {
        _skeleton = skeleton;
        //Update bone positions
        for (int i = 0; i < 35; i++)
        {
            debugObjects[i].transform.position = Vector3.Scale(skeleton.joints[i], scaleFactor);
        }
        transform.position = Vector3.Scale(_skeleton.position, scaleFactor);
    }


}

