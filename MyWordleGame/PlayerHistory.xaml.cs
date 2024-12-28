namespace MyWordleGame
{
	public partial class PlayerHistory : ContentPage
	{
		// variables
		private const string file = "history_file.json";
		private List<Progress> history;

		// constructor
		public PlayerHistory()
		{
			InitializeComponent();
        } // PlayerHistory
    } // class
} // namespace