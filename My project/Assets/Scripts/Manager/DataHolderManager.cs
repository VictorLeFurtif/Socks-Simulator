using System;
using Data;
using UnityEngine;

namespace Manager
{
    public class DataHolderManager : MonoBehaviour
    {
        [SerializeField] private PlayerDataCommon scriptableObjectPlayerDataCommun;
        public PlayerDataCommonInstance playerDataCommon;
        [HideInInspector] public PlayerDataCommon ScriptableObjectPlayerDataCommun { get; private set; }
        private void Awake()
        {
            playerDataCommon = scriptableObjectPlayerDataCommun.Instance();
        }
    }
}