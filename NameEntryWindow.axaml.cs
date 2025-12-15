using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace TicTacToeGUI
{
    public partial class NameEntryWindow : Window
    {
        public string Player1Name { get; private set; } = "Player X"; 
        public string Player2Name { get; private set; } = "Player O";

        public NameEntryWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object? sender, RoutedEventArgs e)
        {
            // Safely retrieve names, defaulting if empty
            Player1Name = string.IsNullOrWhiteSpace(P1NameInput.Text) ? "Player X" : P1NameInput.Text;
            Player2Name = string.IsNullOrWhiteSpace(P2NameInput.Text) ? "Player O" : P2NameInput.Text;
            
            // Close(true) sends 'true' back as the result of ShowDialog<bool?>
            Close(true); 
        }
    }
}