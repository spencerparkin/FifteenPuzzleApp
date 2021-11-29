using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Essentials;

namespace FifteenPuzzle
{
    public class Settings
    {
        public bool makeFartSounds = false;
        public bool highlightSolvedTiles = false;

        public void Save()
        {
            Preferences.Set("makeFartSounds", this.makeFartSounds);
            Preferences.Set("highlightSolvedTiles", this.highlightSolvedTiles);
        }

        public void Load()
        {
            this.makeFartSounds = Preferences.Get("makeFartSounds", false);
            this.highlightSolvedTiles = Preferences.Get("highlightSolvedTiles", this.highlightSolvedTiles);
        }
    }
}
