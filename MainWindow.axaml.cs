using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToeGUI
{
    public partial class MainWindow : Window
    {
        private bool xTurn = true; 
        private List<Button> cells;
        private int xScore = 0;
        private int oScore = 0;
        private bool isGameOver = false;

        // Static members to store names passed from App.axaml.cs
        public static string StaticPlayer1Name { get; set; } = "Player X";
        public static string StaticPlayer2Name { get; set; } = "Player O";

        // Player Name variables
        private readonly string player1Name;
        private readonly string player2Name;

        // Updated Icons (X and O)
        private const string X_ICON = "X"; 
        private const string O_ICON = "O"; 
        private const string PAW_ICON = "🐾"; 

        // Lavender Color Scheme
        private readonly SolidColorBrush XForeground = new SolidColorBrush(Color.Parse("#6A5ACD")); // Slate Blue (P1)
        private readonly SolidColorBrush OForeground = new SolidColorBrush(Color.Parse("#9370DB")); // Medium Purple (P2)
        private readonly SolidColorBrush StatusForeground = new SolidColorBrush(Color.Parse("#483D8B")); // Dark Slate Blue Status
        private readonly SolidColorBrush WinBackground = new SolidColorBrush(Color.Parse("#FFD700")); // Gold Win Highlight
        private readonly SolidColorBrush CellBackground = new SolidColorBrush(Color.Parse("#E6E6FA")); // Lavender

        // ONLY ONE CONSTRUCTOR: Parameterless, required by XAML tooling
        public MainWindow()
        {
            InitializeComponent();
            
            // Set final names from the static properties filled by App.axaml.cs
            player1Name = StaticPlayer1Name;
            player2Name = StaticPlayer2Name;

            cells = new List<Button>
            {
                b0!, b1!, b2!, b3!, b4!, b5!, b6!, b7!, b8! 
            };
            
            // Set the name display texts
            P1NameText.Text = $"{player1Name} (X) Score:";
            P2NameText.Text = $"{player2Name} (O) Score:";

            UpdateScoreText();
            StartNewRound();
        }

        private string GetCurrentPlayerName() => xTurn ? player1Name : player2Name;
        private string GetWinningPlayerName() => !xTurn ? player1Name : player2Name;

        private void Cell_Click(object? sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            
            if (btn == null || btn.Content != null || isGameOver)
                return;

            if (xTurn)
            {
                btn.Content = X_ICON; 
                btn.Foreground = XForeground;
            }
            else
            {
                btn.Content = O_ICON; 
                btn.Foreground = OForeground;
            }

            xTurn = !xTurn;
            CheckGameStatus();
        }
        
        private void CheckGameStatus()
        {
            if (CheckWinner())
            {
                isGameOver = true;
                string winnerName = GetWinningPlayerName(); 
                StatusText.Text = $"🥳 {winnerName} Wins! 🎉";
                
                if (!xTurn) xScore++; else oScore++;
                UpdateScoreText();
                DisableAllCells();
            }
            else if (CheckDraw())
            {
                isGameOver = true;
                StatusText.Text = $"{PAW_ICON} It's a Draw! {PAW_ICON}";
                DisableAllCells();
            }
            else
            {
                string nextPlayerName = GetCurrentPlayerName();
                StatusText.Text = $"Turn: {PAW_ICON} {nextPlayerName}";
                StatusText.Foreground = StatusForeground;
            }
        }

        private bool CheckWinner()
        {
            int[,] wins =
            {
                {0,1,2},{3,4,5},{6,7,8}, 
                {0,3,6},{1,4,7},{2,5,8}, 
                {0,4,8},{2,4,6}          
            };

            for (int i = 0; i < 8; i++)
            {
                string a = cells[wins[i,0]].Content?.ToString() ?? "";
                string b = cells[wins[i,1]].Content?.ToString() ?? "";
                string c = cells[wins[i,2]].Content?.ToString() ?? "";

                if (a != "" && a == b && b == c)
                {
                    cells[wins[i, 0]].Background = WinBackground;
                    cells[wins[i, 1]].Background = WinBackground;
                    cells[wins[i, 2]].Background = WinBackground;
                    return true;
                }
            }
            return false;
        }

        private bool CheckDraw()
        {
            return cells.All(c => c.Content != null);
        }

        private void Reset_Click(object? sender, RoutedEventArgs e)
        {
            StartNewRound();
        }

        private void ResetScore_Click(object? sender, RoutedEventArgs e)
        {
            xScore = 0;
            oScore = 0;
            UpdateScoreText();
            StartNewRound();
        }

        private void StartNewRound()
        {
            foreach (var c in cells)
            {
                c.Content = null;
                c.IsEnabled = true;
                c.Background = CellBackground; 
            }

            xTurn = true;
            isGameOver = false;
            StatusText.Text = $"Turn: {PAW_ICON} {player1Name}";
            StatusText.Foreground = StatusForeground;
        }

        private void DisableAllCells()
        {
            foreach (var c in cells)
            {
                c.IsEnabled = false;
            }
        }

        private void UpdateScoreText()
        {
            XScoreText.Text = xScore.ToString();
            OScoreText.Text = oScore.ToString();
        }
    }
}