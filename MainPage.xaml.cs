using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

namespace FifteenPuzzle
{
	public partial class MainPage : ContentPage
	{
		private int moveCount = 0;
		private Button[,] buttonMatrix = new Button[4, 4];
		private Random random = null;

		public MainPage()
		{
			InitializeComponent();

			this.random = new Random(DateTime.Now.Second * DateTime.Now.Day);

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
		}

		private void UpdateMoveCount(int newMoveCount)
		{
			this.moveCount = newMoveCount;
			this.moveCountLabel.Text = $"Move count: {this.moveCount}";
		}

		private async void OnOptionsButtonClicked(object sender, EventArgs e)
		{
			SettingsPage settingsPage = new SettingsPage();
			var task = this.Navigation.PushModalAsync(settingsPage);
			await task;
		}

		private void OnScrambleButtonClicked(object sender, EventArgs e)
		{
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

			this.UpdateMoveCount(0);
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

		private void MakeMove(int row, int col, int rowDelta, int colDelta)
		{
			Button buttonA = this.buttonMatrix[row, col];
			Button buttonB = this.buttonMatrix[row + rowDelta, col + colDelta];

			string label = buttonA.Text;
			buttonA.Text = buttonB.Text;
			buttonB.Text = label;

			this.UpdateMoveCount(this.moveCount + 1);
		}

		private void MakeMove(Button button)
		{
			if (button == null)
				return;

			int row = -1, col = -1;
			if (!this.FindButtonLocation(button.Text, out row, out col))
				return;

			if (row > 0 && this.buttonMatrix[row - 1, col].Text.Length == 0)
				this.MakeMove(row, col, -1, 0);
			else if (row < 3 && this.buttonMatrix[row + 1, col].Text.Length == 0)
				this.MakeMove(row, col, 1, 0);
			else if (col > 0 && this.buttonMatrix[row, col - 1].Text.Length == 0)
				this.MakeMove(row, col, 0, -1);
			else if (col < 3 && this.buttonMatrix[row, col + 1].Text.Length == 0)
				this.MakeMove(row, col, 0, 1);
		}

		private void OnPuzzleButtonClicked(object sender, EventArgs e)
		{
			this.MakeMove(sender as Button);
		}
	}
}
