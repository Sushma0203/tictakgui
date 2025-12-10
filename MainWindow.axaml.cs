using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TicTacToeGUI
{
    public partial class MainWindow : Window
    {
        private string currentPlayer = "X";
        private string[] board = new string[9];
        private Button[] buttons;
        private int XScore = 0, OScore = 0, DrawScore = 0;
        private Stack<int> moveHistory = new Stack<int>();

        public MainWindow()
        {
            InitializeComponent();

            buttons = new Button[]
            {
                this.FindControl<Button>("btn0"),
                this.FindControl<Button>("btn1"),
                this.FindControl<Button>("btn2"),
                this.FindControl<Button>("btn3"),
                this.FindControl<Button>("btn4"),
                this.FindControl<Button>("btn5"),
                this.FindControl<Button>("btn6"),
                this.FindControl<Button>("btn7"),
                this.FindControl<Button>("btn8")
            };

            ResetBoard();
        }

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            int index = Array.IndexOf(buttons, btn);
            if (board[index] == "")
            {
                board[index] = currentPlayer;
                btn.Content = currentPlayer;
                btn.Foreground = currentPlayer == "X" ? Brushes.Red : Brushes.Blue;
                moveHistory.Push(index);

                int[] winningLine = CheckWinnerLine();
                if (winningLine != null)
                {
                    HighlightWinner(winningLine);
                    StatusText.Text = $"Player {currentPlayer} Wins!";
                    if (currentPlayer == "X") XScore++; else OScore++;
                    UpdateScore();
                    DisableButtons();
                    return;
                }
                else if (board.All(b => b != ""))
                {
                    DrawScore++;
                    UpdateScore();
                    StatusText.Text = "It's a Draw!";
                    return;
                }

                currentPlayer = currentPlayer == "X" ? "O" : "X";
                StatusText.Text = $"Turn: {currentPlayer}";
            }
        }

        private int[] CheckWinnerLine()
        {
            int[,] winPatterns = new int[,]
            {
                {0,1,2},{3,4,5},{6,7,8},
                {0,3,6},{1,4,7},{2,5,8},
                {0,4,8},{2,4,6}
            };

            for (int i = 0; i < winPatterns.GetLength(0); i++)
            {
                int a = winPatterns[i, 0];
                int b = winPatterns[i, 1];
                int c = winPatterns[i, 2];

                if (board[a] != "" && board[a] == board[b] && board[b] == board[c])
                    return new int[] { a, b, c };
            }
            return null;
        }

        private void HighlightWinner(int[] line)
        {
            foreach (var i in line)
                buttons[i].Background = Brushes.LightGreen;
        }

        private void UpdateScore()
        {
            XScoreText.Text = $"X: {XScore}";
            OScoreText.Text = $"O: {OScore}";
            DrawScoreText.Text = $"Draws: {DrawScore}";
        }

        private void DisableButtons()
        {
            foreach (var btn in buttons)
                btn.IsEnabled = false;
        }

        private void ResetBoard()
        {
            for (int i = 0; i < board.Length; i++)
                board[i] = "";

            foreach (var btn in buttons)
            {
                btn.Content = "";
                btn.Background = Brushes.White;
                btn.IsEnabled = true;
            }

            moveHistory.Clear();
            currentPlayer = "X";
            StatusText.Text = $"Turn: {currentPlayer}";
        }

        private void ResetButton_Click(object? sender, RoutedEventArgs e)
        {
            ResetBoard();
        }

        private void UndoButton_Click(object? sender, RoutedEventArgs e)
        {
            if (moveHistory.Count > 0)
            {
                int last = moveHistory.Pop();
                board[last] = "";
                buttons[last].Content = "";
                buttons[last].Background = Brushes.White;
                buttons[last].IsEnabled = true;

                currentPlayer = currentPlayer == "X" ? "O" : "X";
                StatusText.Text = $"Turn: {currentPlayer}";
            }
        }
    }
}
