using UnityEngine;

public static class SkeletonConversion
{
    public static string Convert(ushort i)
    {
        switch (i)
        {
            default:
                return "name";
        }
    }
}

[System.Serializable]
public class Skeleton
{
    public Vector3[] joints;
    public Vector3 position;
    public Vector3 rotation;
}

public class MotionObject : MonoBehaviour
{
    public Vector3 scaleFactor;
    [SerializeField] protected Skeleton[] _skeletons;
    [SerializeField] protected GameObject[] Skeletons;
    //public bool debugSkeleton = false;
    //public GameObject wsSkeleton;

    private void Start()
    {
        if (scaleFactor == Vector3.zero) scaleFactor = new Vector3(0.01f, 0.01f, 0.01f);
    }

    private void LateUpdate()
    {
        //Check if Skeleton data has been received
        if (_skeletons.Length != 0)
        {
            //Checks the size of _skeletons against Skeletons to ensure the arrays match in number of captured skeletons
            #region ArrayResizing
            //Delete excess skeletons
            if (Skeletons.Length > _skeletons.Length)
            {
                for (int i = Skeletons.Length - 1; i > _skeletons.Length - 1; i--)
                {
                    Debug.Log(i);
                    Destroy(Skeletons[i].gameObject);
                }
                System.Array.Resize(ref Skeletons, _skeletons.Length);
            }

            //Expand array to fit skeletons
            if (Skeletons.Length < _skeletons.Length)
            {
                System.Array.Resize(ref Skeletons, _skeletons.Length);
            }
            #endregion 

            ////Instantiate WS Skeleton for each Skeleton tracked
            //for (int i = 0; i < _skeletons.Length; i++)
            //{
            //    if (Skeletons[i] == null)
            //    {
            //        Skeletons[i] = Instantiate<GameObject>(wsSkeleton);
            //        Skeletons[i].GetComponent<WsSkeleton>().InstantiateSkeleton(_skeletons[i]);
            //    }
            //    Skeletons[i].GetComponent<WsSkeleton>().UpdateSkeleton(_skeletons[i]);
            //}

            UpdateMotionObject();
        }
    }

    protected virtual void UpdateMotionObject()
    {
        //Override this to make your own Motion Object
    }

    //Json Parser that saves streamed in data from the WsClient and saves it as an Object
    public void UpdateData(byte[] bytes)
    {
        string wrappedArray = JsonArrayUtility.EncapsulateInWrapper(System.Text.Encoding.UTF8.GetString(bytes));
        _skeletons = JsonArrayUtility.FromJson<Skeleton>(wrappedArray);
    }
    
}
