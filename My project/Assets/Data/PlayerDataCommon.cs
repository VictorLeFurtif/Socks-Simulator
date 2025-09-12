using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "ScriptableObject/Player Data", fileName = "playerData")]
    public class PlayerDataCommon : ScriptableObject
    {
        [field:Header("ROPE DATA"),SerializeField] public RopeData RopeData { get; private set; }
        
        [field:Header("PLAYER CONTROLLER DATA"),SerializeField] public PlayerControllerData PlayerControllerData { get; private set; }

        [field: Header("ATTACK DATA"), SerializeField] public AttackManagerData AttackManagerData { get; private set; }

        public PlayerDataCommonInstance Instance()
        {
            return new PlayerDataCommonInstance(this);
        }
        
    }
    
    [Serializable]
    public class PlayerDataCommonInstance
    {
        public RopeData RopeData;
        public PlayerControllerData PlayerControllerData;
        public AttackManagerData AttackManagerData;
        public PlayerDataCommonInstance(PlayerDataCommon data)
        {
            RopeData = data.RopeData;
            PlayerControllerData = data.PlayerControllerData;
            AttackManagerData = data.AttackManagerData;
        }
    }
}