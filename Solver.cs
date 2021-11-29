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
            mainPage.UpdateLabels();
            mainPage.PlaySoundFXIfEnabled();
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

        private void MarkSolved(Element[,] puzzleMatrix, int minValue, int maxValue, bool solved)
        {
            for(int i = 0; i < 4; i++)
                for(int j = 0; j < 4; j++)
                    if(minValue <= puzzleMatrix[i, j].value && puzzleMatrix[i, j].value <= maxValue)
                        puzzleMatrix[i, j].solved = solved;
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
                this.MarkSolved(puzzleMatrix, 2, 15, false);
                Location oneLocation;
                this.LocateElement(puzzleMatrix, 1, out oneLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, oneLocation, new Location(0, 0));
            }
            else if(puzzleMatrix[0, 1].value != 2)
            {
                this.MarkSolved(puzzleMatrix, 3, 15, false);
                Location twoLocation;
                this.LocateElement(puzzleMatrix, 2, out twoLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, twoLocation, new Location(0, 1));
            }
            else if (puzzleMatrix[0, 2].value != 3)
            {
                this.MarkSolved(puzzleMatrix, 4, 15, false);
                Location threeLocation;
                this.LocateElement(puzzleMatrix, 3, out threeLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, threeLocation, new Location(0, 2));
            }
            else if (puzzleMatrix[0, 3].value != 4)
            {
                this.MarkSolved(puzzleMatrix, 5, 15, false);

                if(puzzleMatrix[1, 2].value != 4)
                {
                    // Special case: We won't be able to make a path in the following case, but it is a trivial case.
                    if(puzzleMatrix[1, 3].value == 4 && puzzleMatrix[0, 3].value == 0)
                        this.moveList.Add(new Location(1, 3));
                    else
                    {
                        Location fourLocation;
                        this.LocateElement(puzzleMatrix, 4, out fourLocation);
                        this.PrepOrMoveTileCloserToTarget(puzzleMatrix, fourLocation, new Location(1, 2));
                    }
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
                this.MarkSolved(puzzleMatrix, 6, 15, false);
                Location fiveLocation;
                this.LocateElement(puzzleMatrix, 5, out fiveLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, fiveLocation, new Location(1, 0));
            }
            else if(puzzleMatrix[1, 1].value != 6)
            {
                this.MarkSolved(puzzleMatrix, 7, 15, false);
                Location sixLocation;
                this.LocateElement(puzzleMatrix, 6, out sixLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, sixLocation, new Location(1, 1));
            }
            else if(puzzleMatrix[1, 2].value != 7)
            {
                this.MarkSolved(puzzleMatrix, 8, 15, false);
                Location sevenLocation;
                this.LocateElement(puzzleMatrix, 7, out sevenLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, sevenLocation, new Location(1, 2));
            }
            else if(puzzleMatrix[1, 3].value != 8)
            {
                this.MarkSolved(puzzleMatrix, 9, 15, false);

                if (puzzleMatrix[2, 2].value != 8)
                {
                    // Special case: We won't be able to make a path in the following case, but it is a trivial case.
                    if (puzzleMatrix[2, 3].value == 8 && puzzleMatrix[1, 3].value == 0)
                        this.moveList.Add(new Location(2, 3));
                    else
                    {
                        Location eightLocation;
                        this.LocateElement(puzzleMatrix, 8, out eightLocation);
                        this.PrepOrMoveTileCloserToTarget(puzzleMatrix, eightLocation, new Location(2, 2));
                    }
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
            else if(puzzleMatrix[2, 0].value != 9)
            {
                this.MarkSolved(puzzleMatrix, 10, 15, false);
                Location nineLocation;
                this.LocateElement(puzzleMatrix, 9, out nineLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, nineLocation, new Location(2, 0));
            }
            else if(puzzleMatrix[3, 0].value != 13)
            {
                this.MarkSolved(puzzleMatrix, 10, 12, false);
                this.MarkSolved(puzzleMatrix, 14, 15, false);

                if(puzzleMatrix[3, 2].value != 13)
                {
                    // Special case: We won't be able to make a path in the following case, but it is a trivial case.
                    if (puzzleMatrix[3, 1].value == 13 && puzzleMatrix[3, 0].value == 0)
                        this.moveList.Add(new Location(3, 1));
                    else
                    {
                        Location thirteenLocation;
                        this.LocateElement(puzzleMatrix, 13, out thirteenLocation);
                        this.PrepOrMoveTileCloserToTarget(puzzleMatrix, thirteenLocation, new Location(3, 2));
                    }
                }
                else if(puzzleMatrix[2, 1].value != 0)
                {
                    Location zeroLocation;
                    this.LocateElement(puzzleMatrix, 0, out zeroLocation);
                    this.FindPath(puzzleMatrix, zeroLocation, new Location(2, 1), new Location(3, 2));
                }
                else
                {
                    // This is truncated commutator.  The desired result is found before finishing the commutator.

                    this.moveList.Add(new Location(3, 1));
                    this.moveList.Add(new Location(3, 0));
                    this.moveList.Add(new Location(2, 0));
                    this.moveList.Add(new Location(2, 1));

                    this.moveList.Add(new Location(3, 1));
                    this.moveList.Add(new Location(3, 2));
                    this.moveList.Add(new Location(2, 2));
                    this.moveList.Add(new Location(2, 1));

                    this.moveList.Add(new Location(2, 0));
                    this.moveList.Add(new Location(3, 0));
                    this.moveList.Add(new Location(3, 1));
                }
            }
            else if(puzzleMatrix[2, 1].value != 10)
            {
                this.MarkSolved(puzzleMatrix, 11, 12, false);
                this.MarkSolved(puzzleMatrix, 14, 15, false);
                Location tenLocation;
                this.LocateElement(puzzleMatrix, 10, out tenLocation);
                this.PrepOrMoveTileCloserToTarget(puzzleMatrix, tenLocation, new Location(2, 1));
            }
            else if(puzzleMatrix[3, 1].value != 14)
            {
                this.MarkSolved(puzzleMatrix, 11, 12, false);
                this.MarkSolved(puzzleMatrix, 15, 15, false);

                if(puzzleMatrix[3, 3].value != 14)
                {
                    // Special case: We won't be able to make a path in the following case, but it is a trivial case.
                    if(puzzleMatrix[3, 2].value == 14 && puzzleMatrix[3, 1].value == 0)
                        this.moveList.Add(new Location(3, 2));
                    else
                    {
                        Location fourteenLocation;
                        this.LocateElement(puzzleMatrix, 14, out fourteenLocation);
                        this.PrepOrMoveTileCloserToTarget(puzzleMatrix, fourteenLocation, new Location(3, 3));
                    }
                }
                else if(puzzleMatrix[2, 2].value != 0)
                {
                    Location zeroLocation;
                    this.LocateElement(puzzleMatrix, 0, out zeroLocation);
                    this.FindPath(puzzleMatrix, zeroLocation, new Location(2, 2), new Location(3, 3));
                }
                else
                {
                    // This is also a truncated commutator.

                    this.moveList.Add(new Location(3, 2));
                    this.moveList.Add(new Location(3, 1));
                    this.moveList.Add(new Location(2, 1));
                    this.moveList.Add(new Location(2, 2));

                    this.moveList.Add(new Location(3, 2));
                    this.moveList.Add(new Location(3, 3));
                    this.moveList.Add(new Location(2, 3));
                    this.moveList.Add(new Location(2, 2));

                    this.moveList.Add(new Location(2, 1));
                    this.moveList.Add(new Location(3, 1));
                    this.moveList.Add(new Location(3, 2));
                }
            }
            else if(puzzleMatrix[2, 2].value != 11 || puzzleMatrix[2, 3].value != 12 || puzzleMatrix[3, 2].value != 15 || puzzleMatrix[3, 3].value != 0)
            {
                if(puzzleMatrix[2, 2].value == 0)
                    this.moveList.Add(new Location(2, 3));
                else if(puzzleMatrix[2, 3].value == 0)
                    this.moveList.Add(new Location(3, 3));
                else if(puzzleMatrix[3, 3].value == 0)
                    this.moveList.Add(new Location(3, 2));
                else if(puzzleMatrix[3, 2].value == 0)
                    this.moveList.Add(new Location(2, 2));
            }
        }

        private bool FindPath(Element[,] puzzleMatrix, Location sourceLocation, Location destinationLocation, Location doNotDisturbLocation)
        {
            for(int i = 0; i < 4; i++)
                for(int j = 0; j < 4; j++)
                    puzzleMatrix[i, j].visited = false;

            puzzleMatrix[doNotDisturbLocation.row, doNotDisturbLocation.col].solved = true;

            // TODO: Note that this does not necessarily find the shortest path.
            //       It may be worth revisiting this code so that it finds the shortest path.
            //       In fact, it must be revisited, because it's an obvious innefficiency.
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
