using UnityEngine;
using System.Collections;

public class AryaCutsceneController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private AudioClip footstepSound;

    [SerializeField] private float footstepInterval = 0.35f;

    private float footstepTimer;

    //--------------------------------------------------

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    //--------------------------------------------------

    public void Face(Vector2 direction)
    {
        direction.Normalize();

        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", 0);

        animator.SetFloat("Speed", 0);

        animator.SetFloat("LastMoveX", direction.x);
        animator.SetFloat("LastMoveY", direction.y);
    }

    //--------------------------------------------------

    public IEnumerator MoveTo(Transform target)
    {
        while (Vector2.Distance(
            transform.position,
            target.position) > 0.05f)
        {
            Vector2 dir =
                ((Vector2)target.position -
                 (Vector2)transform.position).normalized;

            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);

            animator.SetFloat("LastMoveX", dir.x);
            animator.SetFloat("LastMoveY", dir.y);

            animator.SetFloat("Speed", 1);

            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    target.position,
                    moveSpeed * Time.deltaTime);

            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                AudioSource.PlayClipAtPoint(
                    footstepSound,
                    transform.position);

                footstepTimer = footstepInterval;
            }

            yield return null;
        }

        animator.SetFloat("Speed", 0);
    }
}