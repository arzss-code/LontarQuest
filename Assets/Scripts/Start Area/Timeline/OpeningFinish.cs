using UnityEngine;

public class OpeningFinish : MonoBehaviour
{
    public PlayerController player;

    public void EndOpening()
    {
        player.SetCanMove(true);
    }
}