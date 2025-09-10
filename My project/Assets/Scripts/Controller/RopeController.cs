using Enum;
using Interface;
using UnityEngine;

namespace Controller
{
    public class RopeController : MonoBehaviour, IDraggable
    {
        #region Fields

        [SerializeField] private float temporaryLife;

        [SerializeField] private float stunValue;

        [SerializeField] private KeyCode releaseKey;

        [SerializeField] private KoState currentKoState;

        #endregion

        #region RopeController Methods For Attacker
        
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
            if (Input.GetKeyDown(releaseKey) && currentKoState == KoState.Ko)
            {
                
            }
        }

        #endregion
        
        
    }
}
