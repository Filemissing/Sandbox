using UnityEngine;

public class BuildField : MonoBehaviour
{
    [SerializeField] Vector2Int size;

    bool[,] field; // true is occupied, false is free
    private void Awake()
    {
        field = new bool[size.x, size.y];
    }

    public void AddPart(Part part, Vector2Int position)
    {
        bool canPlace = true;
        RectInt space = part.space;
        foreach(Vector2Int pos in space.allPositionsWithin)
        {
            Vector2Int adjustedPos = Vector2Int.RoundToInt(space.center) + pos;
            if (field[adjustedPos.x, adjustedPos.y])
            {
                canPlace = false; 
                break;
            }
        }
        if (canPlace)
        {
            foreach (Vector2Int pos in space.allPositionsWithin)
            {
                Vector2Int adjustedPos = Vector2Int.RoundToInt(space.center) + pos;
                field[adjustedPos.x, adjustedPos.y] = true;
            }
        }
    }
}
