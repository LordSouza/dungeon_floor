using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScollBackgroundMenu : MonoBehaviour
{
    [SerializeField] RawImage _img;
    [SerializeField] float _x;
    [SerializeField] float _y;

    void Update()
    {
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _img.uvRect.size);
    }
}
