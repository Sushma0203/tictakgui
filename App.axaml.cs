using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using System.Threading.Tasks;

namespace TicTacToeGUI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 1. Create a tiny, temporary Window to serve as the required owner.
                var temporaryOwner = new Window { Width = 1, Height = 1, IsVisible = false, ShowInTaskbar = false };
                desktop.MainWindow = temporaryOwner;

                // CRITICAL FIX: Explicitly call Show() to satisfy the framework's "visible owner" requirement.
                temporaryOwner.Show(); 

                // 2. Create the name entry dialog
                var nameDialog = new NameEntryWindow();
                
                // 3. Show the dialog, using the now-visible temporary owner.
                bool? dialogResult = await nameDialog.ShowDialog<bool?>(temporaryOwner); 

                // 4. Check result and set static names
                if (dialogResult == null || dialogResult == false)
                {
                    // Clean up and exit if the user cancels
                    temporaryOwner.Close();
                    desktop.Shutdown();
                    return;
                }

                // 5. Set the static properties to transfer names
                MainWindow.StaticPlayer1Name = nameDialog.Player1Name;
                MainWindow.StaticPlayer2Name = nameDialog.Player2Name;
                
                // 6. Initialize the final main game window
                var mainWindow = new MainWindow();
                
                // 7. Set the real main window and show it.
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
                
                // 8. Close the temporary owner.
                temporaryOwner.Close();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}