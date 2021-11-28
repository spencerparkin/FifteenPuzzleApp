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

        private void PrepOrMoveTileCloserToTarget(Element[,] puzzleMatrix, Location tileLocation, Location ultimateLocation)
        {
            this.moveList.Clear();

            Location zeroLocation;
            this.LocateElement(puzzleMatrix, 0, out zeroLocation);

            List<Func<Location, Location, bool>> comparisonList = new List<Func<Location, Location, bool>>();
            comparisonList.Add((locationA, locationB) => { return locationA.row < locationB.row; });
            comparisonList.Add((locationA, locationB) => { return locationA.row > locationB.row; });
            comparisonList.Add((locationA, locationB) => { return locationA.col < locationB.col; });
            comparisonList.Add((locationA, locationB) => { return locationA.col > locationB.col; });

            List<Location> locationDeltaList = new List<Location>();
            locationDeltaList.Add(new Location(1, 0));
            locationDeltaList.Add(new Location(-1, 0));
            locationDeltaList.Add(new Location(0, 1));
            locationDeltaList.Add(new Location(0, -1));

            for(int i = 0; i < 4; i++)
            {
                if(comparisonList[i](tileLocation, ultimateLocation))
                {
                    Location deltaLocation = locationDeltaList[i];
                    Location targetLocation = new Location(tileLocation.row + deltaLocation.row, tileLocation.col + deltaLocation.col);
                    if (zeroLocation.row == targetLocation.row && zeroLocation.col == targetLocation.col)
                        this.moveList.Add(tileLocation);     // Move tile closer to the ultimate location.
                    else
                        this.FindPath(puzzleMatrix, zeroLocation, targetLocation, tileLocation);

                    if (this.moveList.Count > 0)
                        break;
                }
            }
        }

        private void GenerateMoveSequence(Element[,] puzzleMatrix)
        {
            this.moveList.Clear();

            if(puzzleMatrix[0, 0].value != 1)
            {
                this.MarkUnsolved(puzzleMatrix, 2, 15);
                Location oneLocation;
                this.LocateElement(puzzleMatrix, 1, out oneLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, oneLocation, new Location(0, 0));
            }
            else if(puzzleMatrix[0, 1].value != 2)
            {
                this.MarkUnsolved(puzzleMatrix, 3, 15);
                Location twoLocation;
                this.LocateElement(puzzleMatrix, 2, out twoLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, twoLocation, new Location(0, 1));
            }
            else if (puzzleMatrix[0, 2].value != 3)
            {
                this.MarkUnsolved(puzzleMatrix, 4, 15);
                Location threeLocation;
                this.LocateElement(puzzleMatrix, 3, out threeLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, threeLocation, new Location(0, 2));
            }
            else if (puzzleMatrix[0, 3].value != 4)
            {
                this.MarkUnsolved(puzzleMatrix, 5, 15);

                if(puzzleMatrix[1, 2].value != 4)
                {
                    Location fourLocation;
                    this.LocateElement(puzzleMatrix, 4, out fourLocation);
                    this.PrepOrMoveTileCloserToTarget(puzzleMatrix, fourLocation, new Location(1, 2));
                }
                else if(puzzleMatrix[1, 0].value != 0)
                {
                    Location zeroLocation;
                    this.LocateElement(puzzleMatrix, 0, out zeroLocation);
                    this.FindPath(puzzleMatrix, zeroLocation, new Location(1, 0), new Location(1, 2));
                }
                else
                {
                    // Move string of 1-2-3-4 left.
                    this.moveList.Add(new Location(0, 0));
                    this.moveList.Add(new Location(0, 1));
                    this.moveList.Add(new Location(0, 2));
                    this.moveList.Add(new Location(1, 2));

                    // Make room for the 4.
                    this.moveList.Add(new Location(1, 3));
                    this.moveList.Add(new Location(0, 3));

                    // Move string of 1-2-3-4 into place.
                    this.moveList.Add(new Location(0, 2));
                    this.moveList.Add(new Location(0, 1));
                    this.moveList.Add(new Location(0, 0));
                    this.moveList.Add(new Location(1, 0));
                }
            }
            else if(puzzleMatrix[1, 0].value != 5)
            {
                this.MarkUnsolved(puzzleMatrix, 6, 15);
                Location fiveLocation;
                this.LocateElement(puzzleMatrix, 5, out fiveLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, fiveLocation, new Location(1, 0));
            }
            else if(puzzleMatrix[1, 1].value != 6)
            {
                this.MarkUnsolved(puzzleMatrix, 7, 15);
                Location sixLocation;
                this.LocateElement(puzzleMatrix, 6, out sixLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, sixLocation, new Location(1, 1));
            }
            else if(puzzleMatrix[1, 2].value != 7)
            {
                this.MarkUnsolved(puzzleMatrix, 8, 15);
                Location sevenLocation;
                this.LocateElement(puzzleMatrix, 7, out sevenLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, sevenLocation, new Location(1, 2));
            }
            else if(puzzleMatrix[1, 3].value != 8)
            {
                this.MarkUnsolved(puzzleMatrix, 9, 15);

                if (puzzleMatrix[2, 2].value != 8)
                {
                    Location eightLocation;
                    this.LocateElement(puzzleMatrix, 8, out eightLocation);
                    this.PrepOrMoveTileCloserToTarget(puzzleMatrix, eightLocation, new Location(2, 2));
                }
                else if (puzzleMatrix[2, 0].value != 0)
                {
                    Location zeroLocation;
                    this.LocateElement(puzzleMatrix, 0, out zeroLocation);
                    this.FindPath(puzzleMatrix, zeroLocation, new Location(2, 0), new Location(2, 2));
                }
                else
                {
                    // Move string of 5-6-7-8 left.
                    this.moveList.Add(new Location(1, 0));
                    this.moveList.Add(new Location(1, 1));
                    this.moveList.Add(new Location(1, 2));
                    this.moveList.Add(new Location(2, 2));

                    // Make room for the 8.
                    this.moveList.Add(new Location(2, 3));
                    this.moveList.Add(new Location(1, 3));

                    // Move string of 5-6-7-8 into place.
                    this.moveList.Add(new Location(1, 2));
                    this.moveList.Add(new Location(1, 1));
                    this.moveList.Add(new Location(1, 0));
                    this.moveList.Add(new Location(2, 0));
                }
            }
        }

        private bool FindPath(Element[,] puzzleMatrix, Location sourceLocation, Location destinationLocation, Location doNotDisturbLocation)
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

            return this.moveList.Count > 0;
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
