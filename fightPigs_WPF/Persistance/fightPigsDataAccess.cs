using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FightPigs_WPF.Main.Persistance
{
    public class FightPigsDataAccess : IFightPigsDataAccess
    {
        public async Task<gameTable> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    int[] firstLine = Array.ConvertAll(reader.ReadLine().Split(' '), int.Parse);
                    int tableSize = firstLine[0];
                    (int, int) health = (firstLine[1], firstLine[2]);
                    int[] secondLine = Array.ConvertAll(reader.ReadLine().Split(' '), int.Parse);
                    (int, int) p1IDX = (secondLine[0], secondLine[1]), p2IDX = (secondLine[2], secondLine[3]);
                    int[] thirdLine = Array.ConvertAll(reader.ReadLine().Split(' '), int.Parse);

                    gameTable table = new gameTable(tableSize, p1IDX, p2IDX, health);
                    table[p1IDX.Item1, p1IDX.Item2] = (gameTable.Direction)thirdLine[0];
                    table[p2IDX.Item1, p2IDX.Item2] = (gameTable.Direction)thirdLine[1];

                    return table;
                }
            }
            catch { throw new FightPigsDataException(); }
        }

        public async Task SaveAsync(string path, gameTable table)
        {
            try
            {
                
                using (StreamWriter writer = new StreamWriter(path))
                {
                    
                    writer.WriteLine($"{table.Size} {table.Health.Item1} {table.Health.Item2}");
                    writer.WriteLine($"{table.GetPlayer(1).Item1} {table.GetPlayer(1).Item2} {table.GetPlayer(2).Item1} {table.GetPlayer(2).Item2}");
                    writer.WriteLine($"{(int)table.GetValue(table.GetPlayer(1)).Value} {(int)table.GetValue(table.GetPlayer(2)).Value}");
                }
            }
            catch { throw new FightPigsDataException(); }
        }
    }
}
