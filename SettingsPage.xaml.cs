using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifteenPuzzle
{
    // Good video on MVVM: https://www.youtube.com/watch?v=VqZeTAjsgFQ
    public class SettingsViewModel
    {
        public Settings settings;

        public SettingsViewModel(Settings givenSettings)
        {
            this.settings = givenSettings;
        }

        public bool PlayFartFX
        {
            get { return this.settings.makeFartSounds; }
            set { this.settings.makeFartSounds = value; }
        }

        public bool HighlightTiles
        {
            get { return settings.highlightSolvedTiles; }
            set { this.settings.highlightSolvedTiles = value; }
        }

        public float AutoSolveSpeed
        {
            get { return settings.autoSolveSpeed; }
            set { this.settings.autoSolveSpeed = value; }
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(Settings settings)
        {
            InitializeComponent();

            this.BindingContext = new SettingsViewModel(settings);
        }

        async void OnBackButtonClicked(Object sender, EventArgs e)
        {
            await this.Navigation.PopModalAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if(this.BindingContext is SettingsViewModel viewModel)
                viewModel.settings.Save();
        }
    }
}
