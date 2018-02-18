using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Castle.Core.Internal;
using ConciseDesign.WPF.UserControls;
using FORCEBuild.Helper;
using FORCEBuild.UI.WPF;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using TeachManagement.PresentationLayer.UserControls;
using XSystem.Core.Domain;
using XSystem.Reader.Annotations;
using Page = Reptile.Page;

namespace XSystem.Reader
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public WrapPanel FilmsView => new Lazy<WrapPanel>(
                () => ControlRegister.GetById("FilmsView") as WrapPanel)
            .Value;

        public DialogHostControl DialogHost => new Lazy<DialogHostControl>(() =>
            DialogRegister.GetById("ViewHost")).Value;

        private readonly FilmService _service;

        public Snackbar Snackbar { get; set; } =
            new Snackbar() {
                MessageQueue = new SnackbarMessageQueue(),
                Background = Brushes.Black,
                Foreground = Brushes.White
            };

        public Model SelectModel { get; set; }

        #region view

        /// <summary>
        /// 查询对象创建
        /// </summary>
        private void ModelQueryableCreate()
        {
            if (PreviewActorsQueryable == null && PreviewFilmsQueryable == null &&
                PreviewPublishersQueryable == null && PreviewSeriesQueryable == null)
                return;
            if (BindingPageModels != null) {
                //存放上次页结果
                viewPageCaches.Push(new ViewPageCache {
                    ResultModelsCount = _maxResultModelsCount,
                    Handle = _handle,
                    SearchString = NameSearchString,
                    IsFamousDesc = IsOrderByFamous,
                    ShowType = PreType,
                    ParentModel = ParentModel,
                    BindingPageModels = BindingPageModels,
                    Queryable = PageQueryable
                });
            }
            if (PreType == _filmType) {
                var models = PreviewFilmsQueryable;
                if (!NameSearchString.IsNullOrEmpty()) {
                    models = models.Where(model => model.Name.Contains(NameSearchString));
                }
                if (!CodeSearchString.IsNullOrEmpty()) {
                    models = models.Where(model => model.Code.Contains(CodeSearchString));
                }
                if (IsShowSingleActor) {
                    models = models.Where(model => model.IsOneActor);
                }
                models = IsOrderByFamous
                    ? models.OrderByDescending(model => model.RecommendLevel)
                    : models.OrderBy(model => model.Id);
                PageQueryable = models;
            }
            else if (PreType == _actorType) {
                var models = PreviewActorsQueryable;
                if (!NameSearchString.IsNullOrEmpty()) {
                    models = models.Where(model => model.Name.Contains(NameSearchString));
                }
                models = IsOrderByFamous
                    ? models.OrderByDescending(model => model.RecommendLevel)
                    : models.OrderBy(model => model.Id);
                PageQueryable = models;
            }
            else if (PreType == _seriesType) {
                var models = PreviewSeriesQueryable;
                if (!NameSearchString.IsNullOrEmpty()) {
                    models = models.Where(model => model.Name.Contains(NameSearchString));
                }
                models = IsOrderByFamous
                    ? models.OrderByDescending(model => model.RecommendLevel)
                    : models.OrderBy(model => model.Id);
                PageQueryable = models;
            }
            else if (PreType == _publisherType) {
                var models = PreviewPublishersQueryable;
                if (!NameSearchString.IsNullOrEmpty()) {
                    models = models.Where(model => model.Name.Contains(NameSearchString));
                }
                models = IsOrderByFamous
                    ? models.OrderByDescending(model => model.RecommendLevel)
                    : models.OrderBy(model => model.Id);
                PageQueryable = models;
            }
        }

        private IEnumerable<Model> _bindingPageModels;

        private int _maxResultModelsCount = 0;

        /* 查询实现：
         * 一是每个控件影响一个条件因子，利用Queryable的Expression Tree 动态查询，条件变化后实现查询
         * 二是对查询分层，每层持有缓存并传递结果到下一层
         * 三使用一个查询方法，所有条件都在方法内被检查，条件的变化会调用该方法，同时有利于历史数据保留
         */

        /// <summary>
        /// 当前显示结果（单页）
        /// </summary>
        public IEnumerable<Model> BindingPageModels {
            get { return _bindingPageModels; }
            set {
                if (Equals(value, _bindingPageModels)) return;
                _bindingPageModels = value;
                OnPropertyChanged();
            }
        }

        #region query condition

        public string CodeSearchString {
            get { return _codeSearchString; }
            set {
                if (value == _codeSearchString) return;
                _codeSearchString = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowSingleActor {
            get { return _isShowSingleActor; }
            set {
                if (value == _isShowSingleActor) return;
                _isShowSingleActor = value;
                OnPropertyChanged();
            }
        }

        public string NameSearchString {
            get { return _nameSearchString; }
            set {
                if (value == _nameSearchString) return;
                _nameSearchString = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否使用星级排名
        /// </summary>
        public bool IsOrderByFamous {
            get { return _isOrderByFamous; }
            set {
                if (value == _isOrderByFamous) return;
                _isOrderByFamous = value;
                OnPropertyChanged();
            }
        }

        private int _handle = 0;
        private string _nameSearchString;
        private IQueryable<Model> _pageQueryable;

        public int PageModelCount { get; set; } = 40;

        #endregion

        /// <summary>
        /// 页查询对象
        /// </summary>
        public IQueryable<Model> PageQueryable {
            get { return _pageQueryable; }
            set {
                _pageQueryable = value;
                //自动重置页指针
                if (value != null) {
                    _maxResultModelsCount = value.Count();
                }
                _handle = 0;
                LoadPage();
            }
        }

        private IQueryable<Actor> _previewActorsQueryable;

        /// <summary>
        /// 当前可查询集合
        /// </summary>
        public IQueryable<Actor> PreviewActorsQueryable {
            get { return _previewActorsQueryable; }
            set {
                _previewActorsQueryable = value;
                ModelQueryableCreate();
            }
        }

        public IQueryable<Film> PreviewFilmsQueryable {
            get { return _previewFilmsQueryable; }
            set {
                _previewFilmsQueryable = value;
                ModelQueryableCreate();
            }
        }

        public IQueryable<Series> PreviewSeriesQueryable {
            get { return _previewSeriesQueryable; }
            set {
                _previewSeriesQueryable = value;
                ModelQueryableCreate();
            }
        }

        public IQueryable<Publisher> PreviewPublishersQueryable {
            get { return _previewPublishersQueryable; }
            set {
                _previewPublishersQueryable = value;
                ModelQueryableCreate();
            }
        }

        /// <summary>
        /// 当前集合的父对象（导航属性持有者）
        /// </summary>
        public Model ParentModel {
            get { return _parentModel; }
            set {
                if (Equals(value, _parentModel)) return;
                _parentModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前显示类型
        /// </summary>
        public Type PreType {
            get { return _preType; }
            set {
                if (value == _preType) return;
                _preType = value;
                OnPropertyChanged();
            }
        }

        private readonly Type _actorType = typeof(Actor);

        private readonly Type _filmType = typeof(Film);

        private readonly Type _publisherType = typeof(Publisher);

        private readonly Type _seriesType = typeof(Series);

        private Model _parentModel;
        private Type _preType;

        //private System.Collections.Concurrent.ObjectPool<FilmItem> filmItems=new System.Collections.Concurrent.ObjectPool<FilmItem>();

        public MainWindowViewModel(FilmService service)
        {
            _dialogCoordinator = DialogCoordinator.Instance;
            _service = service;
            //var film = new Film() { Name = "wert./xznlj" };
            //var film2 = new Film() { Name = "wert./xzn2312lj" };
            //var actor = new Actor() { Name = "沃尔特为人类" };
            //var publisher = new Publisher() { Name = "w4ern" };
            //var series = new Series() { Name = "dfssdowe" };
            //_service.AddSeries(new[] { series });
            //_service.AddActors(new[] { actor });
            //_service.AddFilms(new[] { film, film2 });
            //_service.AddPublishers(new[] { publisher });
            //actor.Films.Add(film);
            //actor.Films.Add(film2);
            //film2.Actors.Add(actor);
            //film.Actors.Add(actor);
            //series.Films.Add(film);
            //film.Series = series;
            //publisher.Series.Add(series);
        }

        /// <summary>
        /// 加载页面
        /// </summary>
        private void LoadPage()
        {
            BindingPageModels = PageQueryable
                .Skip(_handle)
                .Take(PageModelCount)
                .ToArray();
            _handle += BindingPageModels.Count();
        }

        public ICommand PageNextCommand => new AnotherCommandImplementation(o => { LoadPage(); },
            o => _handle < _maxResultModelsCount);

        public ICommand PageBackwardCommand => new AnotherCommandImplementation(o => {
            _handle = _handle < 2 * PageModelCount ? 0 : _handle - 2 * PageModelCount;
            LoadPage();
        }, o => _handle > PageModelCount);

        #region history view

        private Stack<ViewPageCache> viewPageCaches = new Stack<ViewPageCache>();
        private bool _isOrderByFamous;
        private string _codeSearchString;
        private bool _isShowSingleActor;
        private IQueryable<Film> _previewFilmsQueryable;
        private IQueryable<Series> _previewSeriesQueryable;
        private IQueryable<Publisher> _previewPublishersQueryable;

        public ICommand PreviousHistoryCommand => new AnotherCommandImplementation((o => {
            var viewPageCache = viewPageCaches.Pop();
            if (viewPageCache != null) {
                this._maxResultModelsCount = viewPageCache.ResultModelsCount;
                _handle = viewPageCache.Handle;
                NameSearchString = viewPageCache.SearchString;
                IsOrderByFamous = viewPageCache.IsFamousDesc;
                PreType = viewPageCache.ShowType;
                ParentModel = viewPageCache.ParentModel;
                this._pageQueryable = viewPageCache.Queryable;
                BindingPageModels = viewPageCache.BindingPageModels;
            }
        }), (o => viewPageCaches.Count > 0));

        #endregion

        public ICommand AllMembersCommand => new AnotherCommandImplementation(o => {
            if (o == null) {
                return;
            }
            //重置查询条件
            NameSearchString = null;
            ParentModel = null;
            switch (o.ToString()) {
                case "Actor":
                    PreType = _actorType;
                    PreviewActorsQueryable = _service.AllActors;
                    break;
                case "Publisher":
                    PreType = _publisherType;
                    PreviewPublishersQueryable = _service.AllPublishers;
                    break;
                case "Film":
                    PreType = _filmType;
                    PreviewFilmsQueryable = _service.AllFilms;
                    break;
                case "Series":
                    PreType = _seriesType;
                    PreviewSeriesQueryable = _service.AllSeries;

                    break;
            }
        });

        /// <summary>
        /// 刷新查询条件
        /// </summary>
        public ICommand FlashCommand => new AnotherCommandImplementation((o => { ModelQueryableCreate(); }));

        #endregion

        public ICommand AttachMemberCommand => new AnotherCommandImplementation(o => { }, o => ParentModel != null);

        //public ICommand NewMemberCommand => new AnotherCommandImplementation((async o => {
        //    try {
        //        Model model = null;
        //        var newNameDialog = new NewNameDialog() {
        //            Introduction = "输入名称"
        //        };
        //        if (await DialogHost.RaiseDialogAsync(newNameDialog)) {
        //            if (ParentModel == null)
        //            {
        //                if (PreType == _actorType)
        //                {
        //                    var actor = new Actor()
        //                    {
        //                        Name = newNameDialog.Result
        //                    };
        //                    _service.AddActors(new[] { actor });
        //                    model = actor;

        //                }
        //                else if (PreType == _filmType)
        //                {
        //                    var film = new Film()
        //                    {
        //                        Name = newNameDialog.Result
        //                    };
        //                    _service.AddFilms(new[] { film });
        //                    model = film;
        //                }
        //                else if (PreType == _publisherType)
        //                {
        //                    var publisher = new Publisher()
        //                    {
        //                        Name = newNameDialog.Result
        //                    };
        //                    _service.AddPublishers(new[] { publisher });
        //                    model = publisher;
        //                }
        //                else if (PreType == _seriesType)
        //                {
        //                    var series = new Series()
        //                    {
        //                        Name = newNameDialog.Result
        //                    };
        //                    _service.AddSeries(new[] { series });
        //                    model = series;
        //                }
        //            }
        //            else
        //            {
        //                if (ParentModel.GetType()==_actorType) {

        //                }
        //                if (PreType == _actorType)
        //                {
        //                    var actor = new Actor()
        //                    {
        //                        Name = newNameDialog.Result
        //                    };
        //                    _service.AddActors(new[] { actor });

        //                }
        //                else if (PreType == _filmType)
        //                {
        //                    var film = new Film()
        //                    {
        //                        Name = newNameDialog.Result
        //                    };
        //                    _service.AddFilms(new[] { film });

        //                }
        //                else if (PreType == _publisherType)
        //                {
        //                    var publisher = new Publisher()
        //                    {
        //                        Name = newNameDialog.Result
        //                    };
        //                    _service.AddPublishers(new[] { publisher });

        //                }
        //                else if (PreType == _seriesType)
        //                {
        //                    var series = new Series()
        //                    {
        //                        Name = newNameDialog.Result
        //                    };
        //                    _service.AddSeries(new[] { series });

        //                }
        //            }
        //            PreviewModels.Add(model);
        //        }
        //    }
        //    catch (Exception e) {
        //        await _dialogCoordinator.ShowMessageAsync(this, "错误", e.Message);
        //    }
        //}));

        public ICommand DeleteCommand => new AnotherCommandImplementation(async o => {
            try {
                var model = o as Model;
                if (ParentModel == null) {
                    //if (PreType == _actorType) {
                    //    _service.RemoveActors(new[] {(Actor) model});
                    //}
                    //else if (PreType == _publisherType) {
                    //    _service.RemovePublishers(new[] {(Publisher) model});
                    //}
                    //else if (PreType == _filmType) {
                    //    _service.RemoveFilms(new[] {(Film) model});
                    //}
                    //else if (PreType == _seriesType) {
                    //    _service.RemoveSeries(new[] {(Series) model});
                    //}
                }
                else { }
                //BindingPageModels.Remove(model);
            }
            catch (Exception e) {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.Message);
            }
        });

        public ICommand ReviewFilmCommand => new AnotherCommandImplementation(o => { });

        #region film

        public ICommand AttachFileCommand => new AnotherCommandImplementation(async o => {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                try {
                    var film = SelectModel as Film;
                    film.FileLocation = openFileDialog.FileName;
                    _service.Update();
                    Snackbar.MessageQueue.Enqueue($"已设定film{film.Name}");
                }
                catch (Exception exception) {
                    await DialogHost.RaiseMessageAsync(exception.Message);
                }
            }
        }, o => SelectModel != null);

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 页面查询缓存,包括查询结果和状态
        /// </summary>
        private class ViewPageCache
        {
            public int ResultModelsCount { get; set; }

            public int Handle { get; set; }

            public string SearchString { get; set; }

            public bool IsFamousDesc { get; set; }

            public Type ShowType { get; set; }

            public Model ParentModel { get; set; }

            public IEnumerable<Model> BindingPageModels { get; set; }

            public IQueryable<Model> Queryable { get; set; }
        }

        public ICommand SaveUpdateCommand => new AnotherCommandImplementation((async o => {
            try {
                _service.Update();
                Snackbar.MessageQueue.Enqueue("已保存");
            }
            catch (Exception e) {
                await DialogHost.RaiseMessageAsync($"保存失败：{e.Message}");
            }
        }));
    }
}