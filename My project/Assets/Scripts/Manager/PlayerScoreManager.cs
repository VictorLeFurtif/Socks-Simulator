using System;
using UnityEngine;
using Controller;
using Enum;
using UnityEngine.UI;
using System.Collections;

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

        [Header("Flags")]
        [SerializeField] private Image[] flagsP1;
        [SerializeField] private Image[] flagsP2;

        [Header("Round Image")]
        [SerializeField] private Image[] Rounds;
        [SerializeField] private Canvas canvasRound;

        [Header("Decor canvas")]
        [SerializeField] private GameObject[] decor;


        public static event Action<PlayerPlacement> OnPlayerScored;
        public static event Action<PlayerPlacement> OnPlayerWon;
        public static event Action OnRoundReset;

        private DataHolderManager leftData;
        private DataHolderManager rightData;
        private int currentRound = 0;

        public PlayerPlacement winPlayer;
        public bool win;

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
            StartCoroutine(ShowRound());

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
                flagsP1[leftPlayerScore].gameObject.SetActive(true);
                leftPlayerScore++;
            }
            else if (player == PlayerPlacement.Right)
            {
                flagsP2[rightPlayerScore].gameObject.SetActive(true);
                rightPlayerScore++;
            }
            
            OnPlayerScored?.Invoke(player);
            
            if (leftPlayerScore >= scoreToWin)
            {
                winPlayer = PlayerPlacement.Left;
                win = true;
            }
            else if (rightPlayerScore >= scoreToWin)
            {
                winPlayer = PlayerPlacement.Right;
                win = true;
            }
            else
            {
                ResetRound();
            }
        }

        public void PlayerWins(PlayerPlacement winner)
        {
            OnPlayerWon?.Invoke(winner);
            GameManager.Instance?.GameOver();
            Invoke(nameof(ResetGame), 3f);
        }
        
        private void ResetRound()
        {
            StartCoroutine(ShowRound());    
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
        
        private IEnumerator ShowRound()
        {
            PlayerController.blockMovement = true;

            canvasRound.enabled = true;
            Rounds[currentRound].gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(2);
            Rounds[currentRound].gameObject.SetActive(false);
            canvasRound.enabled = false;
            if(currentRound >= decor.Length)
                decor[decor.Length - 1].SetActive(true);
            else
                decor[currentRound].SetActive(true);

            if (currentRound > 0 && !(currentRound >= decor.Length))
                decor[currentRound - 1].SetActive(false);

            currentRound++;
            PlayerController.blockMovement = false;
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