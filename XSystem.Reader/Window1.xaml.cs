using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FORCEBuild.UI.WPF;
using Microsoft.Win32;
using XSystem.Reader.Annotations;

namespace XSystem.Reader
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            this.DataContext = new Window1ViewModel();
        }
    }


    public class Window1ViewModel:INotifyPropertyChanged
    {
        private string _content;
        private string _regexString;
        private MatchCollection _matchCollection;
        private string _firstMatch;

        public ICommand OpenTxtCommand => new AnotherCommandImplementation((o => {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "txt|*.txt"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try {
                    var streamReader = new StreamReader(openFileDialog.FileName, Encoding.Unicode);
                    Content = streamReader.ReadToEnd();
                }
                catch (Exception e) {
                    MessageBox.Show(e.Message);
                }
            }
        }));

        public ICommand CalculateCommand => new AnotherCommandImplementation((o => {
            try {
                var regex = new Regex(RegexString);
                MatchCollection = regex.Matches(Content);
                FirstMatch = MatchCollection[0].Groups[1].Value;
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }));

        public ICommand ClearCommand => new AnotherCommandImplementation((o => {
            Content = null;
        }));

        public string FirstMatch {
            get { return _firstMatch; }
            set {
                if (value == _firstMatch) return;
                _firstMatch = value;
                OnPropertyChanged();
            }
        }

        public string Content {
            get { return _content; }
            set {
                if (value == _content) return;
                _content = value;
                OnPropertyChanged();
            }
        }

        public string RegexString {
            get { return _regexString; }
            set {
                if (value == _regexString) return;
                _regexString = value;
                OnPropertyChanged();
            }
        }

        public MatchCollection MatchCollection {
            get { return _matchCollection; }
            set {
                if (Equals(value, _matchCollection)) return;
                _matchCollection = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
