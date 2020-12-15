using engenious;
using engenious.UI;
using engenious.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients
{
    class GameScreen : Screen, IDisposable
    {
        private readonly GameControl gameControl;

        public GameScreen(BaseScreenComponent manager) : base(manager)
        {
            Background = new SolidColorBrush(Color.DarkRed);
            gameControl = new GameControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            Controls.Add(gameControl);
        }

        public void Dispose()
        {
            gameControl.Dispose();
        }
    }
}
