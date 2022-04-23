using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Core
{
    public class CameraFacing : MonoBehaviour
    {
        private Camera mainCamera = null;

        private Camera MainCamera
        {
            get
            {
                if (mainCamera == null) { mainCamera = Camera.main; }
                return mainCamera;
            }
        }

        private void LateUpdate()
        {
            transform.forward = MainCamera.transform.forward;
        }
    }
}