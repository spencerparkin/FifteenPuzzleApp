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
	}
}
