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
using System.Text.RegularExpressions;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using System.Windows.Resources;

namespace Combee
{
    public partial class ReceiptPage : PhoneApplicationPage
    {
        public ReceiptPage()
        {
            InitializeComponent();

            this.DataContext = App.NewViewModel;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //加载正文信息
            string id = NavigationContext.QueryString["id"];

            Receipts r = Storage.FindReceipt(id);
            if (r != null)
            {
                if (TitleBlock.Text != string.Empty)
                    return;
                TitleBlock.Text = r.Title;
                FromBlock.Text = r.AuthorName;
                TimeBlock.Text = r.CreatedAt.ToString();
                userPanel.Tag = r.AuthorId;
                JArray orgzArray = JArray.Parse(r.Organizations);
                bool temp = false;
                ToBlock.Text = "To: ";
                foreach (JObject o in orgzArray)
                {
                    ToBlock.Text += (temp ? "; " : string.Empty);
                    ToBlock.Text += o["name"];
                    temp = true;
                }
                AvatarImage.Source = Storage.GetImageSource(r.AuthorAvatar);

                TextBlock ContentBlock = new TextBlock() { TextWrapping = TextWrapping.Wrap };
                Regex re_label = new Regex(@"<[^>]+>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string source = r.BodyHtml.Replace("<div>", "").Replace("</div>", "").Replace("<ol>", "").Replace("</ol>", "").Replace("&nbsp;", " ");
                Match match = re_label.Match(source);
                int startidx = 0;
                bool isBold = false, isItalics = false, isUnderline = false;
                while (match.Index >= 0 && match.Length > 0)
                {
                    ExtractText(ContentBlock, source, startidx, match.Index, re_label, isBold, isItalics, isUnderline);
                    startidx = match.Index + match.Length;
                    bool isEndTag = match.Value.Contains("/");
                    if (match.Value == "</li>" || match.Value == "<br>")
                    {
                        ContentBlock.Inlines.Add(new LineBreak());
                    }
                    else if (match.Value == "<b>" || match.Value == "</b>")
                    {
                        isBold = !isEndTag;
                    }
                    else if (match.Value == "<i>" || match.Value == "</i>" || match.Value == "<em>" || match.Value == "</em>")
                    {
                        isItalics = !isEndTag;
                    }
                    else if (match.Value == "<u>" || match.Value == "</u>")
                    {
                        isUnderline = !isEndTag;
                    }
                    match = re_label.Match(source, match.Index + 1);
                }
                ContentBlock.Inlines.Add(source.Substring(startidx));
                ContentPanel.Children.Add(ContentBlock);
                Regex re_link = new Regex(@"<a href[^>]+>[^<]+</a>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                Regex re_uri = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                match = re_link.Match(source);
                if (match.Length > 0)
                {
                    TextBlock LinkBlock = new TextBlock() { Text = "打开链接:", Margin = new Thickness(0, 10, 0, 0) };
                    ContentPanel.Children.Add(LinkBlock);
                    HyperlinkButton linkButton;
                    while (match.Length > 0)
                    {
                        Match uriMatch = re_uri.Match(match.Value);
                        linkButton = new HyperlinkButton() { Content = uriMatch.Value, Tag = uriMatch.Value, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left };
                        linkButton.Tap += linkButton_Tap;
                        ContentPanel.Children.Add(linkButton);
                        source = source.Substring(match.Index + 1);
                        match = re_link.Match(source);
                    }
                }
                else
                {
                    Match uriMatch = re_uri.Match(source);
                    if (uriMatch.Length > 0)
                    {
                        TextBlock LinkBlock = new TextBlock() { Text = "打开链接:", Margin = new Thickness(0, 10, 0, 0) };
                        ContentPanel.Children.Add(LinkBlock);
                    }
                    while (uriMatch.Length > 0)
                    {
                        HyperlinkButton linkButton;
                        linkButton = new HyperlinkButton() { Content = uriMatch.Value, Tag = uriMatch.Value, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left };
                        linkButton.Tap += linkButton_Tap;
                        ContentPanel.Children.Add(linkButton);
                        source = source.Substring(uriMatch.Index + 1);
                        uriMatch = re_link.Match(source);
                    }
                }

                JArray attachmentsArray = JArray.Parse(r.Attachments);
                if (attachmentsArray.Count > 0)
                {
                    TextBlock attachBlock = new TextBlock() { Text = "附件(" + attachmentsArray.Count + "):", Margin = new Thickness(0, 20, 0, 0) };
                    attachmentPanel.Children.Add(attachBlock);
                }
                foreach (JObject o in attachmentsArray)
                {
                    string file_name = (string)o["file_name"];
                    string file_size = (string)o["file_size"];
                    string url = (string)o["url"];
                    HyperlinkButton linkButton = new HyperlinkButton() { Content = file_name, Tag = url, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left };
                    linkButton.Tap += linkButton_Tap;
                    attachmentPanel.Children.Add(linkButton);
                }

                if (r.Read == false)
                {
                    WebClient readWebClient = new WebClient();
                    readWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(PutedRead);
                    readWebClient.UploadStringAsync(UriString.GetReceiptReadUri(r.Id), "PUT", string.Empty);
                }

                if (r.FormId != null)
                {
                    Form.id = r.FormId;
                    Form.title = r.FormTitle;
                    Form.list.Clear();

                    WebClient formWebClient = new WebClient();
                    formWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedForm);
                    formWebClient.DownloadStringAsync(UriString.GetFormUri(r.FormId));
                    TextBlock titleBlock = new TextBlock() { Text = Form.title, Margin = new Thickness(10, 10, 0, 0), FontWeight = FontWeights.Bold, FontSize = 24 };
                    formPanel.Children.Add(titleBlock);

                    //PivotItem formItem = new PivotItem() { Margin = new Thickness(12, 0, 12, 0) };
                    //TextBlock headerBlock = new TextBlock() { Text = "表单", FontSize = 50 };
                    //formItem.Header = headerBlock;
                    //formItem.ContentTemplate = this.formTemplate;
                    //pivot.Items.Add(formItem);
                }
                else
                {
                    Form.id = null;
                    Form.title = null;
                    Form.list.Clear();
                    TextBlock titleBlock = new TextBlock() { Text = "该优信不含表单内容~", Margin = new Thickness(10, 10, 0, 0), FontWeight = FontWeights.Bold, FontSize = 24 };
                    formPanel.Children.Add(titleBlock);
                }
            }

            else
            {
                WebClient newWebClient = new WebClient();
                newWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedPost);
                newWebClient.DownloadStringAsync(UriString.GetSinglePostUri(id));
            }
            App.NewViewModel.CommentItems.Clear();

            WebClient commentsWebClient = new WebClient();
            commentsWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedComments);
            commentsWebClient.DownloadStringAsync(UriString.GetCommentsUri(id));

        }


