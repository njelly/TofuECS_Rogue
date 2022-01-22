using System;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class GameCamera : MonoBehaviour
    {
        public Transform Target;
        public Vector3 Offset;
        
        private Transform _t;

        private void Awake()
        {
            _t = transform;
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!Target)
                return;

            _t.position = Target.position + Offset;
        }
    }
}