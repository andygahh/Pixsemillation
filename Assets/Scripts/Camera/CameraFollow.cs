using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] PlayerBody playerBody;
    [SerializeField] float initZoomSpeed;
    [SerializeField] float zoomSpeed;

    [SerializeField] int pixelsPerZoomLevel;
    [SerializeField] int zoomAmountPerLevel;

    float startingOrthographicSize = 14f;

    int zoomLevel;

    void LateUpdate()
    {
        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);

        if (Camera.main.orthographicSize < startingOrthographicSize)
        {
            InitZoom(initZoomSpeed);
        }

        int pixelCount = playerBody.GetPixelCount();

        zoomLevel = Mathf.FloorToInt((pixelCount -1) / pixelsPerZoomLevel);

        float targetOrthographicSize = startingOrthographicSize + (zoomLevel * zoomAmountPerLevel);

        float currentOrthographicSize = Camera.main.orthographicSize;

        float newOrthographicSize = Mathf.MoveTowards(currentOrthographicSize, targetOrthographicSize, zoomSpeed * Time.deltaTime);

        Camera.main.orthographicSize = newOrthographicSize;
    }

    private void InitZoom(float speed)
    {
        float targetOrthographicSize = startingOrthographicSize;

        float currentOrthographicSize = Camera.main.orthographicSize;

        float newOrthographicSize = Mathf.MoveTowards(currentOrthographicSize, targetOrthographicSize, speed * Time.deltaTime);

        Camera.main.orthographicSize = newOrthographicSize;
    }
}
