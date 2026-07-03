using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomArea : MonoBehaviour
{
    public BoxCollider2D Boundary { get; private set; }

    private void Awake()
    {
        Boundary = GetComponent<BoxCollider2D>();
    }

    public bool Contains(Vector2 position)
    {
        return Boundary.OverlapPoint(position);
    }
}