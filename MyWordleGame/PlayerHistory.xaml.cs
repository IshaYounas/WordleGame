namespace MyWordleGame
{
	public partial class PlayerHistory : ContentPage
	{
		// variables
		private const string file = "history.json";
		private List<Progress> history;

		// constructor
		public PlayerHistory()
		{
			InitializeComponent();
		} // PlayerHistory
	} // class
} // namespace