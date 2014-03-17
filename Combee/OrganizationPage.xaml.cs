using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BindingData.Model;
using Newtonsoft.Json.Linq;

namespace Combee
{
    public partial class OrganizationPage : PhoneApplicationPage
    {
        public OrganizationPage()
        {
            InitializeComponent();

            this.DataContext = App.NewViewModel;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.NewViewModel.OrganizationsItems.Clear();
            App.NewViewModel.ReceiptsItems.Clear();
            App.NewViewModel.UsersItems.Clear();

            //加载组织信息
            string id = NavigationContext.QueryString["id"];
            Organizations o = Storage.FindOrganization(id);
            
            if (o != null)
            {
                UseOrganization(o);
            }
            var query = from Receipts rpt in App.NewViewModel.myDB.ReceiptsTable
                        where rpt.Organizations.IndexOf(id) != -1
                        select rpt;
            foreach (Receipts r in query)
            {
                App.NewViewModel.ReceiptsItems.Add(r);
            }

            //获取组织的下级组织
            WebClient childrenWebClient = new WebClient();
            childrenWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedChildren);
            childrenWebClient.DownloadStringAsync(UriString.GetOrganizationChildrenUri(id));

            //获取组织的人员
            WebClient userWebClient = new WebClient();
            userWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedMembers);
            userWebClient.DownloadStringAsync(UriString.GetOrganizationMembersUri(id));

            //获取组织资料
            WebClient orgzWebClient = new WebClient();
            orgzWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedOrganization);
            orgzWebClient.DownloadStringAsync(UriString.GetOrganizationUri(id));
        }

        private void RetrievedChildren(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                foreach(JObject o in arr)
                {
                    Organizations orgz = Network.GetOrganization(o, false);

                    Storage.SaveAvatar(orgz.Avatar);
                    App.NewViewModel.OrganizationsItems.Add(orgz);
                    App.NewViewModel.AddOrganizationsItem(orgz);
                }
                Organizations org = new Organizations();
                App.NewViewModel.OrganizationsItems.Add(org);
            }
        }

        private void RetrievedMembers(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                foreach(JObject o in arr)
                {
                    Users user = Network.GetUser(o);

                    Storage.SaveAvatar(user.Avatar);
                    App.NewViewModel.UsersItems.Add(user);
                    App.NewViewModel.AddUsersItem(user);
                }
                Users use = new Users();
                App.NewViewModel.UsersItems.Add(use);
            }
        }

        private void RetrievedOrganization(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                Organizations orgz = Network.GetOrganization(o, false);

                Storage.SaveAvatar(orgz.Avatar);
                UseOrganization(orgz);
            }
        }

        private void UseOrganization(Organizations o)
        {
            pivot.Title = o.Name;
            if (o.Bio != null && o.Bio != string.Empty)
                BioBlock.Text = o.Bio;
            MemberBlock.Text = o.Members + "人";
            AvatarImage.Source = Storage.GetImageSource(o.Avatar);
        }

        private void UserImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string id = ((Image)sender).Tag.ToString();
            NavigationService.Navigate(new Uri("/Combee;component/UserPage.xaml?id=" + id, UriKind.Relative));
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