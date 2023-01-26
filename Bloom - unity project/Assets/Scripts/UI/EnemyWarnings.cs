using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWarnings : MonoBehaviour
{
    public static EnemyWarnings current;

    [SerializeField] Transform canvasTransform;
    [SerializeField] GameObject warningPrefab;

    Camera screenCamerea;

    List<Transform> enemies = new List<Transform>();
    List<Transform> warnings = new List<Transform>();

    private void Awake()
    {
        screenCamerea = Camera.main;//
        current = this;
    }

    public void AddEnemy(Transform _enemy)
    {
        enemies.Add(_enemy);
        Transform _warning = Instantiate(warningPrefab, Vector3.zero, Quaternion.identity).transform;

        _warning.SetParent(canvasTransform);
        _warning.localScale = Vector3.one;

        warnings.Add(_warning);
    }

    public void RemoveEnemy(Transform _enemy)
    {
        int _index = enemies.IndexOf(_enemy);

        enemies.RemoveAt(_index);
        Transform _warning = warnings[_index];
        warnings.RemoveAt(_index);
        Destroy(_warning.gameObject);
    }

    private void Update()
    {
        Vector3 _screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        for (int i = 0; i < warnings.Count; i++)
        {
            Vector3 _screenPos = Camera.main.WorldToScreenPoint(enemies[i].position);

            //offscreen
            if (_screenPos.z < 0f || _screenPos.x < 0 || _screenPos.x > Screen.width || _screenPos.y < 0f || _screenPos.y > Screen.height)
            {
                warnings[i].gameObject.SetActive(true);

                if (_screenPos.z < 0f)
                {
                    _screenPos *= -1f;
                }

                _screenPos -= _screenCenter;

                float _angle = Mathf.Atan2(_screenPos.y, _screenPos.x);
                _angle -= 90 * Mathf.Deg2Rad;

                float _cos = Mathf.Cos(_angle);
                float _sin = -Mathf.Sin(_angle);

                float _m = _cos / _sin;

                Vector3 _screenBounds = _screenCenter * 0.95f;

                if (_cos > 0)
                {
                    _screenPos = new Vector3(_screenBounds.y / _m, _screenBounds.y, 0f);
                }
                else
                {
                    _screenPos = new Vector3(-_screenBounds.y / _m, -_screenBounds.y, 0f);
                }

                if (_screenPos.x > _screenBounds.x)
                {
                    _screenPos = new Vector3(_screenBounds.x, _screenBounds.x * _m, 0f);
                }
                else if (_screenPos.x < -_screenBounds.x)
                {
                    _screenPos = new Vector3(-_screenBounds.x, -_screenBounds.x * _m, 0f);
                }

                _screenPos += _screenCenter;


                warnings[i].position = _screenPos;
            }
            else
            {
                warnings[i].gameObject.SetActive(false);
            }
        }
    }
}
