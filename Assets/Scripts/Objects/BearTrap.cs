using UnityEngine;

public class BearTrap : Trap
{
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationClip activatedClip, normalizeClip;
    [SerializeField] private float trapDuration;

    protected override void ActivateTrap()
    {
        anim.Play(activatedClip.name, -1, 0f);
        gameControl.PlayerControl.Trapped(transform.position, trapDuration);

        Invoke(nameof(Normalize), trapDuration);
    }

    private void Normalize()
    {
        anim.Play(normalizeClip.name, -1, 0f);
    }
}
