using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BreakableObject : MonoBehaviour
{
    [SerializeField] private int durability;
    [SerializeField] private DOTweenAnimation shakeTween;
    [SerializeField] private FragmentExplodeEffect explodeEffect;

    private bool isDestroyed;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed)
            return;
        
        // compare tag

        TakeDamage();
    }

    private void TakeDamage()
    {
        --durability;
        shakeTween.DORewind();
        shakeTween.DOPlay();

        if (durability <= 0)
        {
            shakeTween.DOComplete();
            shakeTween.DOKill();
            Break();
        }
    }

    private void Break()
    {
        isDestroyed = true;
        explodeEffect.Explode();
        Destroy(gameObject);
    }
}
