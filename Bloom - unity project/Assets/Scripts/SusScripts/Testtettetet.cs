using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testtettetet : MonoBehaviour
{
    
    [SerializeField] RectTransform rekt;
    [SerializeField] Transform a;

    
    
    void Update()
    {
        

        Vector3 _screenPos = Camera.main.WorldToScreenPoint(a.position);

        //offscreen
        if(_screenPos.z < 0f || _screenPos.x < 0 || _screenPos.x > Screen.width || _screenPos.y < 0f || _screenPos.y > Screen.height)
        {
            if (_screenPos.z < 0f)
            {
                _screenPos *= -1f;
            }

            Vector3 _screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            _screenPos -= _screenCenter;

            float _angle = Mathf.Atan2(_screenPos.y, _screenPos.x);
            _angle -= 90 * Mathf.Deg2Rad;

            float _cos = Mathf.Cos(_angle);
            float _sin = -Mathf.Sin(_angle);

            //_screenPos = _screenCenter += new Vector3(_sin * 150, _cos * 150, 0f);

            float _m = _cos / _sin;

            Vector3 _screenBounds = _screenCenter * 0.95f;

            if(_cos > 0)
            {
                _screenPos = new Vector3(_screenBounds.y / _m, _screenBounds.y, 0f);
            }
            else
            {
                _screenPos = new Vector3(-_screenBounds.y / _m, -_screenBounds.y, 0f);
            }

            if(_screenPos.x > _screenBounds.x)
            {
                _screenPos = new Vector3(_screenBounds.x, _screenBounds.x * _m,0f);
            }
            else if(_screenPos.x < -_screenBounds.x)
            {
                _screenPos = new Vector3(-_screenBounds.x, -_screenBounds.x * _m, 0f);
            }

            _screenPos += _screenCenter;


            
        }
        transform.position = _screenPos;

    }
}
