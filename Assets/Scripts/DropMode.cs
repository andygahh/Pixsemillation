using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropMode : MonoBehaviour
{
    List<Vector2Int> pixelSelection = new List<Vector2Int>();
    List<Vector2Int> remainingOnBody;

    PlayerBody currentBody;

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
            MakeSelection();

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if (ValidateSelection())
                {
                    CreateDroppedCluster();
                }
            }
        }
    }

    public bool GetDropModeStatus()
    {
        return isActive;
    }
    
    private void EnterDropMode()
    {
        Time.timeScale = 0;
        isActive = true;

        currentBody = GetComponent<PlayerBody>();

    }

    private void ExitDropMode()
    {
        Time.timeScale = 1;
        isActive = false;
        ClearSelection();
    }

    private void ClearSelection()
    {
        Pixel[] playerPixels = GetComponentsInChildren<Pixel>();

        foreach (Pixel pixel in playerPixels)
        {
            pixel.Deselect();
        }

        pixelSelection.Clear();
    }

    private void MakeSelection()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Vector2Int mouseGridPosition = new Vector2Int();

            Collider2D hitCollider;

            mouseGridPosition.x = Mathf.RoundToInt(mouseWorldPosition.x);
            mouseGridPosition.y = Mathf.RoundToInt(mouseWorldPosition.y);

            hitCollider = Physics2D.OverlapPoint(mouseWorldPosition);

            if (hitCollider != null)
            {
                if (hitCollider.transform.IsChildOf(transform))
                {
                    Vector2Int clickedPixelPosition = new Vector2Int();
                    Vector2Int playerCorePosition = new Vector2Int();
                    Vector2Int selectedPixelPosition = new Vector2Int();

                    Pixel pixel = hitCollider.GetComponent<Pixel>();

                    if (pixel == null)
                    {
                        return;
                    }

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
                    
                    

                }
            }
        }
    }

    private bool ValidateSelection()
    {
        if (pixelSelection.Count == 0)
        {
            return false;
        }
        else
        {
            remainingOnBody = new List<Vector2Int>(currentBody.GetPixelPositions());

            BuildRemainingBody();

            return IsConnected(pixelSelection) && IsConnected(remainingOnBody, new Vector2Int (0,0));
        }
    }

    private void BuildRemainingBody()
    {
        foreach (Vector2Int pixel in pixelSelection)
        {
            remainingOnBody.Remove(pixel);
        }
    }

    private bool IsConnected(List<Vector2Int> pixels)
    {
        List<Vector2Int> checkedPixels = new List<Vector2Int>();
        List<Vector2Int> pixelsToCheck = new List<Vector2Int>();

        checkedPixels.Add(pixels[0]);
        pixelsToCheck.Add(pixels[0]);

        while (pixelsToCheck.Count != 0)
        {
            Vector2Int current = pixelsToCheck[0];
            pixelsToCheck.Remove(current);

            for (int i = 0; i < pixels.Count; i++)
            {
                if (checkedPixels.Contains(pixels[i]))
                {
                    continue;
                }
                else if (CheckAdjacent(current, pixels[i]))
                {
                    checkedPixels.Add(pixels[i]);
                    pixelsToCheck.Add(pixels[i]);
                }
            }
        }

        return checkedPixels.Count == pixels.Count;
    }

    private bool IsConnected(List<Vector2Int> pixels, Vector2Int startingPosition)
    {
        List<Vector2Int> checkedPixels = new List<Vector2Int>();
        List<Vector2Int> pixelsToCheck = new List<Vector2Int>();

        checkedPixels.Add(startingPosition);
        pixelsToCheck.Add(startingPosition);

        while (pixelsToCheck.Count != 0)
        {
            Vector2Int current = pixelsToCheck[0];
            pixelsToCheck.Remove(current);

            for (int i = 0; i < pixels.Count; i++)
            {
                if (checkedPixels.Contains(pixels[i]))
                {
                    continue;
                }
                else if (CheckAdjacent(current, pixels[i]))
                {
                    checkedPixels.Add(pixels[i]);
                    pixelsToCheck.Add(pixels[i]);
                }
            }
        }

        return checkedPixels.Count == pixels.Count;
    }

    private bool CheckAdjacent(Vector2Int pixel, Vector2Int selected)
    {
        List<Vector2Int> adjacents = GetAdjacents();

        foreach (Vector2Int position in adjacents)
        {
            if (pixel + position == selected)
            {
                return true;
            }
        }

        return false;
    }

    private List<Vector2Int> GetAdjacents()
    {
        List<Vector2Int> adjacents = new List<Vector2Int>()
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),

            new Vector2Int(1,1),
            new Vector2Int(1,-1),
            new Vector2Int(-1,1),
            new Vector2Int(-1,-1),
        };

        return adjacents;
    }

    private List<Pixel> FindSelectedPixelObjects()
    {
        Pixel[] bodyPixels = GetComponentsInChildren<Pixel>();
        List<Pixel> selectedPixels = new List<Pixel>();

        Vector2Int playerCorePosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        Vector2Int currentPixelPosition = new Vector2Int();
        Vector2Int relativePosition = new Vector2Int();

        foreach (Pixel bodyPixel in bodyPixels)
        {
            currentPixelPosition.x = Mathf.RoundToInt(bodyPixel.transform.position.x);
            currentPixelPosition.y = Mathf.RoundToInt(bodyPixel.transform.position.y);

            relativePosition = currentPixelPosition - playerCorePosition;

            if (pixelSelection.Contains(relativePosition))
            {
                selectedPixels.Add(bodyPixel);
            }
        }

        return selectedPixels;
    }

    private void CreateDroppedCluster()
    {
        List<Pixel> selectedPixels = FindSelectedPixelObjects();

        GameObject worldCluster = new GameObject("WorldCluster");

        WorldCluster droppedCluster = worldCluster.AddComponent<WorldCluster>();

        currentBody.RemovePixels(pixelSelection);

        worldCluster.transform.position = selectedPixels[0].transform.position;

        foreach (Pixel pixel in selectedPixels)
        {
            pixel.Deselect();
            pixel.transform.SetParent(worldCluster.transform, true);
        }

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        playerMovement.AddWorldCluster(droppedCluster);

        ClearSelection();
    }
}
