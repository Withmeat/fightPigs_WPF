using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fightPigs_WPF.Main.ViewModel
{
    public class pigsField : ViewModelBase
    {
        private String _image;
        public String Image
        {
            get => _image;
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
    }
}
