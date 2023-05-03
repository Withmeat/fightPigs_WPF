using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FightPigs_WPF.Main.Model
{
    public class pigsEventArgs
    {
        public readonly bool stepsOver;
        public readonly Queue<Key> redSteps;
        public readonly Queue<Key> blueSteps;
        public readonly int whoWon;

        public pigsEventArgs(bool stepsOver, Queue<Key> redSteps, Queue<Key> blueSteps, int whoWon)
        { 
            this.stepsOver = stepsOver;
            this.redSteps = redSteps;
            this.blueSteps = blueSteps;
            this.whoWon = whoWon;
        }   
    }
}
