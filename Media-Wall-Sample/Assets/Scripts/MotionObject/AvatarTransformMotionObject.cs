using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarTransformMotionObject : MotionObject
{
    public GameObject HumanoidAvatarPrefab;

    protected override void UpdateMotionObject()
    {
        base.UpdateMotionObject();
        //Instantiate WS Skeleton for each Skeleton tracked
        for (int i = 0; i < _skeletons.Length; i++)
        {
            if (Skeletons[i] == null)
            {
                Skeletons[i] = Instantiate<GameObject>(HumanoidAvatarPrefab);
                Skeletons[i].AddComponent<AvatarTransformMotionSkeleton>();
                Skeletons[i].GetComponent<AvatarTransformMotionSkeleton>().InstantiateSkeleton(_skeletons[i]);
                Skeletons[i].GetComponent<AvatarTransformMotionSkeleton>().scaleFactor = scaleFactor;
            }
            Skeletons[i].GetComponent<AvatarTransformMotionSkeleton>().UpdateSkeleton(_skeletons[i]);
            Skeletons[i].transform.position = Vector3.Scale(_skeletons[i].joints[i], scaleFactor);
        }
    }
}

[RequireComponent(typeof(Animator))]
public class AvatarTransformMotionSkeleton : MonoBehaviour
{
    //The reference to each bone

    [SerializeField] private Skeleton _skeleton;
    [SerializeField] private Animator _animator;
    [SerializeField] public Vector3 scaleFactor;
    [SerializeField] public Vector3 direct;
    GameObject tempParent;


    public void Start()
    {
        tempParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _animator = GetComponent<Animator>();
    }

    public void InstantiateSkeleton(Skeleton skeleton) { _skeleton = skeleton; }

    public void UpdateSkeleton(Skeleton skeleton)
    {
        _skeleton = skeleton;
    }

