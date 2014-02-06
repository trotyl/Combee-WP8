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

namespace Combee
{
    class ThisUser
    {
        public static string name = string.Empty;
        public static string id = string.Empty;
        public static string email = string.Empty;
        public static DateTime created_at = DateTime.Now;
        public static string avatar = string.Empty;
        public static string phone = string.Empty;
        public static string private_token = string.Empty;

        public static void SetValue(string _id, string _name, string _email, DateTime _created_at, string _avatar, string _phone, string _private_token)
        {
            id = _id;
            name = _name;
            email = _email;
            created_at = _created_at;
            avatar = _avatar;
            phone = _phone;
            private_token = _private_token;
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
    }
}
