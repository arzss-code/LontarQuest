using UnityEngine;

public class BlinkText : MonoBehaviour
{
    CanvasGroup cg;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        cg.alpha = Mathf.PingPong(Time.time, 1f);
    }
}