using System;
using Enum;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct RopeData
    {
        [Header("Grab Settings")]
        [Range(0.1f,5f), Tooltip("Minimum distance required to grab enemy")] public float minDistanceToGrab;
        
        [Header("Stun System")]
        [SerializeField, Range(1f, 10f), Tooltip("Maximum stun duration when grabbed")]
        public float maxStunValue;
        
        [Range(0.1f, 2f), Tooltip("Stun value reduced per button press")]
        public float stunValueToTakeOut;

        [Range(0.1f, 10f), Tooltip("Distance from ko player and the flag")]
        public float flagDistance;
        
        [Header("State Machine")]
        public KoState currentKoState;
        public PlayerState currentPlayerState;

        [Header("Win Condition")]
        [SerializeField, Range(0.1f, 2f), Tooltip("Distance threshold to flag for winning")]
        public float epsilon;

        [Header("Runtime State")]
        public bool dragging;


        public float tensionStrength;
        
        public RopeData(
            float minDistanceToGrab = 1f,
            float maxStunValue = 5f,
            float stunValueToTakeOut = 0.5f,
            float flagDistance = 2f,
            KoState currentKoState = KoState.NotKo,
            PlayerState currentPlayerState = PlayerState.Idle,
            float epsilon = 0.5f,
            bool dragging = false,
            float tensionStrength = 0f)
        {
            this.minDistanceToGrab = minDistanceToGrab;
            this.maxStunValue = maxStunValue;
            this.stunValueToTakeOut = stunValueToTakeOut;
            this.flagDistance = flagDistance;
            this.currentKoState = currentKoState;
            this.currentPlayerState = currentPlayerState;
            this.epsilon = epsilon;
            this.dragging = dragging;
            this.tensionStrength = tensionStrength;
        }

    }
}