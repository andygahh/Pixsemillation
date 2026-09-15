using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] PlayerBody playerBody;
    [SerializeField] float zoomSpeed;

    void LateUpdate()
    {
        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);

        int pixelCount = playerBody.GetPixelCount();

        float visibleGridSize = (pixelCount * 2) + 3;

        float targetOrthographicSize = visibleGridSize / 2;

        float currentOrthographicSize = Camera.main.orthographicSize;

        float newOrthographicSize = Mathf.MoveTowards(currentOrthographicSize, targetOrthographicSize, zoomSpeed * Time.deltaTime);

        Camera.main.orthographicSize = newOrthographicSize;
    }
}
