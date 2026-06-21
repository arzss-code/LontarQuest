using TMPro;
using UnityEngine;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public TMP_Text notificationText;

    private void Awake()
    {
        Instance = this;
        notificationText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        StartCoroutine(ShowRoutine(message));
    }

    IEnumerator ShowRoutine(string message)
    {
        notificationText.gameObject.SetActive(true);

        notificationText.text = message;

        yield return new WaitForSeconds(3f);

        notificationText.gameObject.SetActive(false);
    }
}