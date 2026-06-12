using UnityEngine;
using UnityEngine.Playables;

public class OpeningManager : MonoBehaviour
{
    public PlayableDirector director;
    public PlayerController player;

    void Start()
    {
        player.SetCanMove(false);

        if (director != null)
        {
            director.Play();
        }
    }

    public void EnablePlayer()
    {
        director.Stop();

        player.SetCanMove(true);

        Debug.Log("PLAYER AKTIF");
    }
}