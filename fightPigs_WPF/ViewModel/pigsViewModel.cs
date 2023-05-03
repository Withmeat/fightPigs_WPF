using fightPigs_WPF.Main.ViewModel;
using FightPigs_WPF.Main.Model;
using FightPigs_WPF.Main.Persistance;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using fightPigs_WPF.Main.View;

namespace fightPigs_WPF.Main.ViewModel
{
    public class pigsViewModel : ViewModelBase
    {
        private String _rootPath = String.Join(@"\", Directory.GetCurrentDirectory().Split(@"\").TakeWhile(y => String.Compare("bin", y, true) != 0).ToArray());
        public pigsModel _model;
        public ObservableCollection<pigsField> Fields { get; set; }
        public String NextStepInfo { get; set; }
        public String HealthInfo { get; set; }
        public Int32 Size
        {
            get => _model.GameTable.Size;
        }

        public DelegateCommand SmallGameCommand { get; private set; }
        public DelegateCommand MediumGameCommand { get; private set; }
        public DelegateCommand BigGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }


        public event EventHandler? SmallGame;
        public event EventHandler? MediumGame;
        public event EventHandler? BigGame;
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;

        public pigsViewModel(pigsModel model)
        {
            _model = model;
            Fields = new ObservableCollection<pigsField>();

            _model.GameAdvanced += new EventHandler<pigsEventArgs>(Model_GameAdvanced);

            SmallGameCommand = new DelegateCommand(param => OnSmallGame());
            MediumGameCommand = new DelegateCommand(param => OnMediumGame());
            BigGameCommand = new DelegateCommand(param => OnBigGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            NewTable();
        }

        private void NewTable()
        {
            Fields.Clear();

            for (Int32 i = 0; i < _model.GameTable.Size; i++)
                for (Int32 j = 0; j < _model.GameTable.Size; j++)
                {
                    Fields.Add(new pigsField
                    {
                        Image = "",
                        X = j,
                        Y = i
                    });
                }

            NextStepInfo = $"Jelenleg a {(_model.Player == 0 ? "piros" : "kék")} játékos van soron. | " +
                $"Piros: | Kék: ";
            HealthInfo = $"Piros: {_model.GameTable.Health.Item1}hp | Kék: {_model.GameTable.Health.Item2}hp";

            OnPropertyChanged(nameof(NextStepInfo));
            OnPropertyChanged(nameof(HealthInfo));

            RefreshTable();
        }

        private void RefreshTable()
        {
            foreach(pigsField field in Fields)
            {
                String imgPath = "";
                switch (_model.GameTable[field.X, field.Y])
                {
                    case gameTable.Direction.Up: imgPath = String.Concat($"{_rootPath}/PlayerImgs/pigUp", (field.X, field.Y) == _model.GameTable.GetPlayer(1) ? "Red.png" : "Blue.png"); break;
                    case gameTable.Direction.Right: imgPath = String.Concat($"{_rootPath}/PlayerImgs/pigRight", (field.X, field.Y) == _model.GameTable.GetPlayer(1) ? "Red.png" : "Blue.png"); break;
                    case gameTable.Direction.Down: imgPath = String.Concat($"{_rootPath}/PlayerImgs/pigDown", (field.X, field.Y) == _model.GameTable.GetPlayer(1) ? "Red.png" : "Blue.png"); break;
                    case gameTable.Direction.Left: imgPath = String.Concat($"{_rootPath}/PlayerImgs/pigLeft", (field.X, field.Y) == _model.GameTable.GetPlayer(1) ? "Red.png" : "Blue.png"); break;
                }

                field.Image = imgPath;
            }
        }
        private void Model_GameAdvanced(object? sender, pigsEventArgs e)
        {
            if( e.stepsOver )
                RefreshTable();

            String redStepsText = "";
            String blueStepsText = "";
            foreach (Key k in e.redSteps)
                switch (k)
                {
                    case Key.Up: redStepsText += " \u2191"; break;
                    case Key.Down: redStepsText += " \u2193"; break;
                    case Key.Right: redStepsText += " \u2192"; break;
                    case Key.Left: redStepsText += " \u2190"; break;
                    case Key.Q: redStepsText += " \u21BA"; break;
                    case Key.R: redStepsText += " \u21BB"; break;
                    case Key.W: redStepsText += " \u00A6"; break;
                    case Key.E: redStepsText += " \u00A4"; break;
                    default: redStepsText += ' ' + k.ToString(); break;
                }
            foreach (Key k in e.blueSteps)
                switch (k)
                {
                    case Key.Up: blueStepsText += " \u2191"; break;
                    case Key.Down: blueStepsText += " \u2193"; break;
                    case Key.Right: blueStepsText += " \u2192"; break;
                    case Key.Left: blueStepsText += " \u2190"; break;
                    case Key.Q: blueStepsText += " \u21BA"; break;
                    case Key.R: blueStepsText += " \u21BB"; break;
                    case Key.W: blueStepsText += " \u00A6"; break;
                    case Key.E: blueStepsText += " \u00A4"; break;
                    default: blueStepsText += ' ' + k.ToString(); break;
                }

            NextStepInfo = $"Jelenleg a {(_model.Player == 0 ? "piros" : "kék")} játékos van soron. | " +
                $"Piros: {redStepsText} | Kék: {blueStepsText}";
            HealthInfo = $"Piros: {_model.GameTable.Health.Item1}hp | Kék: {_model.GameTable.Health.Item2}hp";

            OnPropertyChanged(nameof(NextStepInfo));
            OnPropertyChanged(nameof(HealthInfo));

            if (e.whoWon != 0)
            {
                MessageBox.Show(e.whoWon == 1 ? "Piros nyert!" : e.whoWon == 2 ? "Kék nyert!" : "Döntetlen!",
                    "Játéknak vége!", MessageBoxButton.OK, MessageBoxImage.Information);
                _model.NewGame();
                NewTable();
            }
        }


        public void OnSmallGame()
        {
            SmallGame?.Invoke(this, EventArgs.Empty);
            NewTable();
            OnPropertyChanged(nameof(Size));
        }
        public void OnMediumGame()
        {
            MediumGame?.Invoke(this, EventArgs.Empty);
            NewTable();
            OnPropertyChanged(nameof(Size));
        }
        public void OnBigGame()
        {
            BigGame?.Invoke(this, EventArgs.Empty);
            NewTable();
            OnPropertyChanged(nameof(Size));
        }
        public void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }
        public void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);

            NextStepInfo = $"Jelenleg a {(_model.Player == 0 ? "piros" : "kék")} játékos van soron. | " +
                $"Piros: | Kék: ";
            HealthInfo = $"Piros: {_model.GameTable.Health.Item1}hp | Kék: {_model.GameTable.Health.Item2}hp";

            NewTable();
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(NextStepInfo));
            OnPropertyChanged(nameof(HealthInfo));
        }
    }
}
