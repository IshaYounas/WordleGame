using System.Text.Json;

namespace MyWordleGame
{
    public partial class PlayerHistory : ContentPage
    {
        // constructor
        public PlayerHistory()
        {
            InitializeComponent();
            BindingContext = new PlayerHistoryViewModel(); // setting the binding context to call the view model page
        } // PlayerHistory
    } // class
} // namespace