using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifteenPuzzle
{
    public class Solver
    {
        public Solver()
        {
        }

        private struct Location
        {
            public Location(int givenRow, int givenCol)
            {
                this.row = givenRow;
                this.col = givenCol;
            }

            public int row, col;
        }

        private List<Location> moveList = new List<Location>();

        private struct Element
        {
            public int value;
            public bool solved;
            public bool visited;
        }

        public bool Run(MainPage mainPage)
        {
            if(moveList.Count == 0)
            {
                int k = 1;
                Element[,] puzzleMatrix = new Element[4,4];
                for(int i = 0; i < 4; i++)
                {
                    for(int j = 0; j < 4; j++)
                    {
                        Element element = new Element();
                        element.value = mainPage.GetPuzzleValue(i, j);
                        element.solved = (element.value == k % 16);
                        puzzleMatrix[i, j] = element;
                        k++;
                    }
                }

                this.GenerateMoveSequence(puzzleMatrix);
                if(moveList.Count == 0)
                    return false;
            }

            Location move = moveList[0];
            moveList.RemoveAt(0);
            mainPage.MakeMove(move.row, move.col);
            return true;
        }

        private void LocateElement(Element[,] puzzleMatrix, int value, out Location location)
        {
            location.row = -1;
            location.col = -1;
            for(location.row = 0; location.row < 4; location.row++)
                for(location.col = 0; location.col < 4; location.col++)
                    if(puzzleMatrix[location.row, location.col].value == value)
                        return;
        }

        private void MarkUnsolved(Element[,] puzzleMatrix, int minValue, int maxValue)
        {
            for(int i = 0; i < 4; i++)
                for(int j = 0; j < 4; j++)
                    if(minValue <= puzzleMatrix[i, j].value && puzzleMatrix[i, j].value <= maxValue)
                        puzzleMatrix[i, j].solved = false;
        }

        private void GenerateMoveSequence(Element[,] puzzleMatrix)
        {
            this.moveList.Clear();

            if(puzzleMatrix[0, 0].value != 1)
            {
                this.MarkUnsolved(puzzleMatrix, 2, 15);

                Location oneLocation;
                this.LocateElement(puzzleMatrix, 1, out oneLocation);

                Location zeroLocation;
                this.LocateElement(puzzleMatrix, 0, out zeroLocation);

                if(oneLocation.row > 0)
                {
                    Location targetLocation = new Location(oneLocation.row - 1, oneLocation.col);
                    if(zeroLocation.row == targetLocation.row && zeroLocation.col == targetLocation.col)
                        this.moveList.Add(oneLocation);     // Move one tile closer to the solved location.
                    else
                        this.FindPath(puzzleMatrix, zeroLocation, targetLocation, oneLocation);
                }
                else if(oneLocation.col > 0)
                {
                    Location targetLocation = new Location(oneLocation.row, oneLocation.col - 1);
                    if(zeroLocation.row == targetLocation.row && zeroLocation.col == targetLocation.col)
                        this.moveList.Add(oneLocation);     // Move one tile closer to the solved location.
                    else
                        this.FindPath(puzzleMatrix, zeroLocation, targetLocation, oneLocation);
                }
            }
            else if(puzzleMatrix[0, 1].value != 2)
            {
                this.MarkUnsolved(puzzleMatrix, 3, 15);

                Location twoLocation;
                this.LocateElement(puzzleMatrix, 2, out twoLocation);

                Location zeroLocation;
                this.LocateElement(puzzleMatrix, 0, out zeroLocation);

                if (twoLocation.row > 0)
                {
                    Location targetLocation = new Location(twoLocation.row - 1, twoLocation.col);
                    if (zeroLocation.row == targetLocation.row && zeroLocation.col == targetLocation.col)
                        this.moveList.Add(twoLocation);     // Move one tile closer to the solved location.
                    else
                        this.FindPath(puzzleMatrix, zeroLocation, targetLocation, twoLocation);
                }
                else if(twoLocation.col > 1)
                {
                    Location targetLocation = new Location(twoLocation.row, twoLocation.col - 1);
                    if (zeroLocation.row == targetLocation.row && zeroLocation.col == targetLocation.col)
                        this.moveList.Add(twoLocation);     // Move one tile closer to the solved location.
                    else
                        this.FindPath(puzzleMatrix, zeroLocation, targetLocation, twoLocation);
                }
                else if(twoLocation.col < 1)
                {
                    Location targetLocation = new Location(twoLocation.row, twoLocation.col + 1);
                    if (zeroLocation.row == targetLocation.row && zeroLocation.col == targetLocation.col)
                        this.moveList.Add(twoLocation);     // Move one tile closer to the solved location.
                    else
                        this.FindPath(puzzleMatrix, zeroLocation, targetLocation, twoLocation);
                }
            }
        }

        private void FindPath(Element[,] puzzleMatrix, Location sourceLocation, Location destinationLocation, Location doNotDisturbLocation)
        {
            for(int i = 0; i < 4; i++)
                for(int j = 0; j < 4; j++)
                    puzzleMatrix[i, j].visited = false;

            puzzleMatrix[doNotDisturbLocation.row, doNotDisturbLocation.col].solved = true;

            // Note that this does not necessarily find the shortest path.
            // It may be worth revisiting this code so that it finds the shortest path.
            this.moveList.Clear();
            this.FindPathRecursive(puzzleMatrix, sourceLocation, destinationLocation);
            if(this.moveList.Count > 0)
                this.moveList.RemoveAt(0);
        }

        private bool FindPathRecursive(Element[,] puzzleMatrix, Location currentLocation, Location destinationLocation)
        {
            puzzleMatrix[currentLocation.row, currentLocation.col].visited = true;

            if(currentLocation.row == destinationLocation.row && currentLocation.col == destinationLocation.col)
            {
                this.moveList.Add(currentLocation);
                return true;
            }

            if(currentLocation.row > 0 &&
                !puzzleMatrix[currentLocation.row - 1, currentLocation.col].solved &&
                !puzzleMatrix[currentLocation.row - 1, currentLocation.col].visited)
            {
                Location nextLocation = new Location(currentLocation.row - 1, currentLocation.col);
                if(this.FindPathRecursive(puzzleMatrix, nextLocation, destinationLocation))
                {
                    this.moveList.Insert(0, currentLocation);
                    return true;
                }
            }

            if(currentLocation.row < 3 &&
                !puzzleMatrix[currentLocation.row + 1, currentLocation.col].solved &&
                !puzzleMatrix[currentLocation.row + 1, currentLocation.col].visited)
            {
                Location nextLocation = new Location(currentLocation.row + 1, currentLocation.col);
                if (this.FindPathRecursive(puzzleMatrix, nextLocation, destinationLocation))
                {
                    this.moveList.Insert(0, currentLocation);
                    return true;
                }
            }

            if (currentLocation.col > 0 &&
                !puzzleMatrix[currentLocation.row, currentLocation.col - 1].solved &&
                !puzzleMatrix[currentLocation.row, currentLocation.col - 1].visited)
            {
                Location nextLocation = new Location(currentLocation.row, currentLocation.col - 1);
                if (this.FindPathRecursive(puzzleMatrix, nextLocation, destinationLocation))
                {
                    this.moveList.Insert(0, currentLocation);
                    return true;
                }
            }

            if (currentLocation.col < 3 &&
                !puzzleMatrix[currentLocation.row, currentLocation.col + 1].solved &&
                !puzzleMatrix[currentLocation.row, currentLocation.col + 1].visited)
            {
                Location nextLocation = new Location(currentLocation.row, currentLocation.col + 1);
                if (this.FindPathRecursive(puzzleMatrix, nextLocation, destinationLocation))
                {
                    this.moveList.Insert(0, currentLocation);
                    return true;
                }
            }

            puzzleMatrix[currentLocation.row, currentLocation.col].visited = false;
            return false;
        }
    }
}
