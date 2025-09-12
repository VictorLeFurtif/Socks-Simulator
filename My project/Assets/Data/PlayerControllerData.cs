using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct PlayerControllerData
    {
        [Header("Move Speed")]
        [Range(0.1f,5f), Tooltip("Move speed of the entity")] public float speed;
        
        [Header("Push Distance")]
        [Range(0.1f,5f), Tooltip("Distance when hit ")] public float distancePush;
        
        [Header("Push Height")]
        [Range(0.1f,5f), Tooltip("Distance when hit in Y ")] public float heightFactor;
        
    }
}