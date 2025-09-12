using System;
using Data;
using UnityEngine;

namespace Manager
{
    public class DataHolderManager : MonoBehaviour
    {
        [SerializeField] private PlayerDataCommon scriptableObjectPlayerDataCommun;
        public PlayerDataCommonInstance playerDataCommon;

        private void Awake()
        {
            playerDataCommon = scriptableObjectPlayerDataCommun.Instance();
        }
    }
}