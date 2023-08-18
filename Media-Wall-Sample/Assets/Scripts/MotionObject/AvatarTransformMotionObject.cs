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
    private Quaternion quatAngles;
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

    private void LateUpdate()
    {
        Transform originalParent;
        Transform currentBone;

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
            if (currentBone != null)
            {
                currentBone = _animator.GetBoneTransform(HumanBodyBones.Spine);
                originalParent = currentBone.parent;

                tempParent.transform.position = Vector3.Scale(_skeleton.joints[1], scaleFactor);
                tempParent.transform.LookAt(currentBone.up, Vector3.up);

                currentBone.SetParent(tempParent.transform);

                posA = _skeleton.joints[1];
                posB = _skeleton.joints[2];

                dir = (posB - posA).normalized;

                //Rotate tempParent to next bone
                tempParent.transform.LookAt(dir, Vector3.up);
                ////Apply said rotation
                //currentBone.rotation = Quaternion.LookRotation(tempParent.transform.forward, Vector3.up);
                //Reset Parent back to original
                currentBone.SetParent(originalParent.transform);
            }

            //----------Chest----------\\
            if (currentBone != null)
            {
                currentBone = _animator.GetBoneTransform(HumanBodyBones.Chest);
                originalParent = currentBone.parent;

                tempParent.transform.position = Vector3.Scale(_skeleton.joints[2], scaleFactor);
                tempParent.transform.LookAt(currentBone.up, Vector3.up);

                currentBone.SetParent(tempParent.transform);

                posA = _skeleton.joints[2];
                posB = _skeleton.joints[25];

                dir = (posB - posA).normalized;

                //Rotate tempParent to next bone
                tempParent.transform.LookAt(dir, Vector3.up);
                ////Apply said rotation
                //currentBone.rotation = Quaternion.LookRotation(tempParent.transform.forward, Vector3.up);
                //Reset Parent back to original
                currentBone.SetParent(originalParent.transform);
            }

            //----------Upper Chest----------\\
            if (currentBone != null)
            {
                currentBone = _animator.GetBoneTransform(HumanBodyBones.UpperChest);
                originalParent = currentBone.parent;

                tempParent.transform.position = Vector3.Scale(_skeleton.joints[25], scaleFactor);
                tempParent.transform.LookAt(currentBone.up, Vector3.up);

                currentBone.SetParent(tempParent.transform);

                posA = _skeleton.joints[25];
                posB = _skeleton.joints[26];

                dir = (posB - posA).normalized;

                //Rotate tempParent to next bone
                tempParent.transform.LookAt(dir, Vector3.up);
                ////Apply said rotation
                //currentBone.rotation = Quaternion.LookRotation(tempParent.transform.forward, Vector3.up);
                //Reset Parent back to original
                currentBone.SetParent(originalParent.transform);
            }

            //----------Neck----------\\
            if (currentBone != null)
            {
                currentBone = _animator.GetBoneTransform(HumanBodyBones.Neck);
                originalParent = currentBone.parent;

                tempParent.transform.position = Vector3.Scale(_skeleton.joints[26], scaleFactor);
                tempParent.transform.LookAt(currentBone.up, Vector3.up);

                currentBone.SetParent(tempParent.transform);

                posA = _skeleton.joints[26];
                posB = _skeleton.joints[27];

                dir = (posB - posA).normalized;

                //Rotate tempParent to next bone
                tempParent.transform.LookAt(dir, Vector3.up);
                ////Apply said rotation
                //currentBone.rotation = Quaternion.LookRotation(tempParent.transform.forward, Vector3.up);
                //Reset Parent back to original
                currentBone.SetParent(originalParent.transform);
            }

            //----------Head----------\\
            triangleNormal = CalculateTriangleNormal(_skeleton.joints[30], _skeleton.joints[28], _skeleton.joints[27]);
            _animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.LookRotation(triangleNormal, Vector3.up));
        }

        //Destroy(tempParent);
    }

    void OnAnimatorIK()
    {
        if (_skeleton != null)
        {
            Vector3 vectorAB;
            Vector3 vectorAC;
            Vector3 triangleNormal;
            Vector3 boneForward;
            Vector3 direction;
            ////--------Calculate rotation by hips--------
            //Vector3 posA = _skeleton.joints[17];
            //Vector3 posB = _skeleton.joints[21];

            ////Direction and magnitude between point A and point B
            //Vector3 dir = posB - posA;

            ////Cross product of the direction and the up vector, normalised to remove magnitude
            ////Perp = Perpendicular Direction between A and B
            //Vector3 hipPerp = Vector3.Cross(dir, Vector3.up).normalized;

            //posA = _skeleton.joints[0];
            //posB = _skeleton.joints[1];

            //dir = posB - posA;
            //dir = dir.normalized;

            //hipPerp.x = dir.x;
            //hipPerp.z = dir.z;

            //Quaternion hipPerpQ = Quaternion.Euler(hipPerp);
            //            //Left_Foot
            //            direction = CalculateDirection(_skeleton.joints[34], _skeleton.joints[23]);
            //            ////Cross product of the direction and the up vector, normalised to remove magnitude
            //            direction = Vector3.Cross(direction, Vector3.left).normalized;
            //            //direction.x += 45f;
            //            _animator.SetBoneLocalRotation(HumanBodyBones.LeftFoot, Quaternion.LookRotation(direction, Vector3.up));
            //            //Left_Leg
            //            direction = CalculateDirection(_skeleton.joints[23], _skeleton.joints[22]);
            //            _animator.SetBoneLocalRotation(HumanBodyBones.LeftLowerLeg, Quaternion.LookRotation(direction, Vector3.up));
            //            //Left_UpperLeg
            //            direction = CalculateDirection(_skeleton.joints[22], _skeleton.joints[21]);
            //            direction.x += 45f;
            //            _animator.SetBoneLocalRotation(HumanBodyBones.LeftUpperLeg, Quaternion.LookRotation(direction, Vector3.down));

            //            //Right_Foot
            //            direction = CalculateDirection(_skeleton.joints[33], _skeleton.joints[19]);
            //            ////Cross product of the direction and the up vector, normalised to remove magnitude
            //            direction = Vector3.Cross(direction, Vector3.left).normalized;
            //            //direction.x += 45f;
            //            _animator.SetBoneLocalRotation(HumanBodyBones.RightFoot, Quaternion.LookRotation(direction, Vector3.up));
            //;
            //            //Right_Leg
            //            direction = CalculateDirection(_skeleton.joints[19], _skeleton.joints[18]);
            //            _animator.SetBoneLocalRotation(HumanBodyBones.RightLowerLeg, Quaternion.LookRotation(direction, Vector3.up));
            //            //Right_UpperLeg
            //            direction = CalculateDirection(_skeleton.joints[18], _skeleton.joints[17]);
            //            direction.x += 45f;
            //            _animator.SetBoneLocalRotation(HumanBodyBones.RightUpperLeg, Quaternion.LookRotation(direction, Vector3.down));


            //Hips
            //vectorAB = CalculateDirection(_skeleton.joints[21], _skeleton.joints[17], Vector3.up);
            //direction = CalculateDirection(_skeleton.joints[0], _skeleton.joints[1], Vector3.up);
            //Debug.Log(vectorAB);
            //Quaternion finalRotation = Quaternion.LookRotation(direction, Vector3.up);
            //Quaternion zRotation = Quaternion.Euler(0, vectorAB.y, 0);
            //finalRotation = finalRotation * zRotation;
            //_animator.bodyRotation = finalRotation;
            //Debug.Log(_animator.GetBoneTransform(HumanBodyBones.Spine).up);
            //Spine_1
            //direction = CalculateDirection(_skeleton.joints[1], _skeleton.joints[2], Vector3.up);
            //_animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.LookRotation(direction, Vector3.up));
            //Spine_2
            //direction = CalculateDirection(_skeleton.joints[2], _skeleton.joints[25], Vector3.up);
            //_animator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.LookRotation(direction, Vector3.up));
            //Spine_3
            //direction = CalculateDirection(_skeleton.joints[25], _skeleton.joints[26], Vector3.up);
            //_animator.SetBoneLocalRotation(HumanBodyBones.UpperChest, Quaternion.LookRotation(direction, Vector3.up));
            //Neck
            //direction = CalculateDirection(_skeleton.joints[26], _skeleton.joints[27], Vector3.up);
            //_animator.SetBoneLocalRotation(HumanBodyBones.Neck, Quaternion.LookRotation(direction, Vector3.up));
            //Head
            //vectorAB = ((_skeleton.joints[28] + _skeleton.joints[30]) / 2) - _skeleton.joints[27];
            //vectorAC = _skeleton.joints[30] - _skeleton.joints[28];
            //triangleNormal = Vector3.Cross(vectorAB, vectorAC).normalized;
            //_animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.LookRotation(triangleNormal, Vector3.up));

            ////Left_Shoulder
            //direction = CalculateDirection(_skeleton.joints[10], _skeleton.joints[11], Vector3.up);
            //_animator.SetBoneLocalRotation(HumanBodyBones.LeftShoulder, Quaternion.LookRotation(direction, direct));

            ////Right_Shoulder
            //direction = CalculateDirection(_skeleton.joints[3], _skeleton.joints[4], Vector3.up);
            //_animator.SetBoneLocalRotation(HumanBodyBones.RightShoulder, Quaternion.LookRotation(direction, direct));

            ////Left_Hand
            //direction = CalculateDirection(_skeleton.joints[13], _skeleton.joints[15]);
            //_animator.SetBoneLocalRotation(HumanBodyBones.LeftHand, Quaternion.LookRotation(direction, Vector3.up));
            ////Left_Forearm
            //direction = CalculateDirection(_skeleton.joints[12], _skeleton.joints[13]);
            //_animator.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.LookRotation(direction, Vector3.up));
            ////Left_Arm
            //direction = CalculateDirection(_skeleton.joints[11], _skeleton.joints[12]);
            //_animator.SetBoneLocalRotation(HumanBodyBones.LeftUpperArm, Quaternion.LookRotation(direction, Vector3.up));

            ////Right_Hand
            //direction = CalculateDirection(_skeleton.joints[6], _skeleton.joints[8]);
            //_animator.SetBoneLocalRotation(HumanBodyBones.RightHand, Quaternion.LookRotation(direction, Vector3.up));
            ////Right_Forearm
            //direction = CalculateDirection(_skeleton.joints[5], _skeleton.joints[6]);
            //_animator.SetBoneLocalRotation(HumanBodyBones.RightLowerArm, Quaternion.LookRotation(direction, Vector3.up));
            ////Right_Arm
            //_animator.SetBoneLocalRotation(HumanBodyBones.RightUpperArm, Quaternion.LookRotation(direction, Vector3.up));
            //direction = CalculateDirection(_skeleton.joints[5], _skeleton.joints[4]);
        }
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