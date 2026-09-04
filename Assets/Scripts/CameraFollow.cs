using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetTransform;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);

        int pixelCount = PlayerMovement.pixelCount;

        float visibleGridSize = (pixelCount * 2) + 3;

        float targetOrthographicSize = visibleGridSize / 2;

        Camera.main.orthographicSize = targetOrthographicSize;
    }
}
