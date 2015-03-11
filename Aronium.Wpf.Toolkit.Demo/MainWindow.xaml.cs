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
        private string _themeName = "Light";
        private List<string> _themes;

        public MainWindow()
        {
            InitializeComponent();

            this.Themes = new List<string>(new[] { "Light", "Dark" });

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

        public string ThemeName
        {
            get { return _themeName; }
            set
            {
                if (_themeName != value)
                {
                    var currentResourceIndex = GetResourceIndex();

                    _themeName = value; 
                    
                    OnPropertyChanged("ThemeName");

                    ChangeTheme(currentResourceIndex);
                }
            }
        }

        private int GetResourceIndex()
        {
            string source = string.Format("pack://application:,,,/Aronium.Wpf.Toolkit;component/Themes/Brushes/{0}.xaml", this.ThemeName);

            var existing = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x.Source.Equals(source));

            if (existing != null)
                return Application.Current.Resources.MergedDictionaries.IndexOf(existing);
            else return -1;
        }

        private void ChangeTheme(int dictionaryIndex)
        {
            if (dictionaryIndex >= 0)
            {
                this.Cursor = Cursors.Wait;
                string source = string.Format("pack://application:,,,/Aronium.Wpf.Toolkit;component/Themes/Brushes/{0}.xaml", this.ThemeName);

                Application.Current.Resources.MergedDictionaries.RemoveAt(dictionaryIndex);
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(source, UriKind.RelativeOrAbsolute) });

                this.Cursor = Cursors.Arrow;
            }
        }

        public List<string> Themes
        {
            get { return _themes; }
            set
            {
                this._themes = value;
                OnPropertyChanged("Themes");
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
