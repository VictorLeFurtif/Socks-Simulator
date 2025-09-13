using Manager;
using UnityEngine;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Canvas gameOverCanvas;
    
        private void Start()
        {
            gameOverCanvas.gameObject.SetActive(false);
        }
    
        private void OnEnable()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }
    
        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    
        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.GameOver:
                    Debug.Log("Lost");
                    gameOverCanvas.gameObject.SetActive(true);
                    break;
                case GameState.Game:
                    gameOverCanvas.gameObject.SetActive(false);
                    break;
            }
        }

        public void ButtonMenu()
        {
            GameManager.Instance?.ReturnToMenu();
        }

        public void RestartGame()
        {
            GameManager.Instance?.RestartGame();
        }
    }
}