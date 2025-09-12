using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct AttackManagerData
    {
        
        [Header("Attack CoolDown")]
        [Range(0.1f,5f), Tooltip("CoolDown before Attacking again")]public float attackCooldown;

        [Header("Value Loose on slider After Hit")]
        [Range(0.1f,5f)]public float looseSlider;

    }
}