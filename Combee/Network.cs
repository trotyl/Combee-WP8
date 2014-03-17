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
    class Network
    {

        public static void GetAsyncAll()
        {
            GetAsync("receipts", "receipts");
            //GetAsync("organizations", "users/" + CurrentUser.GetId() + "/organizations");
            //GetAsync("conversations", "conversations");
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
                Uri uri = UriString.GetModeUri(mode, i);
                webClient.DownloadStringAsync(uri);
            }
        }

        //接收到优信列表
        private static void RetrievedReceipts(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                for (int i = arr.Count - 1; i >= 0; i--)
                {
                    JObject o = JObject.Parse(arr[i].ToString());
                    Receipts rpt = GetReceipt(o);
                    Users user = GetReceiptUser(o);

                    App.NewViewModel.AddReceiptsItem(rpt);
                    App.NewViewModel.AddUsersItem(user);
                }
            }
        }

        private static void RetrievedOrganizations(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                foreach(JObject o in arr)
                {
                    Organizations orgz = GetOrganization(o, true);

                    App.NewViewModel.AddOrganizationsItem(orgz);
                }
            }
        }

        private static void RetrievedConversations(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JArray arr = JArray.Parse(e.Result);
                foreach(JObject o in arr)
                {
                    Users org, lst;
                    Conversations cov = GetConversation(o, out org, out lst);

                    App.NewViewModel.AddConversationsItem(cov);
                    App.NewViewModel.AddUsersItem(org);
                    App.NewViewModel.AddUsersItem(lst);
                }
            }
        }

        internal static Receipts GetReceipt(JObject o)
        {
            Receipts rpt = new Receipts();

            rpt.Id = (string)o["id"];
            rpt.Read = (bool)o["read"];
            rpt.Favorited = (bool)o["favorited"];
            rpt.Origin = (bool)o["origin"];
            rpt.Organizations = o["organizations"].ToString();
            rpt.Attachments = o["post"]["attachments"].ToString();
            rpt.Title = (string)o["post"]["title"];
            rpt.PostId = (string)o["post"]["id"];
            rpt.Body = (string)o["post"]["body"];
            rpt.BodyHtml = (string)o["post"]["body_html"];
            rpt.CreatedAt = (DateTime?)o["post"]["created_at"];
            rpt.AuthorId = (string)o["post"]["author"]["id"];
            rpt.AuthorName = (string)o["post"]["author"]["name"];
            rpt.AuthorAvatar = (string)o["post"]["author"]["avatar"];
            rpt.DisplayAvatar = @"https://combee.co" + rpt.AuthorAvatar;
            rpt.IsAvatarLocal = false;
            if (o["post"]["forms"].Count() > 0)
            {
                rpt.FormId = (string)o["post"]["forms"][0]["id"];
                rpt.FormTitle = (string)o["post"]["forms"][0]["title"];
            }

            return rpt;
        }

        internal static Users GetReceiptUser(JObject o)
        {
            Users user = new Users();

            user.Id = (string)o["post"]["author"]["id"];
            user.Name = (string)o["post"]["author"]["name"];
            user.Email = (string)o["post"]["author"]["email"];
            user.CreatedAt = (DateTime?)o["post"]["author"]["created_at"];
            user.Avatar = (string)o["post"]["author"]["avatar"];
            user.Phone = (string)o["post"]["author"]["phone"];
            user.DisplayAvatar = @"https://combee.co" + user.Avatar;
            user.IsAvatarLocal = false;

            return user;
        }

        internal static Users GetUser(JObject o)
        {
            Users user = new Users();

            user.Id = (string)o["id"];
            user.Name = (string)o["name"];
            user.Email = (string)o["email"];
            user.CreatedAt = (DateTime?)o["created_at"];
            user.Avatar = (string)o["avatar"];
            user.Phone = (string)o["phone"];
            user.DisplayAvatar = @"https://combee.co" + user.Avatar;
            user.IsAvatarLocal = false;

            return user;
        }

        internal static Organizations GetOrganization(JObject o, bool belong)
        {
            Organizations orgz = new Organizations();

            orgz.Id = (string)o["id"];
            orgz.Name = (string)o["name"];
            orgz.CreatedAt = (DateTime?)o["created_at"];
            orgz.Avatar = (string)o["avatar"];
            orgz.DisplayAvatar = @"https://combee.co" + orgz.Avatar;
            orgz.IsAvatarLocal = false;
            orgz.ParentId = (string)o["parent_id"];
            orgz.Members = (string)o["members"];
            orgz.Bio = (string)o["bio"];
            orgz.Header = (string)o["header"];
            orgz.Belong = belong;
            orgz.JoinedAt = (DateTime?)o["joined_at"];

            return orgz;
        }

        internal static Conversations GetConversation(JObject o, out Users org, out Users lst)
        {
            Conversations cov = new Conversations();
            org = new Users();
            lst = new Users();

            cov.Id = (string)o["id"];
            cov.Body = (string)o["last_message"]["body"];
            cov.CreatedAt = (DateTime?)o["created_at"];
            cov.UpdatedAt = (DateTime?)o["updated_at"];
            cov.ParticipantsId = string.Empty;
            for (int ii = 0; ii < o["participants"].Count(); ii++)
            {
                cov.ParticipantsId += ((string)o["participants"][ii]["id"] + "_");
            }
            cov.ParticipantsName = string.Empty;
            for (int ii = 0; ii < o["participants"].Count(); ii++)
            {
                if ((string)o["participants"][ii]["id"] != CurrentUser.GetId())
                {
                    cov.ParticipantsName += ((string)o["participants"][ii]["name"] + " ");
                }
            }
            cov.OriginatorAvatar = (string)o["originator"]["avatar"];
            cov.LastAvatar = (string)o["last_message"]["user"]["avatar"];
            if (o["participants"].Count() <= 2)
            {
                cov.DisplayAvatar = @"https://combee.co" + cov.OriginatorAvatar;
            }
            else
            {
                cov.DisplayAvatar = @"https://combee.co" + cov.LastAvatar;               
            }

            cov.LastName = (string)o["last_message"]["user"]["name"];
            cov.IsAvatarLocal = false;
            cov.OriginatorId = (string)o["originator"]["id"];

            lst.Id = (string)o["last_message"]["user"]["id"];
            lst.Name = (string)o["last_message"]["user"]["id"];
            lst.Email = (string)o["last_message"]["user"]["email"];
            lst.CreatedAt = (DateTime?)o["last_message"]["user"]["created_at"];
            lst.Avatar = (string)o["last_message"]["user"]["avatar"];
            lst.DisplayAvatar = @"https://combee.co" + lst.Avatar;
            lst.IsAvatarLocal = false;
            lst.Phone = (string)o["last_message"]["user"]["phone"];

            org.Id = (string)o["originator"]["id"];
            org.Name = (string)o["originator"]["name"];
            org.Email = (string)o["originator"]["email"];
            org.CreatedAt = (DateTime?)o["originator"]["created_at"];
            org.Avatar = (string)o["originator"]["avatar"];
            org.DisplayAvatar = @"https://combee.co" + org.Avatar;
            org.IsAvatarLocal = false;
            org.Phone = (string)o["originator"]["phone"];

            return cov;
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

    class PostState
    {
        public static List<string> organizations = new List<string>();
    }
}
