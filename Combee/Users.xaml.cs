using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using BindingData.Model;
using BindingData.ViewModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using System.IO.IsolatedStorage;
using System.IO;

namespace Combee
{
    public partial class People : PhoneApplicationPage
    {
        public People()
        {
            InitializeComponent();

            // 为观测模型设置页面的数据上下文属性.
            this.DataContext = App.NewViewModel;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //加载人员信息
            string id = NavigationContext.QueryString["id"];
            var thisPerson = from user in App.NewViewModel.myDB.UsersTable
                             where user.Phone == id
                             select user;
            if(thisPerson.Count() != 0)
            {
                foreach(Users user in thisPerson)
                {
                    UseUsers(user);
                }
            }
            else
            {
                WebClient newWebClient = new WebClient();
                newWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedUsers);
                Uri uri = new Uri(Json.host + "users" + @"/" + id + Json.rear + ThisUser.private_token);

                newWebClient.DownloadStringAsync(uri);
            }
        }

        private void SmsBurron_Click(object sender, EventArgs e)
        {
            SmsComposeTask smsComposeTask = new SmsComposeTask();

            smsComposeTask.To = PhoneTextBlock.Text.Replace("手机: ",string.Empty);
            smsComposeTask.Body = string.Empty;

            smsComposeTask.Show();
        }

        private void CallButton_Click(object sender, EventArgs e)
        {
            PhoneCallTask phoneCallTask = new PhoneCallTask();

            phoneCallTask.PhoneNumber = PhoneTextBlock.Text.Replace("手机: ", string.Empty);
            phoneCallTask.DisplayName = NameTextBlock.Text;

            phoneCallTask.Show();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SavePhoneNumberTask savePhoneNumberTask;
            
            savePhoneNumberTask = new SavePhoneNumberTask();
            savePhoneNumberTask.Completed += new EventHandler<TaskEventArgs>(savePhoneNumberTask_Completed);

            savePhoneNumberTask.PhoneNumber = PhoneTextBlock.Text.Replace("手机: ", string.Empty);

            savePhoneNumberTask.Show();
        }

        void savePhoneNumberTask_Completed(object sender, TaskEventArgs e)
        {
            //switch (e.TaskResult)
            //{
            //    //Logic for when the number was saved successfully
            //    case TaskResult.OK:
            //        MessageBox.Show("保存成功咯~");
            //        break;

            //    //Logic for when the task was cancelled by the user
            //    case TaskResult.Cancel:
            //        MessageBox.Show("为什么不存了捏...?");
            //        break;

            //    //Logic for when the number could not be saved
            //    case TaskResult.None:
            //        MessageBox.Show("保存失败了啊...");
            //        break;
            //}
        }

        private void RetrievedUsers(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                Users user = new Users();

                string id = (string)o["id"];
                user.Id = id;

                string name = (string)o["name"];
                user.Name = name;

                string email = (string)o["email"];
                user.Email = email;

                string created_at = (string)o["created_at"];
                user.CreatedAt = created_at;

                string avatar = (string)o["avatar"];
                user.Avatar = avatar;

                string bio = (string)o["bio"];
                user.Bio = bio;

                string gender = (string)o["gender"];
                user.Gender = gender;

                string qq = (string)o["qq"];
                user.Qq = qq;

                string blog = (string)o["blog"];
                user.Blog = blog;

                string uid = (string)o["uid"];
                user.Uid = uid;

                string header = (string)o["header"];
                user.Header = header;

                string phone = (string)o["phone"];
                user.Phone = phone;

                UseUsers(user);
                App.NewViewModel.AddUsersItem(user);

            }
        }

        private void UseUsers(Users user)
        {
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fileStream = isoFile.OpenFile(Storage.GetSmallImage(user.Avatar), FileMode.Open, FileAccess.Read);
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(fileStream);
            AvatarImage.Source = bitmap;

            //AvatarImage.Source = new BitmapImage(new Uri(@"https://combee.co/" + user.Avatar));
            NameTextBlock.Text = user.Name;
            switch(user.Gender)
            {
                case "男": NameTextBlock.Text += " ♂"; break;
                case "女": NameTextBlock.Text += " ♀"; break;
            }
            if(user.Email != null && user.Email != string.Empty)
            {
                MailTextBlock.Text = user.Email;
            }
            else
            {
                MailTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (user.Uid != null && user.Uid != string.Empty)
            {
                UidTextBlock.Text = "学号/工号: " + user.Uid;
            }
            else
            {
                UidTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (user.Qq != null && user.Qq != string.Empty)
            {
                QqTextBlock.Text = "QQ: " + user.Qq;
            }
            else
            {
                QqTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (user.Phone != null && user.Phone != string.Empty)
            {
                PhoneTextBlock.Text = "手机: " + user.Phone;
            }
            else
            {
                PhoneTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (user.Bio != null && user.Bio != string.Empty)
            {
                BioTextBlock.Text = "座右铭: " + user.Bio;
            }
            else
            {
                BioTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void UserImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void fullUmsgStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }


    }
}