using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using XSystem.Core.Annotations;

namespace XSystem.Core.Domain
{
    public class Model:INotifyPropertyChanged
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        private RecommendLevel _recommendLevel;
        /// <summary>
        /// 推荐级别
        /// </summary>
        public RecommendLevel RecommendLevel {
            get { return _recommendLevel; }
            set {
                if (value == _recommendLevel) return;
                _recommendLevel = value;
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