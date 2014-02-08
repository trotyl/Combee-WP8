using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Combee
{
    public partial class NewPost : PhoneApplicationPage
    {
        public NewPost()
        {
            InitializeComponent();
            this.DataContext = App.NewViewModel;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string time = ((Slider)sender).Value.ToString();
            if(time.Length > 3)
            {
                time = time.Substring(0, 3);
                if(time[2] == '.')
                {
                    time = time.Substring(0, 2);
                }
            }
            TimeBlock.Text = time;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            TimeSlider.Value = 2.0;
            PostState.organizations.Clear();
        }

        private void TitleBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TitleBox.Text == "标题")
            {
                TitleBox.Text = string.Empty;
            }
        }

        private void BodyBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (BodyBox.Text == "内容")
            {
                BodyBox.Text = string.Empty;
            }
        }

        private void TitleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TitleBox.Text == string.Empty)
            {
                TitleBox.Text = "标题";
            }
        }

        private void BodyBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (BodyBox.Text == string.Empty)
            {
                BodyBox.Text = "内容";
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PostState.organizations.Add(((CheckBox)sender).Tag.ToString());
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PostState.organizations.Remove(((CheckBox)sender).Tag.ToString());
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            string uriStr = Json.host + "posts" + Json.rear + ThisUser.private_token;
            uriStr += "&title=" + TitleBox.Text;
            uriStr += "&body_html=" + BodyBox.Text;
            if (PostState.organizations.Count > 0)
            {
                foreach (string orgz in PostState.organizations)
                {
                    uriStr += "&organization_ids[]=" + orgz;
                }
                int time = (int)(TimeSlider.Value * 3600);
                if (TimeBox.IsChecked == true)
                {
                    uriStr += "&delayed_sms_at=" + time;
                }
                WebClient submitWebClient = new WebClient();
                Uri uri = new Uri(uriStr);
                submitWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(Submitted);
                submitWebClient.UploadStringAsync(uri, "POST", string.Empty);
            }
            else
            {
                MessageBox.Show("组织列表为空!");
            }
        }

        private void Submitted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}