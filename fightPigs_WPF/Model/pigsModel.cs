using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FightPigs_WPF.Main.Persistance;
using System.Windows.Input;

namespace FightPigs_WPF.Main.Model
{
    public enum size { Small = 4, Medium = 6, Big = 8, NoChange };
    public class pigsModel
    {
        private const int smallSize = 4;
        private const int mediumSize = 6;
        private const int bigSize = 8;

        private gameTable _table;
        private size _size;
        private int _currentPlayer;
        private (Queue<Key>, Queue<Key>) _keyQueue;
        private IFightPigsDataAccess _dataAccess;

        public event EventHandler<pigsEventArgs>? GameAdvanced;

        public int whoWon
        {
            get
            {
                if (_table.Health.Item1 == 0 && _table.Health.Item2 == 0)
                    return 3;
                else if (_table.Health.Item1 <= 0)
                    return 2;
                else if (_table.Health.Item2 <= 0)
                    return 1;
                return 0;
            }
        }
        public (Queue<Key>, Queue<Key>) Key { get { return _keyQueue; } }
        public size Size { get { return _size; } set { _size = value;  } }
        public int Player { get { return _currentPlayer; } }

        public gameTable GameTable { get { return _table; } }

        public pigsModel( IFightPigsDataAccess dataAccess )
        {
            _dataAccess = dataAccess;
            _table = new gameTable();
            _size = size.Medium;
        }


        public void NewGame()
        {
            if (Size != size.NoChange)
            {
                _table = new gameTable(Size == size.Small ? smallSize : Size == size.Medium ? mediumSize : bigSize);
                _currentPlayer = 0;
                _keyQueue = (new Queue<Key>(), new Queue<Key>());
            }
        }

        public void KeyInputHandler(Key keyInput)
        {
            bool stepsOver = false;

            if (_currentPlayer == 0 && _keyQueue.Item1.Count < 5)
                _keyQueue.Item1.Enqueue(keyInput);
            else if (_keyQueue.Item2.Count < 5)
                _keyQueue.Item2.Enqueue(keyInput);

            if (_keyQueue.Item1.Count == 5)
                _currentPlayer = 1;

            if(_keyQueue.Item1.Count + _keyQueue.Item2.Count == 10)
            {
                stepsOver = true;
                if (_keyQueue.Item1.Count != 0 && _keyQueue.Item2.Count != 0)
                    _table.Step(_keyQueue.Item1, _keyQueue.Item2);
                _currentPlayer = 0;
            }

            this.GameAdvanced(this, new pigsEventArgs(stepsOver, _keyQueue.Item1, _keyQueue.Item2, whoWon));
            if(stepsOver) _keyQueue = (new Queue<Key>(), new Queue<Key>());
        }

        public async Task SaveGameAsync(string path)
        {
            if (_dataAccess == null) throw new InvalidOperationException("No dataaccess is provided!");

            await _dataAccess.SaveAsync(path, _table);
        }

        public async Task LoadGameAsync(string path)
        {
            if (_dataAccess == null) throw new InvalidOperationException("No dataaccess is provided!");

            _table = await _dataAccess.LoadAsync(path);
            Size = (size)_table.Size;
        }
    }
}
