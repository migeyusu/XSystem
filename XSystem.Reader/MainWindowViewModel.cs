using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class MainWindowViewModel:INotifyPropertyChanged
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public DialogHostControl DialogHost => new Lazy<DialogHostControl>(() => DialogRegister.GetById("ViewHost")).Value;
        private readonly FilmService _service;
        private ObservableCollection<Model> _bindingModels;
        private ObservableCollection<Model> _previewModels;

        public ObservableCollection<Model> BindingModels {
            get { return _bindingModels; }
            set {
                if (Equals(value, _bindingModels)) return;
                _bindingModels = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Model> PreviewModels {
            get { return _previewModels; }
            set {
                _previewModels = value;
                BindingModels = value;
            }
        }

        public Model ParentModel {
            get { return _parentModel; }
            set {
                if (Equals(value, _parentModel)) return;
                _parentModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AttachMemberCommand));
                OnPropertyChanged(nameof(NewMemberCommand));
                OnPropertyChanged(nameof(DeleteCommand));
            }
        }

        public Type PreType {
            get { return _preType; }
            set {
                if (Equals(value, _preType)) return;
                _preType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NewMemberCommand));
                OnPropertyChanged(nameof(DeleteCommand));
            }
        }

        private readonly Type _actorType = typeof(Actor);

        private readonly Type _filmType = typeof(Film);

        private readonly Type _publisherType = typeof(Publisher);

        private readonly Type _seriesType = typeof(Series);
        private Model _parentModel;
        private Type _preType;

        public MainWindowViewModel(FilmService service)
        {
            _dialogCoordinator = DialogCoordinator.Instance;
            _service = service;
            var film = new Film() { Name = "wert./xznlj" };
            var film2 = new Film() { Name = "wert./xzn2312lj" };
            var actor = new Actor() { Name = "沃尔特为人类" };
            var publisher = new Publisher() { Name = "w4ern" };
            var series = new Series() { Name = "dfssdowe" };
            _service.AddSeries(new []{series});
            _service.AddActors(new []{actor});
            _service.AddFilms(new []{film,film2});
            _service.AddPublishers(new []{publisher});
            actor.Films.Add(film);
            actor.Films.Add(film2);
            film2.Actors.Add(actor);
            film.Actors.Add(actor);
            series.Films.Add(film);
            film.Series = series;
            publisher.Series.Add(series);
            BindingModels = new ObservableCollection<Model> {
                film
            };
        }

        public ICommand AllMembersCommand => new AnotherCommandImplementation((o => {
            if (o == null) {
                return;
            }
            switch (o.ToString()) {
                case "Actor":
                    PreviewModels = new ObservableCollection<Model>(_service.AllActors);
                    break;
                case "Publisher":
                    PreviewModels = new ObservableCollection<Model>(_service.AllPublishers);
                    break;
                case "Film":
                    PreviewModels = new ObservableCollection<Model>(_service.AllFilms);
                    break;
                case "Series":
                    PreviewModels = new ObservableCollection<Model>(_service.AllSeries);
                    break;
            }
            ParentModel = null;
        }));

        public ICommand AttachMemberCommand => new AnotherCommandImplementation((o => {
            
        }), (o => ParentModel != null));

        public ICommand NewMemberCommand => new AnotherCommandImplementation((async o => {
            try {
                Model model = null;
                var newNameDialog = new NewNameDialog() {
                    Introduction = "输入名称"
                };
                if (await DialogHost.RaiseDialogAsync(newNameDialog)) {
                    if (ParentModel == null)
                    {
                        if (PreType == _actorType)
                        {
                            var actor = new Actor()
                            {
                                Name = newNameDialog.Result
                            };
                            _service.AddActors(new[] { actor });
                            model = actor;
                            
                        }
                        else if (PreType == _filmType)
                        {
                            var film = new Film()
                            {
                                Name = newNameDialog.Result
                            };
                            _service.AddFilms(new[] { film });
                            model = film;
                        }
                        else if (PreType == _publisherType)
                        {
                            var publisher = new Publisher()
                            {
                                Name = newNameDialog.Result
                            };
                            _service.AddPublishers(new[] { publisher });
                            model = publisher;
                        }
                        else if (PreType == _seriesType)
                        {
                            var series = new Series()
                            {
                                Name = newNameDialog.Result
                            };
                            _service.AddSeries(new[] { series });
                            model = series;
                        }
                    }
                    else
                    {
                        if (ParentModel.GetType()==_actorType) {
                            
                        }
                        if (PreType == _actorType)
                        {
                            var actor = new Actor()
                            {
                                Name = newNameDialog.Result
                            };
                            _service.AddActors(new[] { actor });
                         
                        }
                        else if (PreType == _filmType)
                        {
                            var film = new Film()
                            {
                                Name = newNameDialog.Result
                            };
                            _service.AddFilms(new[] { film });
                            
                        }
                        else if (PreType == _publisherType)
                        {
                            var publisher = new Publisher()
                            {
                                Name = newNameDialog.Result
                            };
                            _service.AddPublishers(new[] { publisher });
                           
                        }
                        else if (PreType == _seriesType)
                        {
                            var series = new Series()
                            {
                                Name = newNameDialog.Result
                            };
                            _service.AddSeries(new[] { series });
                            
                        }
                    }
                    PreviewModels.Add(model);
                }
            }
            catch (Exception e) {
                await _dialogCoordinator.ShowMessageAsync(this, "错误", e.Message);
            }
        }));

        public ICommand DeleteCommand => new AnotherCommandImplementation((async o => {
            try {
                var model = o as Model;
                if (ParentModel==null) {
                    if (PreType==_actorType) {
                        _service.RemoveActors(new []{(Actor)model});
                    }else if (PreType==_publisherType) {
                        _service.RemovePublishers(new[] { (Publisher)model });
                    }
                    else if (PreType == _filmType) {
                        _service.RemoveFilms(new[] {(Film) model});
                    }
                    else if(PreType==_seriesType){
                        _service.RemoveSeries(new[] { (Series)model });
                    }
                }
                else {
                    
                }
                PreviewModels.Remove(model);
            }
            catch (Exception e) {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.Message);
            }
            
            
        }));

        public ICommand SearchCommand => new AnotherCommandImplementation((o => {
            if (o == null || o.ToString().IsEmpty()) {
                BindingModels = PreviewModels;
            }
            else {
                BindingModels = PreviewModels.Where(model => model.Name.Contains(o.ToString()))
                    .ToObservableCollection();
            }
        }));

        public ICommand ReviewFilmCommand => new AnotherCommandImplementation((o => {
            
        }));



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}