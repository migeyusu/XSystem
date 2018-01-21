using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
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
using Expression = System.Linq.Expressions.Expression;

namespace XSystem.Reader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel _viewModel;

        private FilmService _filmService;

        public MainWindow(MainWindowViewModel viewModel, FilmService filmService)
        {
            _viewModel = viewModel;
            this._filmService = filmService;
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private Type filmType = typeof(Film);

        private Type publisherType = typeof(Publisher);

        private Type seriesType = typeof(Series);

        private Type actorType = typeof(Actor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">按钮</param>
        /// <param name="e">传入的查询字符串（不管集合大小）</param>
        private void Review_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var button = e.OriginalSource as FrameworkElement;
            var model = button.DataContext as Model;
            _viewModel.ParentModel = model;
            var searchstring = e.Parameter.ToString();
            if (model is Actor) {
                _viewModel.PreviewQueryable = ((Actor) model).Films.AsQueryable();
                //_filmService.FilmsSearch(film => film.Actors.Contains(model));
                _viewModel.PreType = filmType;
            }
            else if (model is Series) {
                _viewModel.PreviewQueryable = _filmService.FilmsSearch(film => film.Series.Name == model.Name);
                _viewModel.PreType = filmType;
            }
            else if (model is Publisher) {
                var publisher = model as Publisher;
                _viewModel.PreviewQueryable = publisher.Series.AsQueryable();
                _viewModel.PreType = seriesType;
            }
        }

        /// <summary>
        /// 查看详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_OnExecuted(object sender, ExecutedRoutedEventArgs e) { }
    }


    /* 目的是为了让IEnumerable能够被当成IQueryable，后发现只需要调用asquery
     */ 
    ///// <summary>
    ///// 允许
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class DefaultQuery<T>:IQueryable<T>
    //{
    //    private readonly IEnumerable<T> _enumerable;

    //    public DefaultQuery(IEnumerable<T> enumerable)
    //    {
    //        this._enumerable = enumerable;
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return _enumerable.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    public Expression Expression { get; }
    //    public Type ElementType { get; }
    //    public IQueryProvider Provider { get; }
    //}

    //public class DefaultProvider
    //{
        
    //}
    
}