    private void LateUpdate()
    {
        Transform originalParent;
        Transform currentBone;
        Transform previousBone;

        Vector3 posA;
        Vector3 posB;
        Vector3 dir;
        Vector3 triangleNormal;
        Quaternion rotation;

        if (_skeleton != null)
        {
            //----------Hips----------\\
            currentBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
            if (currentBone != null)
            {
                // Calculate triangle normal
                triangleNormal = CalculateTriangleNormal(_skeleton.joints[21], _skeleton.joints[17], _skeleton.joints[1]);

                // Calculate rotation to align with triangle normal (without Z-axis rotation)
                rotation = Quaternion.LookRotation(triangleNormal, Vector3.up);

                // Calculate the direction of the upper leg bones
                Vector3 backDirection = (_skeleton.joints[1] - _skeleton.joints[0]).normalized;

                // Calculate the Z rotation angle based on the upper leg direction
                float zRotationOffset = Mathf.Atan2(backDirection.y, backDirection.x) * Mathf.Rad2Deg;

                // Apply the Z rotation to the rotation
                Quaternion zRotation = Quaternion.Euler(0, 0, zRotationOffset-90f);
                //Quaternion finalRotation = rotation;
                Quaternion finalRotation = rotation * zRotation;

                currentBone.transform.rotation = finalRotation;
            }

            //----------Spine----------\\
            previousBone = currentBone;
            currentBone = _animator.GetBoneTransform(HumanBodyBones.Spine);
            if (currentBone != null)
            {
                originalParent = currentBone.parent;

                tempParent.transform.position = currentBone.position;
                tempParent.transform.rotation = previousBone.rotation;

                currentBone.SetParent(tempParent.transform);

                posA = _skeleton.joints[26];
                posB = _skeleton.joints[27];

                dir = (posB - posA).normalized;

                //Rotate tempParent to next bone
                tempParent.transform.LookAt(dir, Vector3.up);
                ////Apply said rotation
                currentBone.rotation = Quaternion.LookRotation(tempParent.transform.forward, Vector3.up);
                currentBone.localScale = Vector3.one;
                //Reset Parent back to original
                currentBone.SetParent(originalParent.transform);
            }

            //----------Chest----------\\
            previousBone = currentBone;
            currentBone = _animator.GetBoneTransform(HumanBodyBones.Chest);
            if (currentBone != null)
            {
                originalParent = currentBone.parent;

                tempParent.transform.position = currentBone.position;
                tempParent.transform.rotation = previousBone.rotation;

                currentBone.SetParent(tempParent.transform);

                posA = _skeleton.joints[2];
                posB = _skeleton.joints[25];

                dir = (posB - posA).normalized;

                //Rotate tempParent to next bone
                tempParent.transform.LookAt(dir, Vector3.up);
                ////Apply said rotation
                currentBone.rotation = Quaternion.LookRotation(tempParent.transform.forward, Vector3.up);
                currentBone.localScale = Vector3.one;
                //Reset Parent back to original
                currentBone.SetParent(originalParent.transform);
            }

            //----------Upper Chest----------\\
            previousBone = currentBone;
            currentBone = _animator.GetBoneTransform(HumanBodyBones.UpperChest);
            if (currentBone != null)
            {
                originalParent = currentBone.parent;

                tempParent.transform.position = currentBone.position;
                tempParent.transform.rotation = previousBone.rotation;

                currentBone.SetParent(tempParent.transform);

                posA = _skeleton.joints[25];
                posB = _skeleton.joints[26];

                dir = (posB - posA).normalized;

                //Rotate tempParent to next bone
                tempParent.transform.LookAt(dir, Vector3.up);
                ////Apply said rotation
                currentBone.rotation = Quaternion.LookRotation(tempParent.transform.forward, Vector3.up);
                currentBone.localScale = Vector3.one;
                //Reset Parent back to original
                currentBone.SetParent(originalParent.transform);
            }

            //----------Neck----------\\
            previousBone = currentBone;
            currentBone = _animator.GetBoneTransform(HumanBodyBones.Neck);
            if (currentBone != null)
            {
                originalParent = currentBone.parent;

                tempParent.transform.position = currentBone.position;
                tempParent.transform.rotation = previousBone.rotation;

                currentBone.SetParent(tempParent.transform);

                posA = _skeleton.joints[1];
                posB = _skeleton.joints[2];

                dir = (posB - posA).normalized;

                //Rotate tempParent to next bone
                tempParent.transform.LookAt(dir, Vector3.up);
                ////Apply said rotation
                currentBone.rotation = Quaternion.LookRotation(tempParent.transform.forward, Vector3.up);
                currentBone.localScale = Vector3.one;
                //Reset Parent back to original
                currentBone.SetParent(originalParent.transform);
            }

            //----------Head----------\\
            triangleNormal = CalculateTriangleNormal(_skeleton.joints[30], _skeleton.joints[28], _skeleton.joints[27]);
            _animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.LookRotation(triangleNormal, Vector3.up));
        }

        //Destroy(tempParent);
    }

    private Vector3 CalculateTriangleNormal(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
    {
        Vector3 AB = vertexB - vertexA;
        Vector3 AC = vertexC - vertexA;

        Vector3 normal = Vector3.Cross(AB, AC).normalized;

        return normal;
    }

    private Vector3 CalculateDirection(Vector3 currentPos, Vector3 targetPos, Vector3 boneForward)
    {
        Vector3 direction = targetPos - currentPos;
        Quaternion rotation = Quaternion.FromToRotation(boneForward, Vector3.forward); // Calculate rotation
        Vector3 transformedDirection = rotation * direction; // Apply rotation to direction
       
        return transformedDirection;
    }



    private Vector3 CalculateLegDirection(Vector3 currentPos, Vector3 targetPos)
    {
        Vector3 direction = targetPos - currentPos;
        direction = new Vector3(-direction.x-45f, -direction.y, -direction.z-45f);
        return direction;
    }

    private Vector3 CalculateArmDirection(Vector3 currentPos, Vector3 targetPos)
    {
        Vector3 direction = targetPos - currentPos;
        direction = new Vector3(direction.y, direction.z, direction.x);
        return direction;
    }
}