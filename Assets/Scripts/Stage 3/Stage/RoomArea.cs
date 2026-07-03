using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomArea : MonoBehaviour
{
    private BoxCollider2D boundary;

    private BoxCollider2D Boundary
    {
        get
        {
            if (boundary == null)
                boundary = GetComponent<BoxCollider2D>();

            return boundary;
        }
    }

    public bool Contains(Vector2 point)
    {
        return Boundary.OverlapPoint(point);
    }

    public Bounds GetBounds()
    {
        return Boundary.bounds;
    }
}