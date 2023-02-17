using System;

using UnityEngine;


public class CapsuleColliderData 
{
    public CapsuleCollider Collider { get; private set; }
    public Vector3 ColliderCenterInLocalSpace { get; private set; }
    public Vector3 ColliderVerticalExtends { get; private set; }


    public void Initialize(GameObject obj)
    {
        if (Collider != null)
        {
            return;
        }

        Collider = obj.GetComponent<CapsuleCollider>();
        UpdateColliderData();
    }

    public void UpdateColliderData()
    {
        ColliderCenterInLocalSpace = Collider.center;
        ColliderVerticalExtends = new Vector3(0f, Collider.bounds.extents.y, 0f);
    }
}
