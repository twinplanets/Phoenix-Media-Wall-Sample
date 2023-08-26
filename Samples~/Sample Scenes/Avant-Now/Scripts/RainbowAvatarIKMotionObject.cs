using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowAvatarIKMotionObject : TwinPlanets.PhoenixMediaWall.MotionObject
{
    public GameObject HumanoidAvatarPrefab;
    public Material[] Materials;
    public bool isRandomised = false;

    protected override void UpdateMotionObject()
    {
        base.UpdateMotionObject();
        //Instantiate WS Skeleton for each Skeleton tracked
        for (int i = 0; i < _skeletons.Length; i++)
        {
            if (Skeletons[i] == null)
            {
                Skeletons[i] = Instantiate<GameObject>(HumanoidAvatarPrefab, this.transform);
                Skeletons[i].AddComponent<TwinPlanets.PhoenixMediaWall.IKMotionSkeleton>();
                Skeletons[i].GetComponent<TwinPlanets.PhoenixMediaWall.IKMotionSkeleton>().InstantiateSkeleton(_skeletons[i]);
                Skeletons[i].GetComponent<TwinPlanets.PhoenixMediaWall.IKMotionSkeleton>().scaleFactor = scaleFactor;
                Skeletons[i].transform.position = Vector3.Scale(_skeletons[i].position, scaleFactor);
                if (Materials != null)
                {
                    if(isRandomised)
                    {
                        Skeletons[i].GetComponentInChildren<Renderer>().material = Materials[Random.Range(0, Materials.Length - 1)];
                    }
                    else
                    {
                        int j = 0;
                        if(j >= Materials.Length)
                        {
                            Skeletons[i].GetComponentInChildren<Renderer>().material = Materials[j];
                        }
                        else
                        {
                            j = 0;
                            Skeletons[i].GetComponentInChildren<Renderer>().material = Materials[j];
                        }
                    }
                }
            }
            Skeletons[i].GetComponent<TwinPlanets.PhoenixMediaWall.IKMotionSkeleton>().UpdateSkeleton(_skeletons[i]);
            Skeletons[i].transform.position = Vector3.Scale(_skeletons[i].joints[i], scaleFactor);
        }
    }
}