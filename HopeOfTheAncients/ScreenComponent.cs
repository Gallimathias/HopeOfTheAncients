using engenious;
using engenious.UI;

namespace HopeOfTheAncients;

public class ScreenComponent : BaseScreenComponent
{
    private MainScreen? mainScreen;

    public ScreenComponent(Game game) : base(game)
    {

    }

    protected override void LoadContent()
    {
        base.LoadContent();
        mainScreen = new MainScreen(this);
        NavigateToScreen(mainScreen);
    }

    protected override void UnloadContent()
    {
        mainScreen?.Dispose();
        base.UnloadContent();
    }

    public override void Dispose()
    {
        mainScreen?.Dispose();
        base.Dispose();
    }
}