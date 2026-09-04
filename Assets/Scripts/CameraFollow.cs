using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetTransform;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);
    }
}
