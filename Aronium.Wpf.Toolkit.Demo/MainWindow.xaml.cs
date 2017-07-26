using Aronium.Wpf.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Aronium.Wpf.Toolkit.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _integerProperty;
        private decimal _decimalProperty;
        private string _themeName = "Dark";
        private List<string> _themes;
        private bool _showTabControlBorder;
        private string _selectedTag;
        List<User> _users;
        private string _searchText;
        List<string> countries = new List<string>(new[] { "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua & Barbuda", "Argentina", "Armenia", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia & Herzegovina", "Botswana", "Brazil", "Brunei Darussalam", "Bulgaria", "Burkina Faso", "Myanmar/Burma", "Burundi", "Cambodia", "Cameroon", "Canada", "Cape Verde", "Cayman Islands", "Central African Republic", "Chad", "Chile", "China", "Colombia", "Comoros", "Congo", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czech Republic", "Democratic Republic of the Congo", "Denmark", "Djibouti", "Dominican Republic", "Dominica", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Ethiopia", "Fiji", "Finland", "France", "French Guiana", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Great Britain", "Greece", "Grenada", "Guadeloupe", "Guatemala", "Guinea", "Guinea-Bissau", "Guyana", "Haiti", "Honduras", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Israel and the Occupied Territories", "Italy", "Ivory Coast (Cote d'Ivoire)", "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kosovo", "Kuwait", "Kyrgyz Republic (Kyrgyzstan)", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Republic of Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Martinique", "Mauritania", "Mauritius", "Mayotte", "Mexico", "Moldova, Republic of", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Namibia", "Nepal", "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Korea, Democratic Republic of (North Korea)", "Norway", "Oman", "Pacific Islands", "Pakistan", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russian Federation", "Rwanda", "Saint Kitts and Nevis", "Saint Lucia", "Saint Vincent's & Grenadines", "Samoa", "Sao Tome and Principe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovak Republic (Slovakia)", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "Korea, Republic of (South Korea)", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Swaziland", "Sweden", "Switzerland", "Syria", "Tajikistan", "Tanzania", "Thailand", "Timor Leste", "Togo", "Trinidad & Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks & Caicos Islands", "Uganda", "Ukraine", "United Arab Emirates", "United States of America (USA)", "Uruguay", "Uzbekistan", "Venezuela", "Vietnam", "Virgin Islands (UK)", "Virgin Islands (US)", "Yemen", "Zambia", "Zimbabwe" });

        public MainWindow()
        {
            InitializeComponent();

            Themes = new List<string>(new[] { "Light", "Dark" });

            DataContext = this;

            IntegerProperty = new Random().Next(0, 1000);

            closableTabControl.ItemClosing += OnClosableTabControlItemClosing;

            Tags = new ObservableCollection<string>(new[] { "New York", "Los Angeles", "Seattle", "San Francisco", "Belgrade" });

            CreateGuidedTour();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public bool ShowTabControlBorder
        {
            get { return _showTabControlBorder; }
            set
            {
                _showTabControlBorder = value;

                tabControl.BorderThickness = new Thickness(value ? 1 : 0);
                tabControl.Padding = new Thickness(value ? 5 : 0);

                OnPropertyChanged("ShowTabControlBorder");
            }
        }

        public List<User> Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new List<User>();

                    for (int i = 1; i <= 20; i++)
                    {
                        _users.Add(new User()
                        {
                            FirstName = "User",
                            LastName = "User " + i,
                            DateOfBirth = DateTime.Today.Date,
                            Email = string.Format("user.{0}@email.com", i)
                        });
                    }
                }

                return _users;
            }
        }

        public List<string> Countries
        {
            get
            {
                return countries;
            }
        }

        public decimal DecimalProperty
        {
            get
            {
                return _decimalProperty;
            }
            set
            {
                _decimalProperty = value; OnPropertyChanged("DecimalProperty");
            }
        }

        public ObservableCollection<string> Tags { get; private set; }

        public string SelectedTag
        {
            get
            {
                return _selectedTag;
            }
            set
            {
                _selectedTag = value;

                OnPropertyChanged("SelectedTag");
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");

                OnPropertyChanged("SelectedCountries");
            }
        }

        public IEnumerable<string> SelectedCountries
        {
            get
            {
                if (!string.IsNullOrEmpty(SearchText))
                {
                    foreach (var country in Countries.Where(x => x.ToUpper().Contains(SearchText.ToUpper())))
                    {
                        yield return country;
                    }
                }
            }
        }

        private void OnCalendarPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void OnAddTabItem(object sender, RoutedEventArgs e)
        {
            var item = new ClosableTabItem() { Header = "Added item" };
            closableTabControl.Items.Add(item);
        }

        private void OnClosableTabControlItemClosing(object sender, ClosableItemEventArgs e)
        {
            e.Cancel = MessageBox.Show($"Close item {e.Item.Header.ToString()}?", "Confirm close", MessageBoxButton.YesNo) != MessageBoxResult.Yes;
        }

        private void ShowFlyout(object sender, RoutedEventArgs e)
        {
            flyout.IsOpen = true;
        }

        private void OnDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1);
        }

        #region - Guided Tour -

        private void CreateGuidedTour()
        {
            guide.Items = new[]
            {
                new GuidedTourItem() {Target = tabItemGuidedTour, Content = "Click to see guided tour in action", Placement = GuidedTourItem.ItemPlacement.Right, Title = "Start guided tour" },
                new GuidedTourItem() {Target = guideElement1, AlternateTargets = new[] { altGuideElement1 }, Content = "Click the button to move to next guide...", Placement = GuidedTourItem.ItemPlacement.Right , Title = "Click first item"},
                new GuidedTourItem() {Target = guideElement2, Content = "Write some text to this text box to move to next guide...", Title="Text box guide", Placement = GuidedTourItem.ItemPlacement.Left},
                new GuidedTourItem() {Target = guideElement3, Content = "Click the button to move to next guide...", Title = "Guide item title", Placement = GuidedTourItem.ItemPlacement.Right },
                new GuidedTourItem() {Target = guideElement4, Content = "Click the button to complete the tour", Title = "Last element", Placement = GuidedTourItem.ItemPlacement.Top}
            };
        }

        private void OnResetGuide(object sender, RoutedEventArgs e)
        {
            guide.Reset();
        }

        private void OnGuidedTourClosed(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Guided tour closed!");
        }

        private void OnGuidedTourFinished(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Guided tour finished!");
        }

        #endregion

        private void OnClosingGuidedTour(object sender, RoutedEventArgs e)
        {
            ((ClosingGuidedTourEventArgs)e).Cancel = MessageBox.Show("Are your sure you wish to dismiss this tour?", "Closing tour", MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes;
        }

        private void OnLiveSearchItemSelected(object sender, LiveSearchItemSelectedEventArgs e)
        {
            MessageBox.Show($"{SearchText}: {e.Item}");
        }
    }
}
