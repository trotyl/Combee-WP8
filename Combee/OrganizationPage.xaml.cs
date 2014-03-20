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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Combee
{
    public partial class OrganizationPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<Organizations> _parentCollection;
        public ObservableCollection<Organizations> ParentCollection
        {
            get { return _parentCollection; }
            set
            {
                _parentCollection = value;
                NotifyPropertyChanged("ParentCollection");
            }
        }

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

        private ObservableCollection<Users> _userCollection;
        public ObservableCollection<Users> UserCollection
        {
            get { return _userCollection; }
            set
            {
                _userCollection = value;
                NotifyPropertyChanged("UserCollection");
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

        public OrganizationPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            //加载组织信息
            string id = NavigationContext.QueryString["id"];

            Organizations o = Storage.FindOrganization(id);

            if (o != null)
            {
                UseOrganization(o);
                if (ParentCollection == null)
                {
                    ParentCollection = new ObservableCollection<Organizations>();

                    var parentQuery = from Organizations org in App.NewViewModel.myDB.OrganizationsTable
                                      where org.Id == o.ParentId
                                      select org;
                    if (parentQuery.Count() == 0)
                    {
                        //获取上级组织资料
                        WebClient parentWebClient = new WebClient();
                        parentWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedParent);
                        parentWebClient.DownloadStringAsync(UriString.GetOrganizationUri(o.ParentId));
                    }
                    else
                    {
                        ParentCollection.Add(parentQuery.First());
                    }
                }
            }

            if (RptCollection == null)
            {
                var rptQuery = from Receipts rpt in App.NewViewModel.myDB.ReceiptsTable
                               where rpt.Organizations.IndexOf(id) != -1
                               orderby rpt.Id descending
                               select rpt;
                RptCollection = new ObservableCollection<Receipts>(rptQuery);
                RptCollection.Add(new Receipts());
            }

            if (OrgzCollection == null)
            {
                var orgzQuery = from Organizations or in App.NewViewModel.myDB.OrganizationsTable
                                where or.ParentId == id
                                select or;
                OrgzCollection = new ObservableCollection<Organizations>(orgzQuery);
                OrgzCollection.Add(new Organizations());
            }

            if (UserCollection == null)
            {
                UserCollection = new ObservableCollection<Users>();

                //获取组织资料
                WebClient orgzWebClient = new WebClient();
                orgzWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedOrganization);
                orgzWebClient.DownloadStringAsync(UriString.GetOrganizationUri(id));

                //获取组织的下级组织
                WebClient childrenWebClient = new WebClient();
                childrenWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedChildren);
                childrenWebClient.DownloadStringAsync(UriString.GetOrganizationChildrenUri(id));

                //获取组织的人员
                WebClient userWebClient = new WebClient();
                userWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedMembers);
                userWebClient.DownloadStringAsync(UriString.GetOrganizationMembersUri(id));
            }
        }


        private void RetrievedChildren(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message);
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                foreach (JObject o in arr)
                {
                    Organizations orgz = Network.GetOrganization(o, false);

                    Storage.SaveAvatar(orgz.Avatar);
                    bool already = false;
                    foreach (Organizations or in OrgzCollection)
                    {
                        if (or.Id == orgz.Id)
                        {
                            already = true;
                            break;
                        }
                    }
                    if (!already)
                    {
                        OrgzCollection.Insert(0, orgz);
                    }
                    App.NewViewModel.AddOrganizationsItem(orgz);
                }
            }
        }

        private void RetrievedMembers(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message);
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                foreach (JObject o in arr)
                {
                    Users user = Network.GetUser(o);

                    Storage.SaveAvatar(user.Avatar);
                    UserCollection.Add(user);
                    App.NewViewModel.AddUsersItem(user);
                }
                UserCollection.Add(new Users());
            }
        }

        private void RetrievedParent(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message);
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                Organizations orgz = Network.GetOrganization(o, false);

                Storage.SaveAvatar(orgz.Avatar);
                ParentCollection.Add(orgz);
                App.NewViewModel.AddOrganizationsItem(orgz);
            }
        }

        private void RetrievedOrganization(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message);
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                Organizations orgz = Network.GetOrganization(o, false);

                Storage.SaveAvatar(orgz.Avatar);
                UseOrganization(orgz);
                App.NewViewModel.AddOrganizationsItem(orgz);
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

        private void userPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Combee;component/UserPage.xaml?id=" + ((StackPanel)sender).Tag.ToString(), UriKind.Relative));
        }
    }
}