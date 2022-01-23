using System;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        [HideInInspector] public Transform Target;
        public Vector3 Offset;
        public float OrthographicSize;

        private Camera _camera;
        private Transform _t;

        private void Awake()
        {
            _t = transform;
            _camera = GetComponent<Camera>();
            OrthographicSize = _camera.orthographicSize;
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            _camera.orthographicSize = OrthographicSize;
            
            if (Target)
                _t.position = Target.position + Offset;
        }

        private void OnValidate()
        {
            GetComponent<Camera>().orthographicSize = OrthographicSize;
            transform.position = Offset;
        }
    }
}