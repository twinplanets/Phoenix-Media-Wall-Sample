using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarIKMotionObject : MotionObject
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
                Skeletons[i].AddComponent<IKMotionSkeleton>();
                Skeletons[i].GetComponent<IKMotionSkeleton>().InstantiateSkeleton(_skeletons[i]);
                Skeletons[i].GetComponent<IKMotionSkeleton>().scaleFactor = scaleFactor;
            }
            Skeletons[i].GetComponent<IKMotionSkeleton>().UpdateSkeleton(_skeletons[i]);
            Skeletons[i].transform.position = Vector3.Scale(_skeletons[i].position, scaleFactor);
        }
    }
}

[RequireComponent(typeof(Animator))]
public class IKMotionSkeleton : MonoBehaviour
{
    //The reference to each bone

    [SerializeField] private Skeleton _skeleton;
    [SerializeField] private Animator _animator;
    [SerializeField] public Vector3 scaleFactor;
    private Quaternion quatAngles;


    public void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void InstantiateSkeleton(Skeleton skeleton) { _skeleton = skeleton; }

    public void UpdateSkeleton(Skeleton skeleton)
    {
        _skeleton = skeleton;

        //Uses the forward vector between two bones to get a rotation value
        #region Rotation Calculation

        //--------Calculate rotation by hips--------
        Vector3 posA = _skeleton.joints[12];
        Vector3 posB = _skeleton.joints[16];

        //Direction and magnitude between point A and point B
        Vector3 dir = posB - posA;

        //Cross product of the direction and the up vector, normalised to remove magnitude
        //Perp = Perpendicular Direction between A and B
        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;

        //Convert to Rotation
        quatAngles = Quaternion.LookRotation(perp, Vector3.up);
        _skeleton.rotation.y = quatAngles.eulerAngles.y;

        //--------Calculate rotation by Spine--------
        posA = _skeleton.joints[0];
        posB = _skeleton.joints[2];

        //Direction and magnitude between point A and point B
        dir = posB - posA;

        //Cross product of the direction and the up vector, normalised to remove magnitude
        //Perp = Perpendicular Direction between A and B
        perp = Vector3.Cross(dir, Vector3.left).normalized;

        //Convert to Rotation
        quatAngles = Quaternion.LookRotation(perp, Vector3.left);

        _skeleton.rotation.x = quatAngles.eulerAngles.x;

        quatAngles = Quaternion.Euler(_skeleton.rotation);

        #endregion
        
    }

    void OnAnimatorIK()
    {
        if (_skeleton != null)
        {
            Vector3 triangleNormal;
            Transform currentBone;
            Quaternion rotation;

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
                Quaternion zRotation = Quaternion.Euler(0, 0, zRotationOffset - 90f);
                //Quaternion finalRotation = rotation;
                Quaternion finalRotation = rotation * zRotation;

                _animator.bodyRotation = finalRotation;
            }

            //----------Head----------\\
            triangleNormal = CalculateTriangleNormal(_skeleton.joints[28], _skeleton.joints[30], _skeleton.joints[27]);
            _animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.LookRotation(triangleNormal, Vector3.up));

            //Right Hand
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, (Vector3.Scale(_skeleton.joints[7], scaleFactor)));

            //Right Foot
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, (Vector3.Scale(_skeleton.joints[19], scaleFactor)));

            //Left Hand
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, (Vector3.Scale(_skeleton.joints[14], scaleFactor)));

            //Left Foot
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, (Vector3.Scale(_skeleton.joints[23], scaleFactor)));

        }
    }

    private Vector3 CalculateTriangleNormal(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
    {
        Vector3 AB = vertexB - vertexA;
        Vector3 AC = vertexC - vertexA;

        Vector3 normal = Vector3.Cross(AB, AC).normalized;

        return normal;
    }
}