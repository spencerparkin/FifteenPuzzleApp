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

        private struct Move
        {
            public int row, col;
        }

        private List<Move> moveList = new List<Move>();

        public bool Run(MainPage mainPage)
        {
            if(moveList.Count == 0)
            {
                // TODO: Generate a move list here.  If the puzzle is solved, return false.
                return false;
            }

            Move move = moveList[0];
            moveList.RemoveAt(0);
            mainPage.MakeMove(move.row, move.col);
            return true;
        }
    }
}
