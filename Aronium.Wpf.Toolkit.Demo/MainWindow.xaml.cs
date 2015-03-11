using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aronium.Wpf.Toolkit.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _integerProperty;
        
        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = this;

            this.IntegerProperty = new Random().Next(0, 1000);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int IntegerProperty
        {
            get { return _integerProperty; }
            set { _integerProperty = value; OnPropertyChanged("IntegerProperty"); }
        }

        public IEnumerable<Dock> TabPlacementValues
        {
            get
            {
                return Enum.GetValues(typeof(Dock)).Cast<Dock>();
            }
        }
    }
}
