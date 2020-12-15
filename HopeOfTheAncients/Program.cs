using System;

namespace HopeOfTheAncients
{
    public static class MainClass
	{
        [STAThread()]
		public static void Main (string[] args)
		{
			using(var game = new HopeOfTheAncientsGame ())
				game.Run();
		}
	}
}
