using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class PendulumMovement : MonoBehaviour
    {
        [Header("Paramètres du Pendule")]
        [SerializeField] private float angleMax = 45f;
        [SerializeField] private float duree = 1f;
    
        private Tween penduleTween;
    
        void Start()
        {
            DemarrerPendule();
        }
    
        public void DemarrerPendule()
        {
            penduleTween?.Kill();
        
            penduleTween = transform.DORotate(new Vector3(0, 0, -angleMax), duree)
                .From(new Vector3(0, 0, angleMax))
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    
        public void ArreterPendule()
        {
            penduleTween?.Kill();
            transform.DORotate(Vector3.zero, 0.5f);
        }
    
        void OnDestroy()
        {
            penduleTween?.Kill();
        }
    }
}