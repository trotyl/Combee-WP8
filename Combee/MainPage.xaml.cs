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
    
            // 为观测模型设置页面的数据上下文属性.
            this.DataContext = App.NewViewModel;
        }

        // 程序退出确认(后退键重载)
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            MessageBoxResult result = MessageBox.Show("您确定要狠心地退出Combee? T^T", "退出确认", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }

        // 判断是否已登录(导航至重载)
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            // 判断当前是否含有token
            if (!settings.Contains("private_token"))
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("logined"))
                {
                    NavigationService.Navigate(new Uri("/Combee;component/Login.xaml", UriKind.Relative));
                }
            }
            else
            {
                //获取用户信息
                if (!settings.Contains("name") || !settings.Contains("id") || !settings.Contains("private_token"))
                {
                    Json.GetCurrentUser();
                }
                else
                {
                    ThisUser.name = (string)IsolatedStorageSettings.ApplicationSettings["name"];
                    ThisUser.id = (string)IsolatedStorageSettings.ApplicationSettings["id"];
                    ThisUser.private_token = (string)IsolatedStorageSettings.ApplicationSettings["private_token"];
                }

                // 首次进入程序导航选项
                if (!settings.Contains("entered"))
                {
                    Json.GetAsync("receipts", "receipts");
                    Json.GetAsync("organizations", @"users/" + ThisUser.id + @"/organizations");
                    Json.GetAsync("conversations", @"user/conversations");

                    settings.Add("entered", "1");
                    settings.Save();
                    var result = MessageBox.Show("欢迎使用Combee，是否进入新功能介绍？", "首次登录提示", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        // cancel the closure of the form.
                        NavigationService.Navigate(new Uri("/Combee;component/Introduction.xaml", UriKind.Relative));
                    }
                }

            }

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/About.xaml", UriKind.Relative));
        }

        private void fullUmsgStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/Post.xaml?id="+((StackPanel)sender).Tag.ToString(), UriKind.Relative));
        }

        private void LikeButton_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void UserImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //object ob = ((Image)sender).Parent;
            //StackPanel stp = (StackPanel)ob;
            //UmsgList.ScrollTo(stp);
            //Receipts rpt = stp.

            //PhoneApplicationService.Current.State["mode"] = "user";
            //PhoneApplicationService.Current.State["back"] = sender;

            //UmsgList.ScrollTo(PhoneApplicationService.Current.State["back"]);

            NavigationService.Navigate(new Uri("/Combee;component/Users.xaml?id=" + ((Image)sender).Tag.ToString(), UriKind.Relative));

        }

        private void ToastButton_Click(object sender, EventArgs e)
        {
            ShellToast toast = new ShellToast();
            toast.Title = "Toast测试";
            toast.Content = "来自 张建奇 的私信。";
            toast.Show();
            //NavigationService.Navigate(new Uri("/Combee;component/Test.xaml", UriKind.Relative));
        }

        private void SetButton_Click(object sender, EventArgs e)
        {
            # region 测试添加优信条目
            string nn = "1";
            if (PhoneApplicationService.Current.State.ContainsKey("nn"))
            {
                // If it exists, assign the data to the application member variable.
                nn = PhoneApplicationService.Current.State["nn"] as string;
            }

            nn = (Int32.Parse(nn) + 1).ToString();
            PhoneApplicationService.Current.State["nn"] = nn;
            # endregion

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

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/Test.xaml", UriKind.Relative));
        }

        private void RenewButton_Click(object sender, EventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains("name") || !settings.Contains("id"))
            {
                Json.GetCurrentUser();
            }
            else
            {
                Json.GetAsync("receipts", "receipts");
                Json.GetAsync("organizations", @"users/" + ThisUser.id + @"/organizations");
                Json.GetAsync("conversations", @"user/conversations");

                //App.NewViewModel.LoadCollectionsFromDatabase();
            }
        }

        public void AddReceipt(Receipts rpt)
        {

        }
    }

}