using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "ScriptableObject/BankDataSound", fileName = "dataSound")]
    public class SoundBankData : ScriptableObject
    {
        [Header("Attack Sounds")]
        [SerializeField] private AudioClip eyeAttack;
        [SerializeField] private AudioClip tongueAttack;
        [SerializeField] private AudioClip assCounter;
        [SerializeField] private AudioClip noseCounter;
        
        [Header("Movement Sounds")]
        [SerializeField] private AudioClip movement;
        [SerializeField] private AudioClip movement2;
        
        [Header("Player Sounds")]
        [SerializeField] private AudioClip player1;
        [SerializeField] private AudioClip player2;
        [SerializeField] private AudioClip dead;
        [SerializeField] private AudioClip stun;
        
        [Header("Game Sounds")]
        [SerializeField] private AudioClip flagGain;
        [SerializeField] private AudioClip roundStart;
        [SerializeField] private AudioClip win;
        [SerializeField] private AudioClip mainSound;

        [Header("Round")] [SerializeField] private AudioClip[] tabRound = new AudioClip[3];

        public AudioClip EyeAttack => eyeAttack;
        public AudioClip TongueAttack => tongueAttack;
        public AudioClip AssCounter => assCounter;
        public AudioClip Movement => movement;
        public AudioClip Movement2 => movement2;
        public AudioClip Dead => dead;
        public AudioClip NoseCounter => noseCounter;
        public AudioClip Player1 => player1;
        public AudioClip Player2 => player2;
        public AudioClip FlagGain => flagGain;
        public AudioClip RoundStart => roundStart;
        public AudioClip[] Round => tabRound;
        public AudioClip Stun => stun;
        public AudioClip Win => win;
        public AudioClip MainSound => mainSound;
    }
}