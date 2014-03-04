using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Net;

namespace Combee
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            NavigationService.RemoveBackEntry();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Storage.Initials();
            
            //UserNameTextBox.Focus();
        }

        private void UserNameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PasswordTextBox.Focus();
            }
        }

        private void PasswordTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/HelpPage.xaml", UriKind.Relative));
        }

        private void WebSiteButton_Click(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri(@"https://combee.co/", UriKind.Absolute);

            webBrowserTask.Show();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (UserNameTextBox.Text == string.Empty)
            {
                MessageBox.Show("用户名不能为空!", "温馨提示", MessageBoxButton.OK);
            }
            else if (PasswordTextBox.Password == string.Empty)
            {
                MessageBox.Show("密码不能为空!", "温馨提示", MessageBoxButton.OK);
            }
            else
            {
                //禁止输入更改
                UserNameTextBox.IsEnabled = false;
                PasswordTextBox.IsEnabled = false;

                //尝试登录以获取用户信息
                WebClient client = new WebClient();
                client.UploadStringCompleted += new UploadStringCompletedEventHandler(GetSession);
                //Uri uri = UriString.GetLoginUri(UserNameTextBox.Text, PasswordTextBox.Password);
                //MessageBox.Show(arg.ToString());
                PostArgs arg = new PostArgs();
                arg["login"] = UserNameTextBox.Text;
                arg["password"] = PasswordTextBox.Password;
                Uri uri = new Uri(UriString.host + "session.json?" + arg.ToString());

                client.UploadStringAsync(uri, "");
            }

        }

        private void GetSession(object sender, UploadStringCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "登录失败!", MessageBoxButton.OK);
                UserNameTextBox.IsEnabled = true;
                PasswordTextBox.IsEnabled = true;
                //JObject o = JObject.Parse("{\"id\":\"5296ec473df68e4148000058\",\"name\":\"余泽江\",\"email\":\"yzj1995@vip.qq.com\",\"created_at\":\"2013-11-28T15:09:59+08:00\",\"avatar\":\"/uploads/avatar/user/5296ec473df68e4148000058_0f63a2d613062e59871b3a22836ef4d3.jpg\",\"phone\":\"15528258522\",\"private_token\":\"FHs5LTD4MpEfrxYEfHnT\"}");
                //CurrentUser.Login(o);
                //NavigationService.GoBack();
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                CurrentUser.Login(o);

                NavigationService.GoBack();
            }
        }

        private void RegisterBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "申请试用 Combee";
            emailComposeTask.Body = "请留下您的姓名，联系电话，学校或公司名称。我们将尽快联系您，为您开通服务。感谢您的支持！";
            emailComposeTask.To = "support@combee.co";
            emailComposeTask.Cc = "trotyl@qq.com";

            emailComposeTask.Show();

        }

        private void LostBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri(@"https://combee.co/account/password/new", UriKind.Absolute);

            webBrowserTask.Show();

        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Opacity = 1;
            TempTextBox.Opacity = 0;
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(PasswordTextBox.Password == string.Empty)
            {
                TempTextBox.Opacity = 1;
                PasswordTextBox.Opacity = 0;
            }
        }

        private void UserNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text == "电子邮件/电话")
                UserNameTextBox.Text = string.Empty;
        }

        private void UserNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text == string.Empty)
                UserNameTextBox.Text = "电子邮件/电话";
        }


    }
}