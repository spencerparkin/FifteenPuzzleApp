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

		private void OnScrambleButtonClicked(object sender, EventArgs e)
		{
			this.UpdateMoveCount(0);

			// TODO: It is not immediately obvious to me if every permutation
			//       of the 16 locations results in a solvable puzzle.  The only
			//       way, for now, to be sure that a scramble is solvable is to
			//       scramble the puzzle under the constraints of the puzzle.

			int[] array = new int[16];
			for(int i = 0; i < array.Length; i++)
				array[i] = i;

			for(int i = 0; i < array.Length - 1; i++)
			{
				int j = this.random.Next(i, array.Length);
				if(i != j)
				{
					array[i] ^= array[j];
					array[j] ^= array[i];
					array[i] ^= array[j];
				}
			}

			for(int i = 0; i < array.Length; i++)
			{
				int row = (int)Math.Floor((float)i / 4.0f);
				int col = i % 4;
				Button button = this.buttonMatrix[row, col];
				button.Text = (array[i] == 0) ? "" : $"{array[i]}";
			}
		}

		private bool FindButtonLocation(Button button, out int row, out int col)
		{
			for (row = 0; row < 4; row++)
				for (col = 0; col < 4; col++)
					if (this.buttonMatrix[row, col] == button)
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

		private void OnPuzzleButtonClicked(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if(button == null)
				return;

			int row = -1, col = -1;
			if(!this.FindButtonLocation(button, out row, out col))
				return;

			if(row > 0 && this.buttonMatrix[row - 1, col].Text.Length == 0)
				this.MakeMove(row, col, -1, 0);
			else if(row < 3 && this.buttonMatrix[row + 1, col].Text.Length == 0)
				this.MakeMove(row, col, 1, 0);
			else if(col > 0 && this.buttonMatrix[row, col - 1].Text.Length == 0)
				this.MakeMove(row, col, 0, -1);
			else if(col < 3 && this.buttonMatrix[row, col + 1].Text.Length == 0)
				this.MakeMove(row, col, 0, 1);
		}
	}
}
