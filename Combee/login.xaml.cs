using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace Combee
{
    public partial class login : PhoneApplicationPage
    {
        public login()
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
            PhoneApplicationService.Current.State["logined"] = false;
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

        private void GetToken(object sender, UploadStringCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "登录失败!", MessageBoxButton.OK);
                UserNameTextBox.IsEnabled = true;
                PasswordTextBox.IsEnabled = true;
            }
            else
            {
                PhoneApplicationService.Current.State["logined"] = true;

                //MessageBox.Show(e.Result.ToString());
                JObject o = JObject.Parse(e.Result);

                ThisUser.id = (string)o["id"];
                ThisUser.name = (string)o["name"];
                ThisUser.email = (string)o["email"];
                ThisUser.created_at = (DateTime)o["created_at"];
                ThisUser.avatar = (string)o["avatar"];
                ThisUser.phone = (string)o["phone"];
                ThisUser.private_token = (string)o["private_token"];

                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                if (!settings.Contains("id"))
                {
                    settings.Add("id", ThisUser.id);
                }
                if (!settings.Contains("name"))
                {
                    settings.Add("name", ThisUser.name);
                }
                if (!settings.Contains("email"))
                {
                    settings.Add("email", ThisUser.email);
                }
                if (!settings.Contains("created_at"))
                {
                    settings.Add("created_at", ThisUser.created_at);
                }
                if (!settings.Contains("avatar"))
                {
                    settings.Add("avatar", ThisUser.avatar);
                }
                if (!settings.Contains("phone"))
                {
                    settings.Add("phone", ThisUser.phone);
                }
                if (!settings.Contains("private_token"))
                {
                    settings.Add("private_token", ThisUser.private_token);
                }
                settings.Save();

                NavigationService.GoBack();
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/Help.xaml", UriKind.Relative));
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
                client.UploadStringCompleted += new UploadStringCompletedEventHandler(GetToken);
                PostArgs arg = new PostArgs();
                arg["login"] = UserNameTextBox.Text;
                arg["password"] = PasswordTextBox.Password;
                Uri newUri = new Uri(Json.host + "session?" + arg.ToString());
                //MessageBox.Show(arg.ToString());
                client.UploadStringAsync(newUri, "POST", "", (object)"");
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