using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoCamera : MonoBehaviour
    {
        public float senstivity = 1.0f;
        Vector2 downPos;
        Vector3 downEuler;
        bool pointerDown = false;

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                downPos = Input.mousePosition;
                downEuler = transform.rotation.eulerAngles;
                pointerDown = true;
            }

            if (pointerDown)
            {
                Vector2 pos = Input.mousePosition;
                Vector2 v = (pos - downPos) * senstivity;
                Vector3 angle = downEuler + new Vector3(-v.y, v.x);
                angle.x = Mathf.Clamp(angle.x, 0.0f, 60.0f);
                angle.y = Mathf.Repeat(angle.y, 360.0f);
                transform.rotation = Quaternion.Euler(angle);
            }

            if (Input.GetMouseButtonUp(0))
            {
                pointerDown = false;
            }
        }
    }
}