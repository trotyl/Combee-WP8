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
using Microsoft.Phone.Notification;
using System.Text;

namespace Combee
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            /// Holds the push channel that is created or found.
            HttpNotificationChannel pushChannel;

            // The name of our push channel.
            string channelName = "PushChannel";

            InitializeComponent();

            // Try to find the push channel.
            pushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();

                pushChannel.BindToShellTile();


            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());
                //MessageBox.Show(String.Format("Channel Uri is {0}",
                //    pushChannel.ChannelUri.ToString()));



            }
    
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
                {
                    Network.GetAsyncAll();
                    PushService.PushServiceSoapClient pushClient = new PushService.PushServiceSoapClient();
                    pushClient.RegisterCompleted += new EventHandler<PushService.RegisterCompletedEventArgs>(RegisterCompleted);
                    pushClient.RegisterAsync(CurrentUser.GetPrivate_token(), CurrentUser.GetChannelUri(), CurrentUser.GetId());
                }

                // 首次进入程序导航选项
                if (CurrentUser.IsFirst())
                {
                    CurrentUser.SetCount(CurrentUser.GetCount() + 1);
                    //MessageBoxResult res = MessageBox.Show("欢迎使用Combee，是否进入新功能介绍？", "首次登录提示", MessageBoxButton.OKCancel);
                    //if (res == MessageBoxResult.OK)
                    //{
                    //    NavigationService.Navigate(new Uri("/Combee;component/IntroductionPage.xaml", UriKind.Relative));
                    //}
                }

            }

        }

        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
                //MessageBox.Show(String.Format("Channel Uri is {0}",
                //    e.ChannelUri.ToString()));
                CurrentUser.SetChannelUri(e.ChannelUri.ToString());
                //PushService.PushServiceSoapClient pushClient = new PushService.PushServiceSoapClient();
                //pushClient.RegisterCompleted += new EventHandler<PushService.RegisterCompletedEventArgs>(RegisterCompleted);
                //pushClient.RegisterAsync(CurrentUser.GetPrivate_token(), e.ChannelUri.ToString(), CurrentUser.GetId());
            });
        }

        private void RegisterCompleted(object sender, PushService.RegisterCompletedEventArgs e)
        {
            ;
        }

        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData))
                    );
        }

        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }

            // Display a dialog of all the fields in the toast.
            Dispatcher.BeginInvoke(() => MessageBox.Show(message.ToString()));

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
            CurrentUser.Logout();
            App.NewViewModel.myDB.CommentTable.DeleteAllOnSubmit(App.NewViewModel.myDB.CommentTable);
            App.NewViewModel.myDB.ReceiptsTable.DeleteAllOnSubmit(App.NewViewModel.myDB.ReceiptsTable);
            App.NewViewModel.myDB.OrganizationsTable.DeleteAllOnSubmit(App.NewViewModel.myDB.OrganizationsTable);
            App.NewViewModel.myDB.ConversationsTable.DeleteAllOnSubmit(App.NewViewModel.myDB.ConversationsTable);
            App.NewViewModel.myDB.UsersTable.DeleteAllOnSubmit(App.NewViewModel.myDB.UsersTable);

            App.NewViewModel.AllConversationsItems.Clear();
            App.NewViewModel.AllOrganizationsItems.Clear();
            App.NewViewModel.AllReceiptsItems.Clear();
            App.NewViewModel.AllUsersItems.Clear();
            App.NewViewModel.myDB.SubmitChanges();
            NavigationService.Navigate(new Uri("/Combee;component/LoginPage.xaml", UriKind.Relative));

            //NavigationService.Navigate(new Uri("/Combee;component/SettingPage.xaml", UriKind.Relative));
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