using fightPigs_WPF.Main.View;
using fightPigs_WPF.Main.ViewModel;
using FightPigs_WPF.Main.Model;
using FightPigs_WPF.Main.Persistance;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace fightPigs_WPF.Main
{
    public partial class App : Application
    {
        #region Fields

        private pigsModel _model = null!;
        private pigsViewModel _viewModel = null!;
        private MainWindow _view = null!;

        #endregion

        #region Constructor

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion

        #region Application event handler
        
        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _model = new pigsModel(new FightPigsDataAccess());
            _model.NewGame();

            _viewModel = new pigsViewModel(_model);
            _viewModel.SmallGame += new EventHandler(ViewModel_SmallGame);
            _viewModel.MediumGame += new EventHandler(ViewModel_MediumGame);
            _viewModel.BigGame += new EventHandler(ViewModel_BigGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);

            _view = new MainWindow();
            _view.KeyDown += OnKeyPress;
            _view.DataContext = _viewModel;
            _view.Show();
        }

        #endregion

        public void OnKeyPress(object? sender, KeyEventArgs pressedKey)
        {
            Key keyData = pressedKey.Key;
            if (keyData == Key.Down || keyData == Key.Up || keyData == Key.Left ||
                keyData == Key.Right || keyData == Key.Q || keyData == Key.R ||
                keyData == Key.W || keyData == Key.E)
                _model.KeyInputHandler(keyData);
        }

        private void ViewModel_SmallGame(object? sender, EventArgs e)
        {
            _model.Size = size.Small;
            _model.NewGame();
        }
        private void ViewModel_MediumGame(object? sender, EventArgs e)
        {
            _model.Size = size.Medium;
            _model.NewGame();
        }
        private void ViewModel_BigGame(object? sender, EventArgs e)
        {
            _model.Size = size.Big;
            _model.NewGame();
        }
        private async void ViewModel_LoadGame(object? sender, EventArgs e)
        {
                try
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = "Load FightPigs Game";
                    openFileDialog.Filter = "PigTable|*.pig";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        await _model.LoadGameAsync(openFileDialog.FileName);
                    }
                }
                catch (FightPigsDataException)
                {
                    MessageBox.Show("Loading game was unsuccessful!" + Environment.NewLine + "Wrong path or file format.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save FightPigs Game";
                saveFileDialog.Filter = "PigTable|*.pig";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (FightPigsDataException)
                    {
                        MessageBox.Show("Unsuccessful save!" + Environment.NewLine + "Wron path or insufficient permission", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Save was unsuccessful!", "FightPigs", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
