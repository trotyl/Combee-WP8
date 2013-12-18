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

namespace Combee
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void LikeButton_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void SupportButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "使用反馈: " + "Combee For WindowsPhone";
            emailComposeTask.Body = "版本号: 1.0.0.0&#13;";
            emailComposeTask.To = "trotyl@qq.com";
            //emailComposeTask.Cc = "cc@example.com";
            //emailComposeTask.Bcc = "bcc@example.com";

            emailComposeTask.Show();
        }

        private void IntroductionButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/Introduction.xaml", UriKind.Relative));
        }
    }
}