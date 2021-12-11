using engenious;
using engenious.UI;
using engenious.UI.Controls;
using System;

namespace HopeOfTheAncients
{
    public class MainScreen : Screen, IDisposable
    {
        private readonly GameScreen gameScreen;
        private readonly StargateScreen stargateScreen;
        public MainScreen(BaseScreenComponent manager) : base(manager)
        {
            gameScreen = new GameScreen(manager);
            stargateScreen = new StargateScreen(manager);
            Background = new BorderBrush(Color.Red);
            StackPanel panel = new(manager);
            Controls.Add(panel);

            TextButton button = new(manager, "Start Game");
            button.LeftMouseClick += (s, e) => manager.NavigateToScreen(gameScreen);
            TextButton stargateButton = new(manager, "To the StarGate");
            stargateButton.LeftMouseClick += (s, e) => manager.NavigateToScreen(stargateScreen);
            panel.Controls.Add(button);
            panel.Controls.Add(stargateButton);
        }

        public void Dispose()
        {
            gameScreen.Dispose();
        }
    }
}