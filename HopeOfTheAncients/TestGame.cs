using System;
using engenious;

namespace HopeOfTheAncients
{
    public class HopeOfTheAncientsGame : Game
    {
        protected override void Initialize()
        {
            var screenComponent = new ScreenComponent(this);

            Components.Add(screenComponent);
            base.Initialize();
        }
    }
}
