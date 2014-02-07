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

            this.DataContext = App.NewViewModel;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //加载正文信息
            string id = NavigationContext.QueryString["id"];

            var query = from Receipts rpt in App.NewViewModel.myDB.ReceiptsTable
                        where rpt.PostId == id
                        select rpt;
            if (query.Count() != 0)
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
                rawHtml += "<html><body bgcolor=\"#34495E\"><p>";
                rawHtml += "<font color=\"#FFFFFF\">";
                //rawHtml += "";
                rawHtml += r.BodyHtml;
                rawHtml += "</p></font></body></html>";
                ContentBrowser.NavigateToString(rawHtml);

                WebClient readWebClient = new WebClient();
                Uri uri = new Uri(Json.host + "receipts/" + r.Id + "/read" + Json.rear + ThisUser.private_token);
                readWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(PutedRead);
                readWebClient.UploadStringAsync(uri, "PUT", string.Empty);

                if (r.FormId != null)
                {
                    Form.id = r.FormId;
                    Form.title = r.FormTitle;
                    Form.list.Clear();

                    WebClient formWebClient = new WebClient();
                    formWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedForm);
                    uri = new Uri(Json.host + "forms/" + r.FormId + Json.rear + ThisUser.private_token);
                    formWebClient.DownloadStringAsync(uri);
                }
            }

            else
            {
                WebClient newWebClient = new WebClient();
                newWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedPost);
                Uri uri = new Uri(Json.host + "posts" + @"/" + id + Json.rear + ThisUser.private_token);

                newWebClient.DownloadStringAsync(uri);
            }
            App.NewViewModel.CommentItems.Clear();

            WebClient commentsWebClient = new WebClient();
            commentsWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedComments);
            Uri theUri = new Uri(Json.host + "posts/" + id + "/comments" + Json.rear + ThisUser.private_token);
            commentsWebClient.DownloadStringAsync(theUri);
        }

        private void RetrievedForm(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                JObject ob = JObject.Parse(e.Result);
                for (int i = 0; i < ob["inputs"].Count(); i++)
                {
                    string id = (string)ob["inputs"][i]["id"];
                    string _type = (string)ob["inputs"][i]["_type"];
                    string label = (string)ob["inputs"][i]["label"];
                    string help_text = (string)ob["inputs"][i]["help_text"];
                    bool required = (bool)ob["inputs"][i]["required"];
                    string identifier = (string)ob["inputs"][i]["identifier"];
                    string position = (string)ob["inputs"][i]["position"];

                    FormItem item = new FormItem();
                    item.id = id;
                    item._type = _type;
                    item.required = required;
                    item.identifier = identifier;
                    item.label = label;
                    item.ready = false;
                    item.value = string.Empty;
                    item.number = false;

                    StackPanel p = new StackPanel();

                    TextBlock b = new TextBlock();
                    if (help_text == string.Empty)
                    {
                        b.Text = label;
                    }
                    else
                    {
                        b.Text = label + "(" + help_text + ")";
                    }
                    b.Text += required ? "*" : "";
                    p.Children.Add(b);
                    
                    switch (_type)
                    {
                        case "Field::TextField":
                            {
                                TextBox c = new TextBox();
                                c.Text = (string)ob["inputs"][i]["default_value"];
                                if(c.Text != string.Empty)
                                {
                                    item.ready = true;
                                }
                                item.value = c.Text;
                                c.Tag = identifier;
                                c.TextChanged += TextBox_TextChanged;
                                p.Children.Add(c);
                                break;
                            }
                        case "Field::TextArea":
                            {
                                TextBox c = new TextBox();
                                c.Text = (string)ob["inputs"][i]["default_value"];
                                if (c.Text != string.Empty)
                                {
                                    item.ready = true;
                                }
                                item.value = c.Text;
                                c.Tag = identifier;
                                c.TextChanged += TextBox_TextChanged;
                                p.Children.Add(c);
                                break;
                            }
                        case "Field::RadioButton":
                            {
                                StackPanel c = new StackPanel();
                                for (int j = 0; j < ob["inputs"][i]["options"].Count(); j++)
                                {
                                    RadioButton a = new RadioButton();
                                    a.Content = (string)ob["inputs"][i]["options"][j]["value"];
                                    a.IsChecked = (bool)ob["inputs"][i]["options"][j]["default_selected"];
                                    a.GroupName = identifier;
                                    a.Tag = (string)ob["inputs"][i]["options"][j]["id"];
                                    a.Checked += RadioButton_Checked;
                                    if ((bool)ob["inputs"][i]["options"][j]["default_selected"])
                                    {
                                        item.ready = true;
                                        item.value = (string)ob["inputs"][i]["options"][j]["id"];
                                    }
                                    c.Children.Add(a);
                                }
                                c.Tag = identifier;
                                p.Children.Add(c);
                                break;
                            }
                        case "Field::CheckBox":
                            {
                                StackPanel c = new StackPanel();
                                for (int j = 0; j < ob["inputs"][i]["options"].Count(); j++)
                                {
                                    CheckBox a = new CheckBox();
                                    a.Content = (string)ob["inputs"][i]["options"][j]["value"];
                                    a.IsChecked = (bool)ob["inputs"][i]["options"][j]["default_selected"];
                                    a.Tag = (string)ob["inputs"][i]["options"][j]["id"];
                                    a.Checked += CheckBox_Checked;
                                    a.Unchecked += CheckBox_Unchecked;
                                    if ((bool)ob["inputs"][i]["options"][j]["default_selected"])
                                    {
                                        item.value += (string)ob["inputs"][i]["options"][j]["id"] + "_";
                                    }
                                    c.Children.Add(a);
                                }
                                item.ready = true;
                                c.Tag = identifier;
                                p.Children.Add(c);
                                break;
                            }
                        case "Field::NumberField":
                            {
                                TextBox c = new TextBox();
                                c.Text = (string)ob["inputs"][i]["default_value"];
                                c.Tag = identifier;
                                c.TextChanged += TextBox_TextChanged;
                                if (c.Text != string.Empty)
                                {
                                    item.ready = true;
                                }
                                item.value = c.Text;
                                item.number = true;
                                p.Children.Add(c);
                                break;
                            }
                    }
                    Form.list.Add(item);
                    FormPanel.Children.Add(p);
                }
            }
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
                for (int i = arr.Count() - 1; i >= 0; i--)
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
                rawHtml += "<html><body bgcolor=\"#34495E\"><p>";
                rawHtml += "<font color=\"#FFFFFF\">";
                rawHtml += (string)o["body_html"];
                rawHtml += "</p></font></body></html>";
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

        private void DeletedButton_Click(object sender, EventArgs e)
        {
            string id = NavigationContext.QueryString["id"];

            var query = from Receipts rpt in App.NewViewModel.myDB.ReceiptsTable
                        where rpt.PostId == id
                        select rpt;
            if (query.Count() != 0)
            {
                Receipts r = query.First();
                
                WebClient readWebClient = new WebClient();
                Uri uri = new Uri(Json.host + "receipts/" + r.Id + "/archived" + Json.rear + ThisUser.private_token);
                readWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(PutedRead);
                readWebClient.UploadStringAsync(uri, "PUT", string.Empty);

                App.NewViewModel.DeleteReceiptItem(r);
                NavigationService.GoBack();
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            FormItem item = Form.list.Find(
                delegate(FormItem it)
                {
                    return it.identifier == (string)box.Tag;
                });
            item.value = box.Text;

            if (box.Text != string.Empty)
            {
                item.ready = true;
            }
            else
            {
                item.ready = false;
            }

            if (item.number)
            {
                int res;
                bool temp = int.TryParse(box.Text, out res);
                item.ready = item.ready & temp;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            FormItem item = Form.list.Find(
                delegate(FormItem it)
                {
                    return it.identifier == (string)box.Tag;
                });
            item.value = box.Text;
            
            if (box.Text != string.Empty)
            {
                item.ready = true;
            }
            else
            {
                item.ready = false;
            }

            if(item.number)
            {
                int res;
                bool temp = int.TryParse(box.Text, out res);
                item.ready = item.ready & temp;
            }

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton bt = (RadioButton)sender;
            StackPanel tp = (StackPanel)(bt.Parent);
            FormItem item = Form.list.Find(
                delegate(FormItem it)
                {
                    return it.identifier == (string)tp.Tag;
                });
            item.value = (string)bt.Tag;
            item.ready = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            StackPanel tp = (StackPanel)(cb.Parent);
            FormItem item = Form.list.Find(
                delegate(FormItem it)
                {
                    return it.identifier == (string)tp.Tag;
                });
            item.value += (string)cb.Tag + "_";
            item.ready = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            StackPanel tp = (StackPanel)(cb.Parent);
            FormItem item = Form.list.Find(
                delegate(FormItem it)
                {
                    return it.identifier == (string)tp.Tag;
                });
            item.value.Replace((string)cb.Tag + "_", string.Empty);

        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            string uriStr = Json.host + "forms/" + Form.id + "/collection" + Json.rear + ThisUser.private_token;
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
                else
                {
                    MessageBox.Show("表单未填写完整");
                    break;
                }
            }
            WebClient submitWebClient = new WebClient();
            Uri uri = new Uri(uriStr);
            submitWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(Submitted);
            submitWebClient.UploadStringAsync(uri, "POST", string.Empty);

        }

        private void Submitted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                
            }
        }

    }
}