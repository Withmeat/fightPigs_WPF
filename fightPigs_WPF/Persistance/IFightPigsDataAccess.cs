using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FightPigs_WPF.Main.Persistance
{
    public interface IFightPigsDataAccess
    {
        Task<gameTable> LoadAsync(string Path);
        Task SaveAsync(String path, gameTable table);
    }
}
