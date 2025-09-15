using System;
using UnityEngine;
using Controller;
using Enum;

namespace Manager
{
    public class PlayerScoreManager : MonoBehaviour
    {
        [Header("Score Settings")]
        [SerializeField] private int scoreToWin = 2;
        
        [Header("Players References")]
        [SerializeField] private PlayerController leftPlayer;
        [SerializeField] private PlayerController rightPlayer;
        
        [Header("Reset Positions")]
        [SerializeField] private Vector3 leftPlayerStartPos;
        [SerializeField] private Vector3 rightPlayerStartPos;
        
        [SerializeField] private int leftPlayerScore = 0;
        [SerializeField] private int rightPlayerScore = 0;
        
        public static event Action<PlayerPlacement> OnPlayerScored;
        public static event Action<PlayerPlacement> OnPlayerWon;
        public static event Action OnRoundReset;

        private DataHolderManager leftData;
        private DataHolderManager rightData;

        public static PlayerScoreManager _instance;
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            if (leftPlayer != null && leftPlayerStartPos == Vector3.zero)
            {
                leftPlayerStartPos = leftPlayer.transform.position;
            }
            
            if (rightPlayer != null && rightPlayerStartPos == Vector3.zero)
            {
                rightPlayerStartPos = rightPlayer.transform.position;
            }
            
            
            rightData = rightPlayer.GetComponent<DataHolderManager>();
            leftData = leftPlayer.GetComponent<DataHolderManager>();
        }
        
        public void AddScore(PlayerPlacement player)
        {
            if (player == PlayerPlacement.Left)
            {
                leftPlayerScore++;
            }
            else if (player == PlayerPlacement.Right)
            {
                rightPlayerScore++;
            }
            
            OnPlayerScored?.Invoke(player);
            
            if (leftPlayerScore >= scoreToWin)
            {
                PlayerWins(PlayerPlacement.Left);
            }
            else if (rightPlayerScore >= scoreToWin)
            {
                PlayerWins(PlayerPlacement.Right);
            }
            else
            {
                ResetRound();
            }
        }
        
        private void PlayerWins(PlayerPlacement winner)
        {
            OnPlayerWon?.Invoke(winner);
            GameManager.Instance?.GameOver();
            Invoke(nameof(ResetGame), 3f);
        }
        
        private void ResetRound()
        {
            ResetPlayerPositions();
            OnRoundReset?.Invoke();
        }
        
        private void ResetPlayerPositions()
        {
            if (leftPlayer != null)
            {
                leftPlayer.transform.position = leftPlayerStartPos;
                leftPlayer.rb.linearVelocity = Vector2.zero;
            }
            
            if (rightPlayer != null)
            {
                rightPlayer.transform.position = rightPlayerStartPos;
                rightPlayer.rb.linearVelocity = Vector2.zero;
            }
        }
        
        
        
        public void ResetGame()
        {
            leftPlayerScore = 0;
            rightPlayerScore = 0;
            
            ResetRound();
            
        }
        
        public int GetLeftPlayerScore() => leftPlayerScore;
        public int GetRightPlayerScore() => rightPlayerScore;
        public int GetScoreToWin() => scoreToWin;
    }
}