using System.Collections.Generic;
using UnityEngine;

public class FragmentExplodeEffect : MonoBehaviour
{
    [SerializeField, Header("X: MIN, Y: MAX")] private Vector2 fragmentSlideSpeed;
    [SerializeField, Header("X: MIN, Y: MAX")] private Vector2 fragmentUpwardForce;

    [SerializeField] private BrokenFragment fragmentPrefab;
    [SerializeField] private List<Sprite> fragmentSprites;

    public void Explode()
    {
        for (int i = 0; i < fragmentSprites.Count; i++)
        {
            BrokenFragment frag = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
            frag.transform.SetParent(transform.parent);

            float slideSpeed = Random.Range(fragmentSlideSpeed.x, fragmentSlideSpeed.y);
            float initialUpwardForce = Random.Range(fragmentUpwardForce.x, fragmentUpwardForce.y);

            frag.Init(fragmentSprites[i], slideSpeed, initialUpwardForce);
        }
    }
}
