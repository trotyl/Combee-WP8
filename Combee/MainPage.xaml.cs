using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BindingData.Model;
using BindingData.ViewModel;
using Microsoft.Phone.Tasks;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace Combee
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        { 
            InitializeComponent();
    
            this.DataContext = App.NewViewModel;
        }

        // 程序退出确认(后退键重载)
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            MessageBoxResult result = MessageBox.Show("您确定要狠心地退出Combee? T^T", "退出确认", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        // 判断是否已登录(导航至重载)
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            // 判断当前是否含有token
            if (!CurrentUser.IsLogin())
            {
                NavigationService.Navigate(new Uri("/Combee;component/LoginPage.xaml", UriKind.Relative));
            }
            else
            {
                if (CurrentUser.IsFirst())
                    Network.GetAsyncAll();

                // 首次进入程序导航选项
                if (CurrentUser.IsFirst())
                {
                    CurrentUser.SetCount(CurrentUser.GetCount() + 1);
                    MessageBoxResult res = MessageBox.Show("欢迎使用Combee，是否进入新功能介绍？", "首次登录提示", MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        NavigationService.Navigate(new Uri("/Combee;component/IntroductionPage.xaml", UriKind.Relative));
                    }
                }

            }

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ReceiptStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/ReceiptPage.xaml?id="+((StackPanel)sender).Tag.ToString(), UriKind.Relative));
        }

        private void LikeMenu_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void AboutMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/AboutPage.xaml", UriKind.Relative));
        }

         private void SettingMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/SettingPage.xaml", UriKind.Relative));
        }

        private void NewConversationButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/ConversationPage.xaml", UriKind.Relative));            
        }

       private void UserImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string id = ((Image)sender).Tag.ToString();
            NavigationService.Navigate(new Uri("/Combee;component/UserPage.xaml?id=" + id, UriKind.Relative));

        }

        private void ToastButton_Click(object sender, EventArgs e)
        {
            ShellToast toast = new ShellToast();
            toast.Title = "Toast测试";
            toast.Content = "来自 张建奇 的私信。";
            toast.Show();
        }

        private void SetButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (btn.Text == "私信")
            {
                btn.IconUri = new Uri("/Assets/AppBar/circle.png", UriKind.Relative);
                btn.Text = "未读私信";
            }
            else
            {
                btn.IconUri = new Uri("/Assets/AppBar/favs.png", UriKind.Relative);
                btn.Text = "私信";
            }
        }

        private void RenewButton_Click(object sender, EventArgs e)
        {
            Network.GetAsyncAll();
        }

        private void NewPostButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/NewPostPage.xaml", UriKind.Relative));
        }
        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/OrganizationPage.xaml?id=" + ((StackPanel)sender).Tag.ToString(), UriKind.Relative));
        }

        private void MainPagePanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MainPagePanorama.SelectedIndex)
            {
                case 0:
                    ApplicationBar = ((ApplicationBar)this.Resources["RptAppBar"]);
                    break;
                case 1:
                    ApplicationBar = ((ApplicationBar)this.Resources["OrgzAppBar"]);
                    break;
                case 2:
                    ApplicationBar = ((ApplicationBar)this.Resources["CovAppBar"]);
                    break;
            }
        }

    }

}