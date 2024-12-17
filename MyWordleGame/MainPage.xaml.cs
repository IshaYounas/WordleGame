﻿using Plugin.Maui.Audio;
namespace MyWordleGame
{
    public partial class MainPage : ContentPage
    {
        // fields
        private string currentGuess = "";
        private const int maxRows = 6;
        private const int maxCols = 5;
        private int currentRow = 0;

        // constructor
        public MainPage()
        {
            InitializeComponent();

            // calling custom methods
            CreateGameGrid();
        } // MainPage

        // methods for buttons
        private void Key_Clicked(object sender, EventArgs e)
        {

        } // Key_Clicked

        private void Enter_Clicked(object sender, EventArgs e)
        {

        } // Enter_Clicked

        private void Del_Clicked(object sender, EventArgs e)
        {

        } // Del_Clicked

        private void CreateGameGrid()
        {
            for (int row = 0; row < 6; row++) // 6 guesses
            {
                for (int col = 0; col < 5; col++) // 5 letter words
                {
                    var label = new Label
                    {
                        BackgroundColor = Colors.AntiqueWhite,
                        FontSize = 20,
                        TextColor = Colors.DarkBlue,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        HeightRequest = 50,
                        WidthRequest = 50
                    }; // var label

                    // adding label to the grid position
                    Grid.SetRow(label, row);
                    Grid.SetColumn(label, col);
                    GameGrid.Children.Add(label);
                } // for (col)
            } // for (row)
        } // CreateGameGrid
    } // class
} // namespace
