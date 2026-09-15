using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] PlayerBody playerBody;
    [SerializeField] float zoomSpeed;

    int zoomLevel;
    int pixelsPerZoomLevel = 5;
    int zoomAmountPerLevel = 1;
    float startingOrthographicSize = 2.5f;
    

    void LateUpdate()
    {
        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);

        int pixelCount = playerBody.GetPixelCount();

        zoomLevel = Mathf.FloorToInt((pixelCount -1) / pixelsPerZoomLevel);

        float targetOrthographicSize = startingOrthographicSize + (zoomLevel * zoomAmountPerLevel);

        float currentOrthographicSize = Camera.main.orthographicSize;

        float newOrthographicSize = Mathf.MoveTowards(currentOrthographicSize, targetOrthographicSize, zoomSpeed * Time.deltaTime);

        Camera.main.orthographicSize = newOrthographicSize;
    }
}
