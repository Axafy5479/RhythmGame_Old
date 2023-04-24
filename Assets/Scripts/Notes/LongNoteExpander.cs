using UnityEngine;
using System.Collections;

public class LongNoteExpander : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0f, 0f, 0f);
    public Vector3 endPosition = new Vector3(5f, 1f, 0f);
    public bool mirrorZ = true;
    void Start()
    {
        Strech(gameObject, startPosition, endPosition, mirrorZ);
    }


    public void Strech(GameObject _sprite, Vector3 _initialPosition, Vector3 _finalPosition, bool _mirrorZ)
    {
        Vector3 centerPos = (_initialPosition + _finalPosition) / 2f;
        _sprite.transform.position = centerPos;
        Vector3 direction = _finalPosition - _initialPosition;
        direction = Vector3.Normalize(direction);
        _sprite.transform.right = direction;
        if (_mirrorZ) _sprite.transform.right *= -1f;
        Vector3 scale = new Vector3(1, 1, 1);
        scale.x = Vector3.Distance(_initialPosition, _finalPosition);
        _sprite.transform.localScale = scale;
    }
}