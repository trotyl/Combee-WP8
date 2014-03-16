using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using BindingData.Model;
using Newtonsoft.Json.Linq;
using BindingData.ViewModel;

namespace Combee
{
    public partial class Test : PhoneApplicationPage
    {
        public Test()
        {
            InitializeComponent();
        }

        public List<string> ReceiptsList = new List<string> { };

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            GetAsync("receipts");
        }

        private void GetAsync(string item)
        {
            //接收消息
            string host = "https://combee.co/api/v1/";
            string rear = ".json?private_token=FHs5LTD4MpEfrxYEfHnT";

            WebClient webClient = new WebClient();

            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Retrieved);

            webClient.DownloadStringAsync(new Uri(host + item + rear));

        }


        private void Retrieved(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JsonTextReader reader = new JsonTextReader(new StringReader(e.Result));
                bool isid = false;
                while (reader.Read())
                {
                    if (!isid && reader.Depth == 2 && reader.Value != null && reader.Value.ToString() == "id")
                    {
                        isid = true;
                    }
                    else if(isid)
                    {
                        ReceiptsList.Add(reader.Value.ToString());
                        isid = false;
                    }
                }

                foreach (string str in ReceiptsList)
                {
                    string host = "https://combee.co/api/v1/";
                    string rear = ".json?private_token=FHs5LTD4MpEfrxYEfHnT";
                    string item = "receipts";
    
                    WebClient newWebClient = new WebClient();

                    newWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedDetail);

                    Uri uri = new Uri(host + item + @"/" + str + rear);

                    newWebClient.DownloadStringAsync(uri);
                }

            }
        }

        private void RetrievedDetail(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
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
                for (int i = 0; i < o["organizations"].Count(); i++)
                {
                    organizations_id += ((string)o["organizations"][i]["id"] + "_");
                }
                //rpt.OrganizationsId = organizations_id;

                string post_id = (string)o["post"]["id"];
                rpt.PostId = post_id;

                string body = (string)o["post"]["body"];
                rpt.Body = body;

                DateTime created_at = (DateTime)o["post"]["created_at"];
                rpt.CreatedAt = created_at;

                string author_name = (string)o["post"]["author"]["name"];
                rpt.AuthorName = author_name;

                string author_id = (string)o["post"]["author"]["id"];
                rpt.AuthorId = author_id;

                App.NewViewModel.AddReceiptsItem(rpt);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            wb.NavigateToString("<html><head></head><body>Silverlight Developer Site" + "<iframe src=\"http://www.silverlight.net\" WIDTH=300 HEIGHT=200>" + "</iframe></body></html>");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

    }

}