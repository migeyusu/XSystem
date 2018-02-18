using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Win32;
using XSystem.Core.Domain;

namespace XSystem.Reader
{
    //创建时间:2018/2/4 16:46:57
    //创建者: 98197 
    //CLR版本:4.0.30319.42000
    //创作机器:DESKTOP-PHQOQCK

    /// <summary>
    /// FilmDetail.xaml 的交互逻辑
    /// </summary>
    public partial class FilmDetail : UserControl
    {
        public FilmDetail()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnClick(object sender, RequestNavigateEventArgs e)
        {
            //Process.Start(((Film)((FrameworkElement)sender).DataContext).SourceUrl);
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
