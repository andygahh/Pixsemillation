using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    int[] currentPosition = new int[] {0, 0};
    int[] movement = new int[] {0, 0};

    // Update is called once per frame
    void Update()
    {
        movement[0] = 0;
        movement[1] = 0;

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            movement[1] += 1;
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            movement[1] -= 1;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            movement[0] -= 1;
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            movement[0] += 1;
        }

        if (movement[0] != 0 || movement[1] != 0)
        {
            currentPosition[0] += movement[0];
            currentPosition[1] += movement[1];

            transform.position = new Vector3(currentPosition[0], currentPosition[1], 0);
        }


    }
}
