using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropMode : MonoBehaviour
{
    List<Vector2Int> pixelSelection = new List<Vector2Int>();

    bool isActive = false;

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (isActive)
            {
                ExitDropMode();
            }
            else
            {
                EnterDropMode();
            }
        }

        if (!isActive)
        {
            return;
        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
                Vector2Int mouseGridPosition = new Vector2Int();

                Collider2D hitCollider;

                mouseGridPosition.x = Mathf.RoundToInt(mouseWorldPosition.x);
                mouseGridPosition.y = Mathf.RoundToInt(mouseWorldPosition.y);

                Debug.Log(mouseGridPosition);

                hitCollider = Physics2D.OverlapPoint(mouseWorldPosition);

                if (hitCollider != null)
                {
                    Debug.Log(hitCollider.name);

                    if (hitCollider.transform.IsChildOf(transform))
                    {
                        Debug.Log("Player pixel selected");
                        
                        Vector2Int clickedPixelPosition = new Vector2Int();
                        Vector2Int playerCorePosition = new Vector2Int();
                        Vector2Int selectedPixelPosition = new Vector2Int();

                        Pixel pixel = hitCollider.GetComponent<Pixel>();

                        clickedPixelPosition.x = Mathf.RoundToInt(hitCollider.transform.position.x);
                        clickedPixelPosition.y = Mathf.RoundToInt(hitCollider.transform.position.y);

                        playerCorePosition.x = Mathf.RoundToInt(transform.position.x);
                        playerCorePosition.y = Mathf.RoundToInt(transform.position.y);

                        selectedPixelPosition = clickedPixelPosition - playerCorePosition;

                        if (Keyboard.current.ctrlKey.IsActuated())
                        {
                            if (pixelSelection.Contains(selectedPixelPosition))
                            {
                                pixelSelection.Remove(selectedPixelPosition);
                                pixel.Deselect();
                            }
                            else
                            {
                                pixelSelection.Add(selectedPixelPosition);
                                pixel.Select();
                            }
                        }
                        else
                        {
                            ClearSelection();
                            pixelSelection.Add(selectedPixelPosition);
                            pixel.Select();
                        }
                        
                        

                        Debug.Log(string.Join(", ", pixelSelection));
                    }
                }
                else
                {
                    Debug.Log("Nothing There");
                }
            }
        }
    }

    public bool GetDropModeStatus()
    {
        return isActive;
    }
    
    public void EnterDropMode()
    {
        Time.timeScale = 0;
        isActive = true;
        Debug.Log("DROP MODE ON");
    }

    public void ExitDropMode()
    {
        Time.timeScale = 1;
        isActive = false;
        ClearSelection();
        Debug.Log("DROP MODE OFF");
    }

    public void ClearSelection()
    {
        Pixel[] playerPixels = GetComponentsInChildren<Pixel>();

        foreach (Pixel pixel in playerPixels)
        {
            pixel.Deselect();
        }
        
        pixelSelection.Clear();
    }
}
