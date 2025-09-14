using Manager;
using UnityEngine;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Canvas gameOverCanvas;
        
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
            gameOverCanvas.gameObject.SetActive(true);
            gameOverCanvas.enabled = false; 
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.GameOver:
                    Debug.Log("Lost - GameOverUI activated");
                    gameOverCanvas.enabled = true;
                    break;
                case GameState.Game:
                    gameOverCanvas.enabled = false;
                    break;
            }
        }

        public void ButtonMenu()
        {
            GameManager.Instance?.ReturnToMenu();
        }

        public void RestartGame()
        {
            GameManager.Instance?.LoadGameScene();
        }
    }
}