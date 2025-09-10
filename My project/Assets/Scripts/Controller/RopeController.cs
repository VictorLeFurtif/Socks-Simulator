using Enum;
using Interface;
using UnityEngine;

namespace Controller
{
    public class RopeController : MonoBehaviour, IDraggable
    {
        #region Fields
        
        [Header("Value")]
        
        [SerializeField] private float temporaryLife;

        public float TemporaryLife
        {
            get => temporaryLife;
            private set
            {
                temporaryLife = value;
                
            }
        }

        [SerializeField] private float maxStunValue;
        
        private int stunValue;

        public int StunValue
        {
            get => stunValue;
            private set
            {
                stunValue = value;
                if (stunValue <= 0)
                {
                    CurrentKoState = KoState.NotKo;
                }
            }
        }

        [SerializeField] private int stunValueToTakeOut;

        [Header("Key")]

         private KeyCode releaseKey;
         
         [Header("State Machine")]

         private KoState currentKoState;

         public KoState CurrentKoState
         {
             get => currentKoState;
             private set
             {
                 //Here we need to make sure that The input are or blocked or release 
                 currentKoState = value;
                 
             }
         }

         #endregion

        #region RopeController Methods For Attacker
        
        //need to assign the maximum value at the begging or this will be called in Update to divide the logic
        //TODO : no idea for the moment
        public void DragRope()
        {
            throw new System.NotImplementedException();
        }
        
        #endregion

        #region RopeController Methods For Defenseur
        
        // Here you spam it to release yourself when you re KO
        //Please god have mercy sooo we need to spam to get the stunValue to 0
        // Then were good because we will make a Get Setter when we Update this value
        //A void will be call to change the state condition in setter if stun value
        // <= 0 so not KO can play and the Enemy release rope easier if also if enemy is 
        // a variable
        public void TryReleaseRope()
        {
            if (Input.GetKeyDown(releaseKey) && currentKoState == KoState.Ko && TemporaryLife > 0)
            {
                StunValue -= stunValueToTakeOut;
            }
        }
        
        

        #endregion
        
        
    }
}
