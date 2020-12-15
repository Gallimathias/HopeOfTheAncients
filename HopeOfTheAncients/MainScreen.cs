using engenious;
using engenious.UI;
using engenious.UI.Controls;

namespace HopeOfTheAncients
{
    public class MainScreen : Screen
    {
        public MainScreen(BaseScreenComponent manager) : base(manager)
        {
            Background = new BorderBrush(Color.DarkRed);
            var panel = new StackPanel(manager);
            Controls.Add(panel);

            panel.Controls.Add(new TextButton(manager, "Hello World!"));

        }
    }
}