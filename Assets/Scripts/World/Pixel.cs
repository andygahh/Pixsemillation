using UnityEngine;

public class Pixel : MonoBehaviour
{
    SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    
    public void Select()
    {
        sprite.color = Color.cyan;
    }

    public void Deselect()
    {
        sprite.color = Color.white;
    }
}
