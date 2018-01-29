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
using Castle.Core.Internal;
using ConciseDesign.WPF.UserControls;
using FORCEBuild.Helper;
using FORCEBuild.UI.WPF;
using MahApps.Metro.Controls.Dialogs;
using TeachManagement.PresentationLayer.UserControls;
using XSystem.Core.Domain;
using XSystem.Reader.Annotations;

namespace XSystem.Reader
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public WrapPanel FilmsView => new Lazy<WrapPanel>(() => ControlRegister.GetById("FilmsView") as WrapPanel)
            .Value;

        public DialogHostControl DialogHost => new Lazy<DialogHostControl>(() =>
            DialogRegister.GetById("ViewHost")).Value;

        private readonly FilmService _service;

        private ObservableCollection<Model> _bindingPageModels;

        private int _maxResultModelsCount = 0;

        /// <summary>
        /// 当前显示结果（单页）
        /// </summary>
        public ObservableCollection<Model> BindingPageModels {
            get { return _bindingPageModels; }
            set {
                if (Equals(value, _bindingPageModels)) return;
                _bindingPageModels = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<Model> _searchResultQueryable;

        /// <summary>
        /// 二次查询结果（用于搜索/翻页）
        /// </summary>
        public IEnumerable<Model> SearchResultQueryable {
            get { return _searchResultQueryable; }
            set {
                _searchResultQueryable = value;
                handle = 0;
                LoadPage();
            }
        }

        private IQueryable<Model> _previewQueryable;

        /// <summary>
        /// 当前可查询集合
        /// </summary>
        public IQueryable<Model> PreviewQueryable {
            get { return _previewQueryable; }
            set {
                _previewQueryable = value;
                var models = value.ToArray();
                _maxResultModelsCount = models.Length;
                SearchResultQueryable = models;
            }
        }

        public Model ParentModel {
            get { return _parentModel; }
            set {
                if (Equals(value, _parentModel)) return;
                _parentModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AttachMemberCommand));
                //OnPropertyChanged(nameof(NewMemberCommand));
                OnPropertyChanged(nameof(DeleteCommand));
            }
        }

        public Type PreType {
            get { return _preType; }
            set {
                if (Equals(value, _preType)) return;
                _preType = value;
                OnPropertyChanged();
                //OnPropertyChanged(nameof(NewMemberCommand));
                OnPropertyChanged(nameof(DeleteCommand));
            }
        }

        private readonly Type _actorType = typeof(Actor);

        private readonly Type _filmType = typeof(Film);

        private readonly Type _publisherType = typeof(Publisher);

        private readonly Type _seriesType = typeof(Series);
        private Model _parentModel;
        private Type _preType;

        //private System.Collections.Concurrent.ObjectPool<FilmItem> filmItems=new System.Collections.Concurrent.ObjectPool<FilmItem>();

        private Stack items = new Stack();

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
            //BindingModels = new ObservableCollection<Model> {
            //    film
            //};
            for (var i = 0; i < PageModelCount; i++) {
                items.Push(new FilmItem());
            }
        }

        private int handle = 0;

        public int PageModelCount { get; set; } = 40;

        private void LoadPage()
        {
            BindingPageModels = new ObservableCollection<Model>(SearchResultQueryable
                .Skip(handle)
                .Take(PageModelCount));
            handle += BindingPageModels.Count;
            //            var models = SearchResultQueryable
            //                .Skip(this.handle)
            //                .Take(PageModelCount)
            //                .ToArray();
            //            int handle = 0;
            //            foreach (FilmItem child in FilmsView.Children) {
            //                child.DataContext = models[handle];
            //                ++handle;
            //            }
            //            for (int i = handle; i < models.Length; i++) {
            //                var model = models[i];
            //                var filmItem = (FilmItem) items.Pop();
            //                filmItem.DataContext = model;
            //                FilmsView.Children.Add(filmItem);
            //            }
            //            this.handle += FilmsView.Children.Count;
        }

        public ICommand PageNextCommand => new AnotherCommandImplementation(o => { LoadPage(); },
            o => handle < _maxResultModelsCount);

        public ICommand PageBackwardCommand => new AnotherCommandImplementation(o => {
            handle = handle < 2 * PageModelCount ? 0 : handle - 2 * PageModelCount;
            LoadPage();
        }, (o => handle > PageModelCount));

        public ICommand UseFamousCommand => new AnotherCommandImplementation((o => {
            
        }));

        public ICommand AllMembersCommand => new AnotherCommandImplementation(o => {
            if (o == null) {
                return;
            }
            switch (o.ToString()) {
                case "Actor":
                    PreviewQueryable = _service.AllActors;
                    break;
                case "Publisher":
                    PreviewQueryable = _service.AllPublishers;
                    break;
                case "Film":
                    PreviewQueryable = _service.AllFilms;
                    break;
                case "Series":
                    PreviewQueryable = _service.AllSeries;
                    break;
            }
            ParentModel = null;
        });

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
                BindingPageModels.Remove(model);
            }
            catch (Exception e) {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.Message);
            }
        });

        public ICommand SearchCommand => new AnotherCommandImplementation(o => {
            if (o == null || o.ToString().IsEmpty()) {
                SearchResultQueryable = PreviewQueryable.ToArray();
            }
            else {
                SearchResultQueryable = PreviewQueryable
                    .Where(model => model.Name.Contains(o.ToString()))
                    .ToArray();
            }
        });

        public ICommand ReviewFilmCommand => new AnotherCommandImplementation(o => { });


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}