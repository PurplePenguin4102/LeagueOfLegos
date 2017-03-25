using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace LeagueOfLegos
{
    public class MainPageModel : INotifyPropertyChanged
    {
        private ObservableCollection<Legend> _allChars = new ObservableCollection<Legend>();
        private Dictionary<string, ChampData> _extData;
        public ObservableCollection<Legend> characters { get; set; } = new ObservableCollection<Legend>();
        public event PropertyChangedEventHandler PropertyChanged;

        #region Full Properties
        private string _legendName;
        public string LegendName
        {
            get { return _legendName; }
            set
            {
                _legendName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LegendName)));
            }
        }


        private string _imageURI;
        public string ImageURI
        {
            get { return _imageURI; }
            set
            {
                _imageURI = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageURI)));
            }
        }

        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                FilterChars();
            }
        }

        private Legend _selectedLegend;
        public Legend SelectedLegend
        {
            get { return _selectedLegend; }
            set
            {
                if (value == null)
                    return;
                else
                {
                    _selectedLegend = value;
                    SetImage(value.Name, 0);
                    LegendName = $"Selected Legend : {value.Name}";
                }
            }
        }
        #endregion

        private async void SetImage(string name, int id)
        {
            ImageURI = (await DDragon.GetSplash(name, 0)).AbsolutePath;
        }

        public MainPageModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _allChars.Add(new Legend()
                {
                    Name = "Aatrox",
                    Title = "The darkened blade",
                });
                _allChars.Add(new Legend()
                {
                    Name = "Ahri",
                    Title = "the Nine-Tailed Fox",
                });
                FilterChars();
            }
            else
            {
                LoadData();
            }
        }

        private async void LoadData()
        {
            _extData = (await DDragon.AskTheDragonAsync()).data;
            ParseDataToChars();
            FilterChars();
        }

        private void ParseDataToChars()
        {
            foreach (var nameChamp in _extData)
            {
                _allChars.Add(new Legend()
                {
                    Name = nameChamp.Key,
                    Title = nameChamp.Value.title,
                    ThumbName = DDragon.GetThumbName(nameChamp.Value.image.full).AbsolutePath,
                    id = nameChamp.Value.id
                });

            }
        }

        private void FilterChars()
        {
            if (Filter == null)
                Filter = "";

            // apply filter
            string lowerCase = Filter.Trim().ToLower();
            IEnumerable<Legend> filteredList = _allChars.Where((legend) =>
            {
                return legend.Name.ToLower().Contains(lowerCase);
            });

            // removing bad champs
            IEnumerable<Legend> toRemove = _allChars.Except(filteredList);
            foreach (Legend legend in toRemove)
                characters.Remove(legend);

            // adding good champs
            foreach (Legend legend in filteredList)
            {
                if (!characters.Contains(legend))
                    InsertLegend(legend);
            }

            if (characters.Count() > 0)
                SelectedLegend = characters[0];
        }

        /// <summary>
        /// Inserts a Legend into the list by name
        /// </summary>
        private void InsertLegend(Legend legend)
        {
            int i;
            for (i = 0; i < characters.Count(); i++)
            {
                if (LessThanAlphabet(legend.Name, characters[i].Name))
                {
                    characters.Insert(i, legend);
                    break;
                }
            }
            if (i == characters.Count())
            {
                characters.Add(legend);
            }
        }

        /// <summary>
        /// string comparator for alphabet
        /// </summary>
        private bool LessThanAlphabet(string name1, string name2)
        {
            string name1Lower = name1.ToLower();
            string name2Lower = name2.ToLower();
            int upperBound = name1.Length < name2.Length ? name1.Length : name2.Length;
            for (int i = 0; i < upperBound; i++)
            {
                if (name1Lower[i] == name2Lower[i]) continue;
                else return (name1Lower[i] < name2Lower[i]);
            }
            return upperBound == name1.Length; // assuming "abc" < "abcd" should be true
        }
    }
}