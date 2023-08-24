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
    [SerializeField] [Min(1)] protected int maxSkeletons = 5;
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
            Debug.Log("Skeletons Detected. Quantity: " + _skeletons.Length);
            //Checks the size of _skeletons against Skeletons to ensure the arrays match in number of captured skeletons
            #region ArrayResizing

            int maxLength = Mathf.Clamp(_skeletons.Length, 0, maxSkeletons);

            //Delete excess skeletons
            if (Skeletons.Length > maxLength)
            {
                for (int i = Skeletons.Length-1; i > maxLength-1; i--)
                {
                    Destroy(Skeletons[i].gameObject);
                }
                System.Array.Resize(ref Skeletons, maxLength);
            }

            //Expand array to fit skeletons
            if (Skeletons.Length < maxSkeletons)
            {
                System.Array.Resize(ref Skeletons, maxLength);
            }
            #endregion 

            UpdateMotionObject();
        }
        else
        {
            foreach (var skelly in Skeletons)
            {
                Destroy(skelly.gameObject);
            }
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
