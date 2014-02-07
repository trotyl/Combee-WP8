using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;
using BindingData.Model;

namespace Combee
{
    class Json
    {
        //静态参数
        public static string host = "https://combee.co/api/v1/";
        public static string rear = ".json?private_token=";

        public static void GetCurrentUser()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetUser);
            Uri newUri = new Uri(host + "user" + rear + ThisUser.private_token);
            webClient.DownloadStringAsync(newUri);
        }

        private static void GetUser(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("连接服务器失败! 请检查您的网络连接!", "错误" + e.Error.HResult.ToString(), MessageBoxButton.OK);
                //MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JObject o = JObject.Parse(e.Result);

                ThisUser.name = (string)o["name"];
                ThisUser.id = (string)o["id"];
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                if (!settings.Contains("userName"))
                {
                    settings.Add("userName", ThisUser.name);
                }

                if (!settings.Contains("userId"))
                {
                    settings.Add("userId", ThisUser.id);
                }
                settings.Save();

                GetAsync("receipts", "receipts");
                GetAsync("organizations", "user/" + ThisUser.id + "/organizations");
                GetAsync("conversations", @"user/conversations");
            }
        }

        public static void GetAsync(string item, string mode)
        {
            //接收消息
            for (int i = 1; i <= 3; i++ )
            {
                WebClient webClient = new WebClient();
                switch (item)
                {
                    case "receipts":
                        {
                            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedReceipts);
                            break;
                        }
                    case "organizations":
                        {
                            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedOrganizations);
                            break;
                        }
                    case "conversations":
                        {
                            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedConversations);
                            break;
                        }
                }
                Uri uri = new Uri(host + mode + rear + ThisUser.private_token + "&page=" + i);
                webClient.DownloadStringAsync(uri);
            }
        }

        //接收到优信列表
        private static void RetrievedReceipts(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                for (int i = arr.Count - 1; i >= 0; i--)
                {
                    JToken t = arr[i];
                    JObject o = JObject.Parse(t.ToString());
                    Receipts rpt = new Receipts();

                    string id = (string)o["id"];
                    rpt.Id = id;

                    bool read = (bool)o["read"];
                    rpt.Read = read;

                    bool favorited = (bool)o["favorited"];
                    rpt.Favorited = favorited;

                    bool origin = (bool)o["origin"];
                    rpt.Origin = origin;

                    string title = (string)o["post"]["title"];
                    rpt.Title = title;

                    string organizations_id = string.Empty;
                    for (int ii = 0; ii < o["organizations"].Count(); ii++)
                    {
                        organizations_id += ((string)o["organizations"][ii]["id"] + "_");
                    }
                    rpt.OrganizationsId = organizations_id;

                    string post_id = (string)o["post"]["id"];
                    rpt.PostId = post_id;

                    string body = (string)o["post"]["body"];
                    rpt.Body = body;

                    string body_html = (string)o["post"]["body_html"];
                    rpt.BodyHtml = body_html;

                    DateTime created_at = (DateTime)o["post"]["created_at"];
                    rpt.CreatedAt = created_at;

                    Users user = new Users();

                    string author_id = (string)o["post"]["author"]["id"];
                    rpt.AuthorId = author_id;
                    user.Id = author_id;

                    string author_name = (string)o["post"]["author"]["name"];
                    rpt.AuthorName = author_name;
                    user.Name = author_name;

                    string author_email = (string)o["post"]["author"]["email"];
                    user.Email = author_email;

                    string author_created = (string)o["post"]["author"]["created_at"];
                    user.CreatedAt = author_created;

                    string author_avatar = (string)o["post"]["author"]["avatar"];
                    rpt.AuthorAvatar = author_avatar;
                    user.Avatar = author_avatar;

                    string author_phone = (string)o["post"]["author"]["phone"];
                    user.Phone = author_phone;

                    if(o["post"]["forms"].Count() > 0)
                    {
                        string form_id = (string)o["post"]["forms"][0]["id"];
                        rpt.FormId = form_id;

                        string form_title = (string)o["post"]["forms"][0]["title"];
                        rpt.FormTitle = form_title;

                    }

                    rpt.DisplayAvatar = @"https://combee.co" + author_avatar;
                    user.DisplayAvatar = @"https://combee.co" + author_avatar;

                    rpt.IsAvatarLocal = false;
                    user.IsAvatarLocal = false;

                    App.NewViewModel.AddUsersItem(user);
                    App.NewViewModel.AddReceiptsItem(rpt);

                }
            }
        }

        private static void RetrievedOrganizations(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                for (int i = 0; i < arr.Count; i++)
                {
                    JToken t = arr[i];
                    JObject o = JObject.Parse(t.ToString());
                    Organizations orgz = new Organizations();

                    string id = (string)o["id"];
                    orgz.Id = id;

                    string name = (string)o["name"];
                    orgz.Name = name;

                    DateTime created_at = (DateTime)o["created_at"];
                    orgz.CreatedAt = created_at;

                    string avatar = (string)o["avatar"];
                    orgz.Avatar = avatar;

                    orgz.DisplayAvatar = @"https://combee.co" + avatar;

                    orgz.IsAvatarLocal = false;

                    string parent_id = (string)o["parent_id"];
                    orgz.ParentId = parent_id;

                    string members = (string)o["members"];
                    orgz.Members = members;

                    //string bio = (string)o["bio"];
                    //orgz.Bio = bio;
                    orgz.Bio = null;

                    //string header = (string)o["header"];
                    //orgz.Header = header;
                    orgz.Header = null;

                    //if ((string)o["joined_at"] == null)
                    //{
                    //    orgz.InIt = false;
                    //    orgz.JoinedAt = DateTime.Now;
                    //}
                    //else
                    //{
                    //    orgz.InIt = true;
                    //    DateTime joined_at = (DateTime)o["joined_at"];
                    //    orgz.JoinedAt = joined_at;
                    //}
                    orgz.InIt = true;
                    orgz.JoinedAt = DateTime.Now;

                    App.NewViewModel.AddOrganizationsItem(orgz);
                }
            }
        }

        private static void RetrievedConversations(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                for (int i = 0; i < arr.Count; i++)
                {
                    JToken t = arr[i];
                    JObject o = JObject.Parse(t.ToString());
                    Conversations cov = new Conversations();

                    string id = (string)o["id"];
                    cov.Id = id;

                    string body = (string)o["last_message"]["body"];
                    cov.Body = body;

                    DateTime created_at = (DateTime)o["created_at"];
                    cov.CreatedAt = created_at;

                    DateTime updated_at = (DateTime)o["updated_at"];
                    cov.UpdatedAt = updated_at;

                    string participants_id = string.Empty;
                    for (int ii = 0; ii < o["participants"].Count(); ii++)
                    {
                        participants_id += ((string)o["participants"][ii]["id"] + "_");
                    }
                    cov.ParticipantsId = participants_id;

                    string participants_name = string.Empty;
                    for (int ii = 0; ii < o["participants"].Count(); ii++)
                    {
                        if ((string)o["participants"][ii]["name"] != ThisUser.name)
                        {
                            participants_name += ((string)o["participants"][ii]["name"] + " ");
                        }
                    }
                    cov.ParticipantsName = participants_name;

                    string originator_avatar = (string)o["originator"]["avatar"];
                    cov.OriginatorAvatar = originator_avatar;

                    string originator_id = (string)o["originator"]["id"];
                    cov.OriginatorId = originator_id;

                    cov.DisplayAvatar = @"https://combee.co" + originator_avatar;

                    cov.IsAvatarLocal = false;

                    App.NewViewModel.AddConversationsItem(cov);
                }
            }
        }

        private static void RetrievedDetailOrganizations(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                Organizations orgz = new Organizations();

                string id = (string)o["id"];
                orgz.Id = id;

                string name = (string)o["name"];
                orgz.Name = name;

                DateTime created_at = (DateTime)o["created_at"];
                orgz.CreatedAt = created_at;

                string avatar = (string)o["avatar"];
                orgz.Avatar = avatar;

                orgz.DisplayAvatar = @"https://combee.co" + avatar;

                orgz.IsAvatarLocal = false;

                string parent_id = (string)o["parent_id"];
                orgz.ParentId = parent_id;

                string members = (string)o["members"];
                orgz.Members = members;

                string bio = (string)o["bio"];
                orgz.Bio = bio;

                string header = (string)o["header"];
                orgz.Header = header;
                orgz.Header = null;

                if ((string)o["joined_at"] == null)
                {
                    orgz.InIt = false;
                    orgz.JoinedAt = DateTime.Now;
                }
                else
                {
                    orgz.InIt = true;
                    DateTime joined_at = (DateTime)o["joined_at"];
                    orgz.JoinedAt = joined_at;
                }

                App.NewViewModel.AddOrganizationsItem(orgz);

            }
        }

    }

    public class PostArgs
    {
        Dictionary<string, string> values;
        public PostArgs()
        {
            this.values = new Dictionary<string, string>();
        }
        public string this[string index]
        {
            get { return this.values[index]; }
            set { this.values[index] = value; }
        }
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            var em = this.values.GetEnumerator();
            bool first = true;
            while (em.MoveNext())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    result.Append('&');
                }
                result.Append(em.Current.Key);
                result.Append('=');
                result.Append(HttpUtility.UrlEncode(em.Current.Value));
            }
            return result.ToString();
        }
    }

    class Form
    {
        public static string id;
        public static string title;
        public static List<FormItem> list = new List<FormItem>();
    }

    class FormItem
    {
        public string id;
        public bool required;
        public string identifier;
        public string _type;
        public string label;
        public string value;
        public bool ready;
        public bool number;
    }
}
