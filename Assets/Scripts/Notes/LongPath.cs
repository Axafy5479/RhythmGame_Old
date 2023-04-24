using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPath : MonoBehaviour
{
    [SerializeField] private Transform topTrn;
    [SerializeField] private Transform endTrn;
    private Transform trn;
    private SpriteRenderer sprite;
    private float height;

    private void Start()
    {
        trn = this.transform;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Strech(topTrn.localPosition, endTrn.localPosition, true);
    }
    public void Strech(Vector3 _initialPosition, Vector3 _finalPosition, bool _mirrorZ)
    {
        Vector3 centerPos = (_initialPosition + _finalPosition) / 2f;
        trn.localPosition = centerPos;
  
        sprite.size = new Vector2(2.2f, Vector3.Distance(_initialPosition, _finalPosition) / trn.localScale.y);

       // trn.localScale = scale;
    }
}
