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
    public partial class NewPostPage : PhoneApplicationPage
    {
        public NewPostPage()
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
            if (PostState.organizations.Count == 0)
            {
                MessageBox.Show("所选发送组织为空!");
                return;
            }
            SubmitButton.IsEnabled = false;
            int time = (int)(TimeSlider.Value * 3600);
            Uri uri = UriString.GetPostUri(TitleBox.Text, BodyBox.Text, (TimeBox.IsChecked == true ? (int?)time : null));

            WebClient submitWebClient = new WebClient();
            submitWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(Submitted);
            submitWebClient.UploadStringAsync(uri, "POST", string.Empty);
        }
       
        private void Submitted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                SubmitButton.IsEnabled = true;
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void choseButton_Click(object sender, EventArgs e)
        {
            if (pivot.SelectedIndex == 0)
            {
                pivot.SelectedIndex = 1;
            }
            else
            {
                pivot.SelectedIndex = 0;
            }

        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pivot.SelectedIndex == 1)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "编辑内容";
            }
            else
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "选择组织";
            }

        }
    }
}