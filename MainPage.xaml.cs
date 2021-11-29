using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using Plugin.SimpleAudioPlayer;

namespace FifteenPuzzle
{
	public partial class MainPage : ContentPage
	{
		private int moveCount = 0;
		private Button[,] buttonMatrix = new Button[4, 4];
		private Random random = null;
		private Solver solver = null;
		private List<ISimpleAudioPlayer> playerList = new List<ISimpleAudioPlayer>();
		private Settings settings = new Settings();

		public int GetPuzzleValue(int row, int col)
		{
			Button button = this.buttonMatrix[row, col];
			string label = button.Text;
			int value = (label == "") ? 0 : int.Parse(label);
			return value;
		}

		public MainPage()
		{
			this.settings.Load();

			InitializeComponent();

			this.random = new Random(0); //DateTime.Now.Second * DateTime.Now.Day);

			this.buttonMatrix[0, 0] = this.Button00;
			this.buttonMatrix[0, 1] = this.Button01;
			this.buttonMatrix[0, 2] = this.Button02;
			this.buttonMatrix[0, 3] = this.Button03;

			this.buttonMatrix[1, 0] = this.Button10;
			this.buttonMatrix[1, 1] = this.Button11;
			this.buttonMatrix[1, 2] = this.Button12;
			this.buttonMatrix[1, 3] = this.Button13;

			this.buttonMatrix[2, 0] = this.Button20;
			this.buttonMatrix[2, 1] = this.Button21;
			this.buttonMatrix[2, 2] = this.Button22;
			this.buttonMatrix[2, 3] = this.Button23;

			this.buttonMatrix[3, 0] = this.Button30;
			this.buttonMatrix[3, 1] = this.Button31;
			this.buttonMatrix[3, 2] = this.Button32;
			this.buttonMatrix[3, 3] = this.Button33;

			Device.StartTimer(TimeSpan.FromSeconds(0.5), () => {
				if(this.solver != null)
					this.solver.Run(this);
				return true;
			});

			var assembly = Assembly.GetExecutingAssembly();
			foreach(string resourceName in assembly.GetManifestResourceNames())
			{
				if(resourceName.Contains("Fart"))
				{
					Stream stream = assembly.GetManifestResourceStream(resourceName);
					var player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
					if (player.Load(stream))
						this.playerList.Add(player);
				}
			}
		}

        private async void OnOptionsButtonClicked(object sender, EventArgs e)
		{
			SettingsPage settingsPage = new SettingsPage(this.settings);
			await this.Navigation.PushModalAsync(settingsPage);
		}

		private void OnScrambleButtonClicked(object sender, EventArgs e)
		{
			if(this.solver != null)
				return;

			int scrambleCount = 150;
			for(int i = 0; i < scrambleCount; i++)
			{
				int row = -1, col = -1;
				this.FindButtonLocation("", out row, out col);

				int rowDelta = 0, colDelta = 0;
				do
				{
					do
					{
						rowDelta = this.random.Next(-1, 2);
						colDelta = this.random.Next(-1, 2);
					}
					while(!(rowDelta == 0 && colDelta != 0) && !(rowDelta != 0 && colDelta == 0));
				}
				while(row + rowDelta < 0 || row + rowDelta >= 4 || col + colDelta < 0 || col + colDelta >= 4);

				Button button = this.buttonMatrix[row + rowDelta, col + colDelta];
				this.MakeMove(button);
			}

			this.moveCount = 0;
			this.UpdateLabels();
		}

		private bool FindButtonLocation(string label, out int row, out int col)
		{
			for (row = 0; row < 4; row++)
				for (col = 0; col < 4; col++)
					if (this.buttonMatrix[row, col].Text == label)
						return true;
			row = -1;
			col = -1;
			return false;
		}

		public void UpdateLabels()
		{
			this.moveCountLabel.Text = $"Move count: {this.moveCount}";

			int solveCount = 0;
			int number = 1;
			for(int i = 0; i < 4; i++)
				for(int j = 0; j < 4; j++)
					if(this.GetPuzzleValue(i, j) == number++ % 16)
						solveCount++;

			if(solveCount == 16)
				this.percentSolvedLabel.Text = "Solved!";
			else
			{
				int percent = (int)Math.Floor(((double)solveCount / 16.0) * 100.0);
				this.percentSolvedLabel.Text = $"{percent}% solved!";
			}
		}

		private bool MakeMove(int row, int col, int rowDelta, int colDelta)
		{
			Button buttonA = this.buttonMatrix[row, col];
			Button buttonB = this.buttonMatrix[row + rowDelta, col + colDelta];

			string label = buttonA.Text;
			buttonA.Text = buttonB.Text;
			buttonB.Text = label;

			return true;
		}

		public bool MakeMove(int row, int col)
		{
			if (row > 0 && this.buttonMatrix[row - 1, col].Text.Length == 0)
				return this.MakeMove(row, col, -1, 0);
			else if (row < 3 && this.buttonMatrix[row + 1, col].Text.Length == 0)
				return this.MakeMove(row, col, 1, 0);
			else if (col > 0 && this.buttonMatrix[row, col - 1].Text.Length == 0)
				return this.MakeMove(row, col, 0, -1);
			else if (col < 3 && this.buttonMatrix[row, col + 1].Text.Length == 0)
				return this.MakeMove(row, col, 0, 1);
			return false;
		}

		private bool MakeMove(Button button)
		{
			if (button == null)
				return false;

			int row = -1, col = -1;
			if (!this.FindButtonLocation(button.Text, out row, out col))
				return false;

			return this.MakeMove(row, col);
		}

		private void OnPuzzleButtonClicked(object sender, EventArgs e)
		{
			if(this.solver != null)
				return;

			if(this.MakeMove(sender as Button))
			{
				this.moveCount++;
				this.UpdateLabels();
				this.PlaySoundFXIfEnabled();
			}
		}

		public void PlaySoundFXIfEnabled()
		{
			if (this.settings.makeFartSounds)
			{
				int i = this.random.Next(0, this.playerList.Count);
				this.playerList[i].Play();
			}
		}

		private void OnResetButtonClicked(object sender, EventArgs e)
		{
			if(this.solver != null)
				return;
			
			int k = 1;
			for(int i = 0; i < 4; i++)
				for(int j = 0; j < 4; j++)
					this.buttonMatrix[i, j].Text = (k == 16) ? "" : $"{k++}";
			
			this.moveCount = 0;
			this.UpdateLabels();
		}

		private void OnSolverSwitchClicked(object sender, EventArgs e)
		{
			if(this.solverSwitch.IsToggled)
				this.solver = new Solver();
			else
				this.solver = null;

			this.scrambleButton.IsEnabled = (this.solver == null);
			this.resetButton.IsEnabled = (this.solver == null);
		}
	}
}
