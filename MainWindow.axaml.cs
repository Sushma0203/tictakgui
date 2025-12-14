using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToeGUI
{
    public partial class MainWindow : Window
    {
        private bool playerTurn = true;
        private bool roundOver = false;

        private int xWins = 0;
        private int oWins = 0;
        private int draws = 0;
        private int[]? winLine;

        private List<Button> buttons;
        private DispatcherTimer pulseTimer;
        private int pulseIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            buttons = new List<Button>
            {
                btn0, btn1, btn2,
                btn3, btn4, btn5,
                btn6, btn7, btn8
            };

            pulseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            pulseTimer.Tick += Pulse;

            DifficultySlider.PropertyChanged += (_, __) =>
            {
                DifficultyLabel.Text = DifficultySlider.Value switch
                {
                    1 => "Easy",
                    2 => "Medium",
                    3 => "Hard",
                    _ => "Easy"
                };
            };
        }

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            if (!playerTurn || roundOver) return;
            var btn = (Button)sender!;
            if (btn.Content != null) return;

            PlaceSymbol(btn, "X");
            if (CheckEnd("X")) return;

            playerTurn = false;
            AITurn();
        }

        private void AITurn()
        {
            int difficulty = (int)DifficultySlider.Value;
            int move = -1;

            if (difficulty == 1)
            {
                var empty = buttons.Select((b, i) => (b, i)).Where(x => x.b.Content == null).Select(x => x.i).ToList();
                Random rnd = new();
                move = empty[rnd.Next(empty.Count)];
            }
            else
            {
                move = FindWinningMove("O") ?? FindWinningMove("X") ?? buttons.FindIndex(b => b.Content == null);
            }

            PlaceSymbol(buttons[move], "O");
            CheckEnd("O");
            playerTurn = true;
        }

        private int? FindWinningMove(string s)
        {
            int[,] w =
            {
                {0,1,2},{3,4,5},{6,7,8},
                {0,3,6},{1,4,7},{2,5,8},
                {0,4,8},{2,4,6}
            };
            for (int i = 0; i < 8; i++)
            {
                var line = new[] { w[i,0], w[i,1], w[i,2] };
                var marks = line.Select(i => buttons[i].Content?.ToString()).ToList();
                if (marks.Count(m => m == s) == 2 && marks.Count(m => m == null) == 1)
                    return line[marks.IndexOf(null)];
            }
            return null;
        }

        private void PlaceSymbol(Button btn, string s)
        {
            btn.Content = s;
            btn.Foreground = new SolidColorBrush(Color.Parse("#4B0082")); // dark purple for X/O
            btn.Background = new SolidColorBrush(Color.Parse("#E6E6FA")); // light lavender
            btn.BorderBrush = new SolidColorBrush(Color.Parse("#B299E6")); // darker lavender border
            btn.IsEnabled = false;
        }


        private bool CheckEnd(string s)
        {
            int[,] w =
            {
                {0,1,2},{3,4,5},{6,7,8},
                {0,3,6},{1,4,7},{2,5,8},
                {0,4,8},{2,4,6}
            };

            for (int i = 0; i < 8; i++)
            {
                if (buttons[w[i,0]].Content?.ToString() == s &&
                    buttons[w[i,1]].Content?.ToString() == s &&
                    buttons[w[i,2]].Content?.ToString() == s)
                {
                    winLine = new[] { w[i,0], w[i,1], w[i,2] };
                    StartPulse();

                    roundOver = true;
                    if (s == "X") xWins++; else oWins++;
                    UpdateScore();
                    StatusText.Text = s == "X" ? "You won the round 🎉" : "AI won the round 🤖";

                    HandleMatchEnd();
                    return true;
                }
            }

            if (buttons.All(b => b.Content != null))
            {
                roundOver = true;
                draws++;
                StatusText.Text = "Draw 🤝";
                UpdateScore();
                HandleMatchEnd();
                return true;
            }

            return false;
        }

        private void StartPulse()
        {
            pulseIndex = 0;
            pulseTimer.Start();
        }

        private void Pulse(object? sender, EventArgs e)
        {
            if (winLine == null) return;
            var color = pulseIndex % 2 == 0 ? "#B299E6" : "#F2E6FF";
            foreach (var i in winLine)
                buttons[i].Background = new SolidColorBrush(Color.Parse(color));

            pulseIndex++;
            if (pulseIndex > 6) pulseTimer.Stop();
        }

        private void UpdateScore()
        {
            XScoreText.Text = $"X: {xWins}";
            OScoreText.Text = $"O: {oWins}";
            DrawScoreText.Text = $"Draws: {draws}";
        }

        private void ResetRound_Click(object? sender, RoutedEventArgs e) => ResetRound();

        private void ResetRound()
        {
            foreach (var b in buttons)
            {
                b.Content = null;
                b.IsEnabled = true;
                b.Background = new SolidColorBrush(Color.Parse("#F2E6FF"));
                b.BorderBrush = new SolidColorBrush(Color.Parse("#B299E6"));
            }
            roundOver = false;
            playerTurn = true;
            StatusText.Text = "Turn: X";
            winLine = null;

            ResetRoundButton.IsEnabled = false;
        }

        private void NewMatch_Click(object? sender, RoutedEventArgs e)
        {
            xWins = oWins = draws = 0;
            UpdateScore();
            ResetRound();
        }

        private void HandleMatchEnd()
        {
            if (xWins + oWins + draws >= 3)
            {
                StatusText.Text += "\nBest-of-3 finished!";
                ResetRoundButton.IsEnabled = false;
                NewMatchButton.IsEnabled = true;
            }
            else
            {
                ResetRoundButton.IsEnabled = true;
                NewMatchButton.IsEnabled = false;
            }
        }
    }
}
