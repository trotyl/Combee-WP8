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
using BindingData.ViewModel;

namespace Combee
{
    public partial class Umsgs : PhoneApplicationPage
    {
        public Umsgs()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string id_str = NavigationContext.QueryString["id"];
            //foreach(Umsg u in UmsgViewModel.uvm.AllUmsg)
            //{
            //    if (u.id == id_str)
            //    {
            //        TitleTextBlock.Text = u.title;
            //        ContentTextBlock.Text = u.body_html;
            //        FromTextBlock.Text = "来自: " + u.from;
            //    }
            //}
        }

    }
}