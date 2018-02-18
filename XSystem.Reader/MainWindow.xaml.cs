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

        private readonly FilmService _filmService;

        public MainWindow(MainWindowViewModel viewModel, FilmService filmService)
        {
            _viewModel = viewModel;
            this._filmService = filmService;
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private readonly Type _filmType = typeof(Film);

        private Type _publisherType = typeof(Publisher);

        private readonly Type _seriesType = typeof(Series);

        private Type _actorType = typeof(Actor);

        /// <summary>
        /// commandbinding 路由绑定,集合路径传递
        /// </summary>
        /// <param name="sender">按钮</param>
        /// <param name="e"></param>
        private void Review_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
            if (e.Parameter == null)
            {
                Executor(sender, e);
            }
            //else {
            //    Executor(sender,);
            //}
        }

        /// <summary>
        /// listview 点击item来源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            Executor(sender, e);
        }

        private FilmDetail filmDetail = new FilmDetail();

        /// <summary>
        /// 选择执行器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Executor(object sender, RoutedEventArgs e)
        {
            _viewModel.NameSearchString = null;
            var button = e.OriginalSource as FrameworkElement;
            var model = button.DataContext as Model;
            if (model is Film) {
                filmDetail.DataContext = model;
                _viewModel.DialogHost.RaiseDialog(filmDetail, null);
                return;
            }
            if (model is Actor) {
                _viewModel.PreType = _filmType;
                _viewModel.PreviewFilmsQueryable = ((Actor) model).Films.AsQueryable();
                //_filmService.FilmsSearch(film => film.Actors.Contains(model));

            }
            else if (model is Series) {
                _viewModel.PreType = _filmType;
                _viewModel.PreviewFilmsQueryable = _filmService.FilmsSearch(film => film.Series.Name == model.Name);
                
            }
            else if (model is Publisher) {
                var publisher = model as Publisher;
                _viewModel.PreType = _seriesType;
                _viewModel.PreviewSeriesQueryable = publisher.Series.AsQueryable();
            }
            //最后才确认
            _viewModel.ParentModel = model;
        }
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