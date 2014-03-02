using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combee
{
    class UriString
    {
        public static string host = "https://combee.co/api/v1/";
        public static string rear = ".json?private_token=";

        internal static Uri GetOrganizationUri(string id)
        {
            Uri uri = new Uri(host + "organizations/" + id + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetOrganizationChildrenUri(string id)
        {
            Uri uri = new Uri(host + "organizations/" + id + "/children" + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetOrganizationMembersUri(string id)
        {
            Uri uri = new Uri(host + "organizations/" + id + "/members" + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetPostUri(string title, string body, int? time)
        {
            string uriStr = host + "posts" + rear + CurrentUser.GetPrivate_token();
            uriStr += "&title=" + title;
            uriStr += "&body_html=" + body;
            foreach (string orgz in PostState.organizations)
            {
                uriStr += "&organization_ids[]=" + orgz;
            }
            if (time != null)
            {
                uriStr += "&delayed_sms_at=" + time;
            }
            Uri uri = new Uri(uriStr);
            return uri;
        }

        internal static Uri GetUserUri(string id)
        {
            Uri uri = new Uri(host + "users" + @"/" + id + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetReceiptReadUri(string id)
        {
            Uri uri = new Uri(host + "receipts/" + id + "/read" + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetUserOrganizationsUri(string id)
        {
            Uri uri = new Uri(host + "users" + @"/" + id + @"/organizations" + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetFormUri(string id)
        {
            Uri uri = new Uri(host + "forms/" + id + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetSinglePostUri(string id)
        {
            Uri uri = new Uri(host + "posts" + @"/" + id + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetCommentsUri(string id)
        {
            Uri uri = new Uri(host + "posts/" + id + "/comments" + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetReceiptArchivedUri(string id)
        {
            Uri uri = new Uri(host + "receipts/" + id + "/archived" + rear + CurrentUser.GetPrivate_token());
            return uri;
        }

        internal static Uri GetFormCollectionUri(string id)
        {
            string uriStr = host + "forms/" + id + "/collection" + rear + CurrentUser.GetPrivate_token();
            foreach (FormItem item in Form.list)
            {
                if (item.ready || !item.required)
                {
                    if (item._type == "Field::CheckBox")
                    {
                        string[] strs = item.value.Split('_');
                        foreach (string str in strs)
                        {
                            if (str != string.Empty)
                            {
                                string itemStr = "&entities[" + item.identifier + "][]=" + str;
                                uriStr += itemStr;
                            }
                        }
                    }
                    else
                    {
                        string itemStr = "&entities[" + item.identifier + "]=" + item.value;
                        uriStr += itemStr;
                    }
                }
            }
            Uri uri = new Uri(uriStr);
            return uri;
        }

        internal static Uri GetModeUri(string mode, int i)
        {
            Uri uri = new Uri(host + mode + rear + CurrentUser.GetPrivate_token() + "&page=" + i);
            return uri;
        }

        internal static Uri GetLoginUri(string login, string password)
        {
            PostArgs arg = new PostArgs();
            arg["login"] = login;
            arg["password"] = password;
            Uri uri = new Uri(host + "session?" + arg.ToString());
            return uri;
        }
    }
}
