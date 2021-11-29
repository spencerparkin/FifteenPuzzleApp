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
        public float autoSolveSpeed = 50.0f;

        public void Save()
        {
            Preferences.Set("makeFartSounds", this.makeFartSounds);
            Preferences.Set("highlightSolvedTiles", this.highlightSolvedTiles);
            Preferences.Set("autoSolveSpeed", this.autoSolveSpeed);
        }

        public void Load()
        {
            this.makeFartSounds = Preferences.Get("makeFartSounds", this.makeFartSounds);
            this.highlightSolvedTiles = Preferences.Get("highlightSolvedTiles", this.highlightSolvedTiles);
            this.autoSolveSpeed = Preferences.Get("autoSolveSpeed", this.autoSolveSpeed);
        }
    }
}
