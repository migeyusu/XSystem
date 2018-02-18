using System.ComponentModel;
using System.Runtime.CompilerServices;
using FORCEBuild.Properties;

namespace XSystem.Reader
{
    //创建时间:2018/2/4 16:46:57
    //创建者: 98197 
    //CLR版本:4.0.30319.42000
    //创作机器:DESKTOP-PHQOQCK

    /// <summary>
    /// FilmDetailViewModel.xaml 的ViewModel
    /// </summary>
    public class FilmDetailViewModelViewModel : INotifyPropertyChanged
    {


        #region protected

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}