using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;
using TMPro;

public class TicTacToe : MonoBehaviour
{

    public Button[] cells; // Các ô trên bàn cờ (Button)
    public TextMeshProUGUI statusText; // Text hiển thị trạng thái (thắng, thua, hòa)
    private int[] board = new int[9]; // Bàn cờ: 0 = trống, 1 = người chơi, -1 = AI
    private bool isPlayerTurn = true; // Người chơi đi trước

    void Start()
    {
        ResetBoard();
    }

    public void OnCellClicked(int index)
    {
        if (board[index] != 0 || !isPlayerTurn) return;

        board[index] = 1; // Người chơi = 1
        cells[index].GetComponentInChildren<TextMeshProUGUI>().text = "X";
        cells[index].interactable = false;

        if (CheckWin(1))
        {
            EndGame("You Win!");
            return;
        }
        else if (IsBoardFull())
        {
            EndGame("Draw!");
            return;
        }

        isPlayerTurn = false;
        Invoke(nameof(AIMove), 0.5f); // Chờ 0.5 giây để AI di chuyển
    }

    void AIMove()
    {
        int bestMove = GetBestMove();
        board[bestMove] = -1; // AI = -1
        cells[bestMove].GetComponentInChildren<TextMeshProUGUI>().text = "O";
        cells[bestMove].interactable = false;

        if (CheckWin(-1))
        {
            EndGame("AI Wins!");
            return;
        }
        else if (IsBoardFull())
        {
            EndGame("Draw!");
            return;
        }

        isPlayerTurn = true;
    }

    int GetBestMove()
    {
        int bestScore = int.MinValue;
        int move = -1;

        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == 0)
            {
                board[i] = -1; // AI giả lập bước đi
                int score = Minimax(board, 0, false);
                board[i] = 0; // Hoàn tác

                if (score > bestScore)
                {
                    bestScore = score;
                    move = i;
                }
            }
        }
        return move;
    }

    int Minimax(int[] board, int depth, bool isMaximizing)
    {
        if (CheckWin(-1)) return 10 - depth; // AI thắng
        if (CheckWin(1)) return depth - 10; // Người chơi thắng
        if (IsBoardFull()) return 0; // Hòa

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == 0)
                {
                    board[i] = -1; // AI giả lập bước đi
                    int score = Minimax(board, depth + 1, false);
                    board[i] = 0; // Hoàn tác
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == 0)
                {
                    board[i] = 1; // Người chơi giả lập bước đi
                    int score = Minimax(board, depth + 1, true);
                    board[i] = 0; // Hoàn tác
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    bool CheckWin(int player)
    {
        int[,] winConditions = {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, // Hàng ngang
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, // Hàng dọc
            { 0, 4, 8 }, { 2, 4, 6 }              // Đường chéo
        };

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            if (board[winConditions[i, 0]] == player &&
                board[winConditions[i, 1]] == player &&
                board[winConditions[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    bool IsBoardFull()
    {
        foreach (int cell in board)
        {
            if (cell == 0) return false;
        }
        return true;
    }

    void EndGame(string result)
    {
        statusText.text = result;
        foreach (Button cell in cells)
        {
            cell.interactable = false;
        }
    }

    public void ResetBoard()
    {
        board = new int[9];
        foreach (Button cell in cells)
        {
            cell.GetComponentInChildren<TextMeshProUGUI>().text = "";
            cell.interactable = true;
        }
        statusText.text = "Your Turn!";
        isPlayerTurn = true;
    }
}
