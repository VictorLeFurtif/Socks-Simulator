using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Image winnerImage;
        [SerializeField] private Sprite player1WinSprite;
        [SerializeField] private Sprite player2WinSprite;
        [SerializeField] private float autoReturnDelay = 5f;
        
        private Canvas gameOverCanvas;
        
        private void Awake()
        {
            gameOverCanvas = GetComponent<Canvas>();
        }
        
        private void OnEnable()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }
    
        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    
        private void Start()
        {
            gameOverCanvas.enabled = false;
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.GameOver:
                    ShowGameOver();
                    break;
                case GameState.Game:
                    HideGameOver();
                    break;
            }
        }

        private void ShowGameOver()
        {
            PlayerScoreManager psm = PlayerScoreManager._instance;
            
            int winner = psm.GetLeftPlayerScore() > psm.GetRightPlayerScore() ? 1 : 2;
            winnerImage.sprite = winner == 1 ? player1WinSprite : player2WinSprite;

            var sayWinner = StartCoroutine(SoundManager.Instance.SayWinner(winner == 1
                ? SoundManager.Instance.SoundData.Player1
                : SoundManager.Instance.SoundData.Player2));


            gameOverCanvas.enabled = true;
            StartCoroutine(AutoReturnToMenu());
        }

        private void HideGameOver()
        {
            gameOverCanvas.enabled = false;
            StopAllCoroutines();
        }

        private IEnumerator AutoReturnToMenu()
        {
            yield return new WaitForSecondsRealtime(autoReturnDelay);
            GameManager.Instance?.ReturnToMenu();
        }
        
    }
}