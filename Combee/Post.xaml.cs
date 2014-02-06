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
using BindingData.Model;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;
using Combee.ViewModels;

namespace Combee
{
    public partial class Post : PhoneApplicationPage
    {
        public Post()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //加载正文信息
            string id = NavigationContext.QueryString["id"];

            var query = from Receipts rpt in App.NewViewModel.myDB.ReceiptsTable
                        where rpt.PostId == id
                        select rpt;
            if(query.Count() != 0)
            {
                Receipts r = query.First();
                TitleTextBlock.Text = r.Title;
                FromTextBlock.Text = r.AuthorName;

                IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream fileStream = isoFile.OpenFile(Storage.GetSmallImage(r.AuthorAvatar), FileMode.Open, FileAccess.Read);
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(fileStream);
                AvatarImage.Source = bitmap;

                string rawHtml = string.Empty;
                rawHtml += "<html><meta name='viewport' content='width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0' /><body bgcolor=\"#34495E\"><p>";
                rawHtml += "<font color=\"#FFFFFF\">";
                //rawHtml += "";
                rawHtml += r.BodyHtml;
                rawHtml += "</p></font></body></html>";
                ContentBrowser.NavigateToString(rawHtml);

                WebClient readWebClient = new WebClient();
                Uri uri = new Uri(Json.host + "receipts/" + r.Id + "/read" + Json.rear + ThisUser.private_token);
                readWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(PutedRead);
                readWebClient.UploadStringAsync(uri, "PUT", string.Empty);
            }

            else
            {
                WebClient newWebClient = new WebClient();
                newWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedPost);
                Uri uri = new Uri(Json.host + "posts" + @"/" + id + Json.rear + ThisUser.private_token);

                newWebClient.DownloadStringAsync(uri);
            }

            WebClient commentsWebClient = new WebClient();
            commentsWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedComments);
            Uri theUri = new Uri(Json.host + "posts/" + id + "/comments" + Json.rear + ThisUser.private_token);
            commentsWebClient.DownloadStringAsync(theUri);
        }

        private void RetrievedComments(object sender, DownloadStringCompletedEventArgs e)
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
                    JObject ob = JObject.Parse(arr[i].ToString());
                    Comment cm = new Comment();

                    cm.Id = (string)ob["id"];

                    cm.Body = (string)ob["body"];

                    cm.CreatedAt = (DateTime)ob["created_at"];

                    cm.UserId = (string)ob["user"]["id"];

                    cm.UserName = (string)ob["user"]["name"];

                    cm.UserAvatar = (string)ob["user"]["avatar"];

                    cm.DisplayAvatar = @"https://combee.co" + cm.UserAvatar;

                    cm.IsAvatarLocal = false;

                    Storage.SaveAvatar(cm.UserAvatar);

                    App.NewViewModel.CommentItems.Add(cm);
                }
            }
        }

        private void PutedRead(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }

        }

        private void RetrievedPost(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                string rawHtml = string.Empty;
                rawHtml += "<html><body bgcolor=\"#34495E\"><div>";
                rawHtml += "<font color=\"#FFFFFF\">";
                rawHtml += (string)o["body_html"];
                rawHtml += "</div></font></body></html>";
                ContentBrowser.NavigateToString(rawHtml);
                TitleTextBlock.Text = (string)o["title"];
                FromTextBlock.Text = (string)o["author"]["name"];
            }

        }

        private void ContentBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ContentBrowser.Opacity = 1;
        }

        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction.ToString() == "Horizontal")
            {
                this.pivot.SelectedIndex = 1;
            }
        }

        private void UserImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string id = ((Image)sender).Tag.ToString();
            NavigationService.Navigate(new Uri("/Combee;component/Users.xaml?id=" + id, UriKind.Relative));
        }

    }
}