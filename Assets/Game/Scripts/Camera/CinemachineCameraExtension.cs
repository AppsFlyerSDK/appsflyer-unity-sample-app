using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineCameraExtension : MonoBehaviour
{

    #region Fields

    private Vector3 initailPosition;
    private Quaternion initialRotation;

    #endregion

    #region Methods
    public void Reset(Transform tranform)
    {
        transform.position = tranform.position;
        transform.rotation = tranform.rotation;
    }
    #endregion
}
