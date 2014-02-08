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
    public partial class Organization : PhoneApplicationPage
    {
        public Organization()
        {
            InitializeComponent();
        }

        private void UserImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string id = ((Image)sender).Tag.ToString();
            NavigationService.Navigate(new Uri("/Combee;component/Users.xaml?id=" + id, UriKind.Relative));
        }

        private void fullUmsgStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/Post.xaml?id=" + ((StackPanel)sender).Tag.ToString(), UriKind.Relative));
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/Organization.xaml?id=" + ((StackPanel)sender).Tag.ToString(), UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //加载组织信息
            string id = NavigationContext.QueryString["id"];
            var query_organization = from orgz in App.NewViewModel.myDB.OrganizationsTable
                                     where orgz.Id == id
                                     select orgz;
            if (query_organization.Count() != 0)
            {
                Organizations orgz = query_organization.First();
                UseOrganization(orgz);
            }

            {
                //获取组织资料
                WebClient orgzWebClient = new WebClient();
                orgzWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedOrganizations);
                Uri uri = new Uri(Json.host + "organizations/" + id + Json.rear + ThisUser.private_token);

                orgzWebClient.DownloadStringAsync(uri);
            }
            {
                //获取组织的下级组织
                WebClient childrenWebClient = new WebClient();
                childrenWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedChildren);
                Uri uri = new Uri(Json.host + "organizations/" + id + "/children" + Json.rear + ThisUser.private_token);

                childrenWebClient.DownloadStringAsync(uri);

                App.NewViewModel.OrganizationsItems.Clear();
            }
            {
                //获取组织的人员
                WebClient userWebClient = new WebClient();
                userWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedMembers);
                Uri uri = new Uri(Json.host + "organizations/" + id + "/members" + Json.rear + ThisUser.private_token);

                userWebClient.DownloadStringAsync(uri);
                
                App.NewViewModel.OrganizationsItems.Clear();
            }
        }

        private void RetrievedChildren(object sender, DownloadStringCompletedEventArgs e)
        {
            throw new NotImplementedException();
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
                for (int i = 0; i < arr.Count(); i++)
                {
                    JObject o = JObject.Parse(arr[i].ToString());
                    Users user = new Users();


                }
            }
        }

        private void RetrievedOrganizations(object sender, DownloadStringCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UseOrganization(Organizations orgz)
        {
            throw new NotImplementedException();
        }

    }
}