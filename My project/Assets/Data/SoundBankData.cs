using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "ScriptableObject/BankDataSound", fileName = "dataSound")]
    public class SoundBankData : ScriptableObject
    {
        public AudioClip EyeAttack { get; private set; }
        public AudioClip TongueAttack { get; private set; }
        public AudioClip AssCounter { get; private set; }
        public AudioClip Movement { get; private set; }
        public AudioClip Dead { get; private set; }
        public AudioClip NoseCounter { get; private set; }
        public AudioClip Player1 { get; private set; }
        public AudioClip Player2 { get; private set; }
        public AudioClip FlagGain { get; private set; }
        public AudioClip RoundStart { get; private set; }
        public AudioClip Round1 { get; private set; }
        public AudioClip Round2 { get; private set; }
        public AudioClip Round3 { get; private set; }
        public AudioClip Stun { get; private set; }
        public AudioClip Win { get; private set; }
    }
}
