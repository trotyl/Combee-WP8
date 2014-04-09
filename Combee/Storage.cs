using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows;
using System.ComponentModel;
using BindingData.Model;
using BindingData.ViewModel;
using Newtonsoft.Json.Linq;

namespace Combee
{
    class CurrentUser
    {
        private static bool? login = null;
        private static int? count = null;
        private static string name = null;
        private static string id = null;
        private static string email = null;
        private static DateTime? created_at = null;
        private static string avatar = null;
        private static string phone = null;
        private static string private_token = null;
        private static string channel_uri = null;

        public static void SetLogin(bool _login) { login = _login; Save(); }
        public static void SetCount(int? _count) { count = _count; Save(); }
        public static void SetName(string _name) { name = _name; Save(); }
        public static void SetId(string _id) { id = _id; Save(); }
        public static void SetEmail(string _email) { email = _email; Save(); }
        public static void SetCreatedAt(DateTime? _created_at) { created_at = _created_at; Save(); }
        public static void SetAvatar(string _avatar) { avatar = _avatar; Save(); }
        public static void SetPhone(string _phone) { phone = _phone; Save(); }
        public static void SetPrivate_token(string _private_token) { private_token = _private_token; Save(); }
        public static void SetChannelUri(string _channel_uri) { channel_uri = _channel_uri; Save(); }

        public static bool IsLogin() 
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if(settings.Contains("login"))
            {
                login = (bool?)settings["login"];
            }
            if(login == null || login == false)
                return false;
            return true;
        }

        public static bool IsFirst()
        {
            if (count == null || count == 0)
                return true;
            return false;
        }
    
        public static int? GetCount() { return count; }
        public static string GetName() { return name; }
        public static string GetId() { return id; }
        public static string GetEmail() { return email; }
        public static DateTime? GetCreatedAt() { return created_at; }
        public static string GetAvatar() { return avatar; }
        public static string GetPhone() { return phone; }
        public static string GetPrivate_token() { return private_token; }
        public static string GetChannelUri() { return channel_uri; }

        public static void Login(JObject o)
        {
            SetLogin(true);
            SetCount(0);
            SetId((string)o["id"]);
            SetName((string)o["name"]);
            SetEmail((string)o["email"]);
            SetCreatedAt((DateTime?)o["created_at"]);
            SetAvatar((string)o["avatar"]);
            SetPhone((string)o["phone"]);
            SetPrivate_token((string)o["private_token"]);
        }

        public static void Logout()
        {
            SetLogin(false);
            SetCount(null);
            SetId(null);
            SetName(null);
            SetEmail(null);
            SetCreatedAt(null);
            SetAvatar(null);
            SetPhone(null);
            SetPrivate_token(null);

            Clear();
        }

