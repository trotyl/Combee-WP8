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
using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;

namespace Combee
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
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
            string body = "设备信息：\n";
            body += DeviceStatus.DeviceManufacturer + "\n";
            body += DeviceStatus.DeviceName +"\n";
            body += DeviceStatus.DeviceFirmwareVersion + "\n";
            body += DeviceStatus.DeviceHardwareVersion + "\n";
            body += DeviceStatus.DeviceTotalMemory.ToString() + "\n";
            body += DeviceStatus.ApplicationCurrentMemoryUsage.ToString() + "\n";
            body += DeviceStatus.ApplicationMemoryUsageLimit.ToString() + "\n";
            body += DeviceStatus.ApplicationPeakMemoryUsage.ToString() + "\n";
            body += DeviceStatus.IsKeyboardPresent.ToString() + "\n";
            body += DeviceStatus.IsKeyboardDeployed.ToString() + "\n";
            body += DeviceStatus.PowerSource.ToString() + "\n";
            body += DeviceNetworkInformation.CellularMobileOperator + "\n";
            body += DeviceNetworkInformation.IsNetworkAvailable.ToString() + "\n";
            body += DeviceNetworkInformation.IsCellularDataEnabled.ToString() + "\n";
            body += DeviceNetworkInformation.IsCellularDataRoamingEnabled.ToString() + "\n";
            body += DeviceNetworkInformation.IsWiFiEnabled.ToString() + "\n";

            body += "\n请输入反馈内容：\n";
            emailComposeTask.Body = body;
            emailComposeTask.To = "support@combee.co";
            emailComposeTask.Cc = "trotyl@qq.com";
            //emailComposeTask.Bcc = "bcc@example.com";

            emailComposeTask.Show();
        }

        private void IntroductionButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/IntroductionPage.xaml", UriKind.Relative));
        }
    }
}