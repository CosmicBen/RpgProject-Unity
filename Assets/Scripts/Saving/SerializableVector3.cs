using System;
using UnityEngine;

namespace Rpg.Saving
{
    [Serializable]
    public class SerializableVector3
    {
        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;

        public SerializableVector3(Vector3 source)
        {
            x = source.x;
            y = source.y;
            z = source.z;
        }

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }
    }
}