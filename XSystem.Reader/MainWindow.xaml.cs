using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MahApps.Metro.Controls;
using XSystem.Core.Domain;

namespace XSystem.Reader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void Review_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var button = e.OriginalSource as FrameworkElement;
            _viewModel.ParentModel = button.DataContext as Model;
            var eCommand = (IEnumerable<Model>) e.Parameter;
            _viewModel.PreviewModels = new ObservableCollection<Model>(eCommand);
            _viewModel.PreType = e.Parameter.GetType().GenericTypeArguments[0];
        }

        private void Edit_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
        }
    }
}
