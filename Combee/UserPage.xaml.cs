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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Combee
{
    public partial class UserPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<Organizations> _orgzCollection;
        public ObservableCollection<Organizations> OrgzCollection
        {
            get { return _orgzCollection; }
            set
            {
                _orgzCollection = value;
                NotifyPropertyChanged("OrgzCollection");
            }
        }

        private ObservableCollection<Receipts> _rptCollection;
        public ObservableCollection<Receipts> RptCollection
        {
            get { return _rptCollection; }
            set
            {
                _rptCollection = value;
                NotifyPropertyChanged("RptCollection");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // 用来通知程序某属性已改变.
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public UserPage()
        {
            InitializeComponent();

            // 为观测模型设置页面的数据上下文属性.
            //this.DataContext = App.NewViewModel;
            this.DataContext = this;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            //加载人员信息
            string id = NavigationContext.QueryString["id"];

            Users user = Storage.FindUser(id);
            if (user != null)
            {
                UseUsers(user);

                //获取用户资料
                WebClient newWebClient = new WebClient();
                newWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedUsers);
                newWebClient.DownloadStringAsync(UriString.GetUserUri(id));

                //获取用户组织
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedOrganizations);
                webClient.DownloadStringAsync(UriString.GetUserOrganizationsUri(id));

                if (RptCollection == null)
                {
                    var thisReceipts = from rpt in App.NewViewModel.myDB.ReceiptsTable
                                       where rpt.AuthorId == user.Id
                                       select rpt;

                    RptCollection = new ObservableCollection<Receipts>(thisReceipts);
                    RptCollection.Add(new Receipts());
                }

                if (OrgzCollection == null)
                {
                    OrgzCollection = new ObservableCollection<Organizations>();
                }
            }
        }

        //发送短信
        private void SmsBurron_Click(object sender, EventArgs e)
        {
            SmsComposeTask smsComposeTask = new SmsComposeTask();

            smsComposeTask.To = PhoneTextBlock.Text.Replace("手机: ",string.Empty);
            smsComposeTask.Body = string.Empty;

            smsComposeTask.Show();
        }

        //拨打电话
        private void CallButton_Click(object sender, EventArgs e)
        {
            try
            {
                PhoneCallTask phoneCallTask = new PhoneCallTask();

                phoneCallTask.PhoneNumber = PhoneTextBlock.Text.Replace("手机: ", string.Empty);
                phoneCallTask.DisplayName = NameTextBlock.Text.Replace(" ♂", "").Replace(" ♀", "");

                phoneCallTask.Show();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        //保存至通讯录
        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                SavePhoneNumberTask savePhoneNumberTask;

                savePhoneNumberTask = new SavePhoneNumberTask();
                savePhoneNumberTask.Completed += new EventHandler<TaskEventArgs>(savePhoneNumberTask_Completed);

                savePhoneNumberTask.PhoneNumber = PhoneTextBlock.Text.Replace("手机: ", string.Empty);


                savePhoneNumberTask.Show();

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        //发送电子邮件
         private void EmailButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = string.Empty;
            emailComposeTask.Body = "\n\n来自Combee客户端";
            emailComposeTask.To = MailTextBlock.Text;

            emailComposeTask.Show();
        }

        //保存联系人完成反馈
        void savePhoneNumberTask_Completed(object sender, TaskEventArgs e)
        {
            switch (e.TaskResult)
            {
                //Logic for when the number was saved successfully
                case TaskResult.OK:
                    //MessageBox.Show("保存成功咯~");
                    break;

                //Logic for when the task was cancelled by the user
                case TaskResult.Cancel:
                    //MessageBox.Show("为什么不存了捏...?");
                    break;

                //Logic for when the number could not be saved
                case TaskResult.None:
                    //MessageBox.Show("保存失败了啊...");
                    break;
            }
        }

        //接收组织列表完成反馈
        private void RetrievedOrganizations(object sender, DownloadStringCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                //MessageBox.Show(e.Error.Message);
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                foreach (JObject o in arr)
                {
                    Organizations orgz = Network.GetOrganization(o, false);

                    Storage.SaveAvatar((string)o["avatar"]);
                    OrgzCollection.Add(orgz);
                    App.NewViewModel.AddOrganizationsItem(orgz);
                }
                OrgzCollection.Add(new Organizations());
            }
        }

        private void RetrievedUsers(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                Users user = new Users();

                user.Id = (string)o["id"];
                user.Name = (string)o["name"];
                user.Email = (string)o["email"];
                user.CreatedAt = (DateTime)o["created_at"];
                user.Avatar = (string)o["avatar"];
                user.Bio = (string)o["bio"];
                user.Gender = (string)o["gender"];
                user.Qq = (string)o["qq"];
                user.Blog = (string)o["blog"];
                user.Uid = (string)o["uid"];
                user.Header = (string)o["header"];
                user.Phone = (string)o["phone"];

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

            NameTextBlock.Text = user.Name;
            switch(user.Gender)
            {
                case "男": NameTextBlock.Text += " ♂"; break;
                case "女": NameTextBlock.Text += " ♀"; break;
            }
            if(user.Email != null && user.Email != string.Empty)
            {
                MailTextBlock.Text = user.Email;
                MailTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                MailTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (user.Uid != null && user.Uid != string.Empty)
            {
                UidTextBlock.Text = "学号/工号: " + user.Uid;
                UidTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                UidTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (user.Qq != null && user.Qq != string.Empty)
            {
                QqTextBlock.Text = "QQ: " + user.Qq;
                QqTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                QqTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (user.Phone != null && user.Phone != string.Empty)
            {
                PhoneTextBlock.Text = "手机: " + user.Phone;
                PhoneTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PhoneTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (user.Bio != null && user.Bio != string.Empty)
            {
                BioTextBlock.Text = "座右铭: " + user.Bio;
                BioTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                BioTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            if(user.CreatedAt != null )
            {
                CreatedTextBlock.Text = "注册于: " + user.CreatedAt.ToString();
                CreatedTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                CreatedTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void UserImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //这里本身就是本人的优信的头像。。。让你点就死循环了。。。
        }

        private void rptPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/ReceiptPage.xaml?id=" + ((StackPanel)sender).Tag.ToString(), UriKind.Relative));
        }

        private void orgzPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/OrganizationPage.xaml?id=" + ((StackPanel)sender).Tag.ToString(), UriKind.Relative));
        }

    }
}