        private void linkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string desUri = (string)((HyperlinkButton)sender).Tag;
            if (desUri.Replace("http", "") == desUri)
            {
                desUri = @"https://combee.co" + desUri + "?private_token=" + CurrentUser.GetPrivate_token();
            }

            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(desUri, UriKind.Absolute);
            webBrowserTask.Show();

        }

        void ExtractText(TextBlock block, string source, int startidx, int endidx, Regex re, bool isBold, bool isItalics, bool isUnderline)
        {
            string text = source.Substring(startidx, endidx - startidx);
            text = re.Replace(text, "");
            if (!isItalics && !isBold && !isUnderline)
            {
                block.Inlines.Add(text);
            }
            else
            {
                block.Inlines.Add(new Run()
                {
                    FontWeight = (isBold ? FontWeights.Bold : FontWeights.Normal),
                    FontStyle = (isItalics ? FontStyles.Italic : FontStyles.Normal),
                    TextDecorations = (isUnderline ? TextDecorations.Underline : null),
                    Text = text
                });
            }
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

                    TextBlock b = new TextBlock() { Margin = new Thickness(10, 0, 0, 0) };
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
                    formPanel.Children.Add(p);
                }
                Border bdr = new Border() { Height = 100 };
                formPanel.Children.Add(bdr);
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
                foreach(JObject o in arr)
                {
                    Comment cm = new Comment();

                    cm.Id = (string)o["id"];
                    cm.Body = (string)o["body"];
                    cm.CreatedAt = (DateTime)o["created_at"];
                    cm.UserId = (string)o["user"]["id"];
                    cm.UserName = (string)o["user"]["name"];
                    cm.UserAvatar = (string)o["user"]["avatar"];
                    cm.DisplayAvatar = @"https://combee.co" + cm.UserAvatar;
                    cm.IsAvatarLocal = false;

                    Storage.SaveAvatar(cm.UserAvatar);
                    App.NewViewModel.CommentItems.Add(cm);
                }
                Comment cmt = new Comment();
                App.NewViewModel.CommentItems.Add(cmt);
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

            }

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
            NavigationService.Navigate(new Uri("/Combee;component/UserPage.xaml?id=" + id, UriKind.Relative));
        }

        private void DeletedButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("你确定要删除该优信咩..?", "友情提示~", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.Cancel)
            {
                return;
            }

            string id = NavigationContext.QueryString["id"];

            var query = from Receipts rpt in App.NewViewModel.myDB.ReceiptsTable
                        where rpt.PostId == id
                        select rpt;
            if (query.Count() != 0)
            {
                Receipts r = query.First();
                
                WebClient readWebClient = new WebClient();
                readWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(PutedRead);
                readWebClient.UploadStringAsync(UriString.GetReceiptArchivedUri(r.Id), "PUT", string.Empty);

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

        private void commentButton_Click(object sender, EventArgs e)
        {
            commentBox.Visibility = System.Windows.Visibility.Visible;
            if (pivot.SelectedIndex != 1)
                pivot.SelectedIndex = 1;
            ApplicationBar = ((ApplicationBar)this.Resources["cmtgAppBar"]);
        }

        private void cmtSubmitButton_Click(object sender, EventArgs e)
        {
            if (commentBox.Text == string.Empty)
            {
                MessageBox.Show("亲~内容不能为空!");
                return;
            }
            string id = NavigationContext.QueryString["id"];
            Receipts r = Storage.FindReceipt(id);

            WebClient cmtWebClient = new WebClient();
            cmtWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(Commented);
            cmtWebClient.UploadStringAsync(UriString.GetReceiptCommentUri(r.PostId, commentBox.Text), string.Empty);

        }


        private void commentBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (commentBox.Text == "请在此输入内容~")
            {
                commentBox.Text = string.Empty;
            }
        }

        private void commentBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (commentBox.Text == string.Empty)
            {
                commentBox.Text = "请在此输入内容~";
            }
        }

        private void cmtCancelButton_Click(object sender, EventArgs e)
        {
            commentBox.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar = ((ApplicationBar)this.Resources["cmtAppBar"]);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (Form.id == null)
                return;
            ((ApplicationBarIconButton)sender).IsEnabled = false;
            foreach (FormItem item in Form.list)
            {
                if (!item.ready && item.required)
                {
                    MessageBox.Show("表单未填写完整");
                    return;
                }
            }
            WebClient submitWebClient = new WebClient();
            submitWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(Submitted);
            submitWebClient.UploadStringAsync(UriString.GetFormCollectionUri(Form.id), "POST", string.Empty);

        }

        private void Commented(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {

                Comment cmt = new Comment();
                cmt.Body = commentBox.Text;
                cmt.CreatedAt = DateTime.Now;
                cmt.DisplayAvatar = CurrentUser.GetAvatar();
                cmt.Id = null;
                cmt.IsAvatarLocal = true;
                cmt.UserAvatar = CurrentUser.GetAvatar();
                cmt.UserId = CurrentUser.GetId();
                cmt.UserName = CurrentUser.GetName();
                App.NewViewModel.CommentItems.Insert(0, cmt);

                commentBox.Visibility = System.Windows.Visibility.Collapsed;
                commentBox.Text = string.Empty;
                ApplicationBar = ((ApplicationBar)this.Resources["cmtAppBar"]);
            }
        }

        private void Submitted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message);
                MessageBox.Show("提交失败!");
            }
        }

        private void attachmentImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ((Image)sender).Visibility = System.Windows.Visibility.Collapsed;
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    ApplicationBar = ((ApplicationBar)this.Resources["cntAppBar"]);
                    break;
                case 1:
                    if (commentBox.Visibility == System.Windows.Visibility.Collapsed)
                        ApplicationBar = ((ApplicationBar)this.Resources["cmtAppBar"]);
                    else
                        ApplicationBar = ((ApplicationBar)this.Resources["cmtgAppBar"]);
                    break;
                case 2:
                    ApplicationBar = ((ApplicationBar)this.Resources["frmAppBar"]);
                    break;
            }
        }

        private void userPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string id = ((StackPanel)sender).Tag.ToString();
            NavigationService.Navigate(new Uri("/Combee;component/UserPage.xaml?id=" + id, UriKind.Relative));
        }

    }
}