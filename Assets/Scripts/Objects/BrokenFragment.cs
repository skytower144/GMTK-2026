using TomatoPunch;
using UnityEngine;

public class BrokenFragment : BouncingObject
{
    public void Init(Sprite sprite, float slideSpeed, float initialUpwardForce)
    {
        spriteRenderer.sprite = sprite;
        Init(slideSpeed, initialUpwardForce);
    }
}