        private static void Save()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains("login"))
            {
                settings.Add("login", login);
            }
            else
            {
                settings["login"] = login;
            }

            if (!settings.Contains("count"))
            {
                settings.Add("count", count);
            }
            else
            {
                settings["count"] = count;
            }

            if (!settings.Contains("id"))
            {
                settings.Add("id", id);
            }
            else
            {
                settings["id"] = id;
            }

            if (!settings.Contains("name"))
            {
                settings.Add("name", name);
            }
            else
            {
                settings["name"] = name;
            }

            if (!settings.Contains("email"))
            {
                settings.Add("email", email);
            }
            else
            {
                settings["email"] = email;
            }

            if (!settings.Contains("created_at"))
            {
                settings.Add("created_at", created_at);
            }
            else
            {
                settings["created_at"] = created_at;
            }

            if (!settings.Contains("avatar"))
            {
                settings.Add("avatar", avatar);
            }
            else
            {
                settings["avatar"] = avatar;
            }

            if (!settings.Contains("phone"))
            {
                settings.Add("phone", phone);
            }
            else
            {
                settings["phone"] = phone;
            }

            if (!settings.Contains("private_token"))
            {
                settings.Add("private_token", private_token);
            }
            else
            {
                settings["private_token"] = private_token;
            }

            settings.Save();
        }

        private static void Clear()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("login"))
            {
                IsolatedStorageSettings.ApplicationSettings["login"] = false;
            }

            if (settings.Contains("count"))
            {
                settings.Remove("count");
            }

            if (settings.Contains("id"))
            {
                settings.Remove("id");
            }

            if (settings.Contains("name"))
            {
                settings.Remove("name");
            }

            if (settings.Contains("email"))
            {
                settings.Remove("email");
            }

            if (settings.Contains("created_at"))
            {
                settings.Remove("created_at");
            }

            if (settings.Contains("avatar"))
            {
                settings.Remove("avatar");
            }

            if (settings.Contains("phone"))
            {
                settings.Remove("phone");
            }

            if (settings.Contains("private_token"))
            {
                settings.Remove("private_token");
            }

            settings.Save();
        }

        public static bool Read()
        {
            try
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                count = (int?)settings["count"];
                login = (bool?)settings["login"];
                id = (string)settings["id"];
                name = (string)settings["name"];
                email = (string)settings["email"];
                created_at = (DateTime?)settings["created_at"];
                avatar = (string)settings["avatar"];
                phone = (string)settings["phone"];
                private_token = (string)settings["private_token"];
                return true;
            }
            catch (Exception)
            {
                Logout();
                return false;
            }
        }

    }

    class Storage
    {
        private static Object thisLock = new Object();

        public static bool hehe = false;

        public static void Initials()
        {
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            if (!iso.DirectoryExists("uploads"))
            {
                iso.CreateDirectory("uploads");
            }
            if (!iso.DirectoryExists("uploads/avatar"))
            {
                iso.CreateDirectory("uploads/avatar");
            }
            if (!iso.DirectoryExists("uploads/header"))
            {
                iso.CreateDirectory("uploads/header");
            }
            if (!iso.DirectoryExists("uploads/avatar/user"))
            {
                iso.CreateDirectory("uploads/avatar/user");
            }
            if (!iso.DirectoryExists("uploads/header/user"))
            {
                iso.CreateDirectory("uploads/header/user");
            }
            if (!iso.DirectoryExists("uploads/avatar/organization"))
            {
                iso.CreateDirectory("uploads/avatar/organization");
            }
            if (!iso.DirectoryExists("assets"))
            {
                iso.CreateDirectory("assets");
            }
            if (!iso.DirectoryExists("assets/avatar"))
            {
                iso.CreateDirectory("assets/avatar");
            }
            if (!iso.DirectoryExists("assets/header"))
            {
                iso.CreateDirectory("assets/header");
            }
            if (!iso.DirectoryExists("assets/avatar/organization"))
            {
                iso.CreateDirectory("assets/avatar/organization");
            }
            if (!iso.DirectoryExists("assets/avatar/user"))
            {
                iso.CreateDirectory("assets/avatar/user");
            }
            if (!iso.DirectoryExists("assets/header/user"))
            {
                iso.CreateDirectory("assets/header/user");
            }

            iso.Dispose();

        }

        public static void SaveAvatar(string uri)
        {
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            uri = GetSmallImage(uri);
            if (iso.FileExists(uri))
            {
                App.NewViewModel.AlterAvatar(GetDefaultImage(uri));
                return;
            }
            iso.Dispose();

            string uriSource = @"https://combee.co" + uri;

            WebClient client = new WebClient();
            Uri fulluri = new Uri(uriSource);

            Object Path = (Object)uri;
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCallback);
            client.OpenReadAsync(fulluri, Path);

        }

        private static void OpenReadCallback(object sender, OpenReadCompletedEventArgs e)
        {
            lock (thisLock)
            {
                using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (iso.FileExists(e.UserState.ToString()))
                    {
                        App.NewViewModel.AlterAvatar(GetDefaultImage(e.UserState.ToString()));
                        return;
                    }
                    else
                    {
                        using (var fileStream = iso.OpenFile
                            (e.UserState.ToString(), FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            var stream = e.Result;
                            stream.CopyTo(fileStream);
                        }
                    }
                }

                App.NewViewModel.AlterAvatar(GetDefaultImage(e.UserState.ToString()));
            }
        }

        public static string GetSmallImage(string str)
        {
            int len = str.Length;
            string pre = string.Empty;
            string lst = string.Empty;
            for(int i = len - 1; i > 0; i--)
            {
                if(str[i] == '/')
                {
                    pre = str.Substring(0, i + 1);
                    lst = str.Substring(i + 1, len - i - 1);
                    break;
                }
            }

            string fnl = pre + "mobile_" + lst;
            return fnl;
        }

        public static string GetDefaultImage(string str)
        {
            str = str.Replace("mobile_", string.Empty);
            return str;
        }


        internal static System.Windows.Media.ImageSource GetImageSource(string path)
        {
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fileStream = isoFile.OpenFile(Storage.GetSmallImage(path), FileMode.Open, FileAccess.Read);
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(fileStream);
            return bitmap;
        }

        internal static Receipts FindReceipt(string id)
        {
            var query = from Receipts rpt in App.NewViewModel.myDB.ReceiptsTable
                        where rpt.PostId == id
                        select rpt;
            if (query.Count() != 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        internal static Organizations FindOrganization(string id)
        {
            var query = from Organizations orgz in App.NewViewModel.myDB.OrganizationsTable
                        where orgz.Id == id
                        select orgz;
            if (query.Count() != 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        internal static Users FindUser(string id)
        {
            var query = from user in App.NewViewModel.myDB.UsersTable
                             where user.Id == id
                             select user;
            if (query.Count() != 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }
    }
}
