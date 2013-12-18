using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

//数据模型的指令
using BindingData.Model;
using Combee;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace BindingData.ViewModel
{
    public class TheViewModel : INotifyPropertyChanged
    {
        // 所有优信项目.
        private ObservableCollection<Receipts> _allReceiptsItems;
        public ObservableCollection<Receipts> AllReceiptsItems
        {
            get { return _allReceiptsItems; }
            set
            {
                _allReceiptsItems = value;
                NotifyPropertyChanged("AllReceiptsItems");
            }
        }

        //// 当前用户优信项目.
        //private ObservableCollection<Receipts> _theReceiptsItems;
        //public ObservableCollection<Receipts> TheReceiptsItems
        //{
        //    get { return _theReceiptsItems; }
        //    set
        //    {
        //        _theReceiptsItems = value;
        //        NotifyPropertyChanged("TheReceiptsItems");
        //    }
        //}

        // 所有组织项目.
        private ObservableCollection<Organizations> _allOrganizationsItems;
        public ObservableCollection<Organizations> AllOrganizationsItems
        {
            get { return _allOrganizationsItems; }
            set
            {
                _allOrganizationsItems = value;
                NotifyPropertyChanged("AllOrganizationsItems");
            }
        }

        //// 当前用户组织项目.
        //private ObservableCollection<Organizations> _theOrganizationsItems;
        //public ObservableCollection<Organizations> TheOrganizationsItems
        //{
        //    get { return _theOrganizationsItems; }
        //    set
        //    {
        //        _theOrganizationsItems = value;
        //        NotifyPropertyChanged("TheOrganizationsItems");
        //    }
        //}
        
        // 所有人员项目.
        private ObservableCollection<Users> _allUsersItems;
        public ObservableCollection<Users> AllUsersItems
        {
            get { return _allUsersItems; }
            set
            {
                _allUsersItems = value;
                NotifyPropertyChanged("AllUsersItems");
            }
        }

        // 所有会话项目.
        private ObservableCollection<Conversations> _allConversationsItems;
        public ObservableCollection<Conversations> AllConversationsItems
        {
            get { return _allConversationsItems; }
            set
            {
                _allConversationsItems = value;
                NotifyPropertyChanged("AllConversationsItems");
            }
        }

        // 对本地数据库的 LINQ to SQL 的数据上下文.
        public MyDataContext myDB;

        // 类的构造方法, 创建数据上下文对象.
        public TheViewModel(string toDoDBConnectionString)
        {
            myDB = new MyDataContext(toDoDBConnectionString);
        }

        // 把数据上下文中的变化写入数据库.
        public void SaveChangesToDB()
        {
            myDB.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // 用来通知程序某属性已改变.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        // 查询数据库并加载集合用于全景页面.
        public void LoadCollectionsFromDatabase()
        {
            //if (!Storage.hehe)
            //{
            //    Storage.hehe = true;

            //    // 更新优信显示时间.
            //    var ReceiptsInDB = from Receipts rpt in myDB.ReceiptsTable
            //                            orderby rpt.Id descending
            //                            select rpt;

            //    if (ReceiptsInDB.Count() != 0)
            //    {
            //        foreach (Receipts rpt in ReceiptsInDB)
            //        {
            //            if (rpt.CreatedAt.Date == DateTime.Now.Date)
            //            {
            //                rpt.DisplayTime = rpt.CreatedAt.ToShortTimeString();
            //            }
            //            else
            //            {
            //                rpt.DisplayTime = rpt.CreatedAt.ToShortDateString();
            //            }
            //        }
            //    }

            //    // 更新会话显示时间.
            //    var ConversationsInDB = from Conversations cov in myDB.ConversationsTable
            //                                 orderby cov.UpdatedAt descending
            //                                 select cov;

            //    if (ConversationsInDB.Count() != 0)
            //    {
            //        foreach (Conversations cov in ConversationsInDB)
            //        {
            //            if (cov.UpdatedAt.Date == DateTime.Now.Date)
            //            {
            //                cov.DisplayTime = cov.CreatedAt.ToShortTimeString();
            //            }
            //            else
            //            {
            //                cov.DisplayTime = cov.CreatedAt.ToShortDateString();
            //            }
            //        }
            //    }

            //}

            var ReceiptsItemsInDB = from Receipts rpt in myDB.ReceiptsTable
                                    orderby rpt.Id descending
                                    select rpt;

            // 查询数据库并加载所有优信项目.
            AllReceiptsItems = new ObservableCollection<Receipts>(ReceiptsItemsInDB);

            // 更新会话显示时间.
            var ConversationsItemsInDB = from Conversations cov in myDB.ConversationsTable
                                         orderby cov.UpdatedAt descending
                                         select cov;

            // 查询数据库并加载所有组织项目.
            AllConversationsItems = new ObservableCollection<Conversations>(ConversationsItemsInDB);

            // 制定在数据库中对所有组织项目的查询.
            var OrganizationsItemsInDB = from Organizations o in myDB.OrganizationsTable
                                         where o.InIt == true
                                         orderby o.Name
                                         select o;

            // 查询数据库并加载所有组织项目.
            AllOrganizationsItems = new ObservableCollection<Organizations>(OrganizationsItemsInDB);

            // 制定在数据库中对所有人员项目的查询.
            var UsersItemsInDB = from Users us in myDB.UsersTable
                                 select us;

            // 查询数据库并加载所有人员项目.
            AllUsersItems = new ObservableCollection<Users>(UsersItemsInDB);

        }

        #region Receipts相关方法

        // 在数据库和集合中添加一个优信项目.
        public void AddReceiptsItem(Receipts newRpt)
        {
            var query = from rpt in myDB.ReceiptsTable
                        where rpt.Id == newRpt.Id
                        select rpt;

            if (query.Count() == 0)
            {
                // 在数据上下文中添加一个优信项目.
                myDB.ReceiptsTable.InsertOnSubmit(newRpt);

                // 在数据库中保存更改.
                myDB.SubmitChanges();

                // 在所有可观测集合中添加一个新的优信项目.
                AllReceiptsItems.Add(newRpt);

                App.NewViewModel.LoadCollectionsFromDatabase();

            }

            else
            {
                // 从DataContext中取出该Receipts
                Receipts theRpt = myDB.ReceiptsTable.First(r => r.Id == newRpt.Id);

                if(!theRpt.Equals(newRpt))
                {
                    if(theRpt.AuthorAvatar != newRpt.AuthorAvatar)
                    {
                        theRpt.AuthorAvatar = newRpt.AuthorAvatar;
                        theRpt.DisplayAvatar = newRpt.AuthorAvatar;
                        theRpt.IsAvatarLocal = false;
                    }
                    theRpt.AuthorName = newRpt.AuthorName;
                    theRpt.Favorited = newRpt.Favorited;
                    theRpt.Read = newRpt.Read;

                    // 在数据库中保存更改.
                    myDB.SubmitChanges();

                    App.NewViewModel.LoadCollectionsFromDatabase();
                }
            }

            if (newRpt.IsAvatarLocal == false)
            {
                Task.Run(() =>
                {
                    StartSaveAvatar(newRpt);
                });
            }
        }
 
        // 从数据库和集合中清除一个优信项目.
        public void DeleteReceiptItem(Receipts rpt)
        {

            // 从可观测集合中清除某个优信项目.
            AllReceiptsItems.Remove(rpt);

            // 从数据上下文中清除某个优信项目.
            myDB.ReceiptsTable.DeleteOnSubmit(rpt);

            // 在数据库中保存更改.
            myDB.SubmitChanges();

            App.NewViewModel.LoadCollectionsFromDatabase();

        }

        public void StartSaveAvatar(Receipts rpt)
        {
            Storage.SaveAvatar(Storage.GetSmallImage(rpt.AuthorAvatar));

            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                // 从DataContext中取出该Receipts
                Receipts theRpt = myDB.ReceiptsTable.First(r => r.Id == rpt.Id);

                theRpt.DisplayAvatar = rpt.AuthorAvatar;
                theRpt.IsAvatarLocal = true;

                // 在数据库中保存更改.
                myDB.SubmitChanges();

                App.NewViewModel.LoadCollectionsFromDatabase();

            }));

        }

        #endregion

        #region Organizations相关方法
        // 在数据库和集合中添加一个组织项目.
        public void AddOrganizationsItem(Organizations newOrgz)
        {
            var query = from orgz in myDB.OrganizationsTable
                        where orgz.Id == newOrgz.Id
                        select orgz;

            if (query.Count() == 0)
            {
                // 在数据上下文中添加一个组织项目.
                myDB.OrganizationsTable.InsertOnSubmit(newOrgz);

                // 在数据库中保存更改.
                myDB.SubmitChanges();

                // 在所有可观测集合中添加一个新的组织项目.
                AllOrganizationsItems.Add(newOrgz);

                App.NewViewModel.LoadCollectionsFromDatabase();
            }

            else
            {
                // 从DataContext中取出该Receipts
                Organizations theOrgz = myDB.OrganizationsTable.First(r => r.Id == newOrgz.Id);

                if (!theOrgz.Equals(newOrgz))
                {
                    if (theOrgz.Avatar != newOrgz.Avatar)
                    {
                        theOrgz.Avatar = newOrgz.Avatar;
                        theOrgz.DisplayAvatar = newOrgz.Avatar;
                        theOrgz.IsAvatarLocal = false;
                    }
                    theOrgz.Name = newOrgz.Name;
                    theOrgz.Bio = newOrgz.Bio;
                    theOrgz.Members = newOrgz.Members;
                    theOrgz.ParentId = newOrgz.ParentId;

                    // 在数据库中保存更改.
                    myDB.SubmitChanges();

                    App.NewViewModel.LoadCollectionsFromDatabase();

                }
            }

            if (newOrgz.IsAvatarLocal == false)
            {
                Task.Run(() =>
                {
                    StartSaveAvatar(newOrgz);
                });
            }

        }

        // 从数据库和集合中清除一个组织项目.
        public void DeleteOrganizaitonsItem(Organizations orgzForDelete)
        {

            // 从可观测集合中清除某个优信项目.
            AllOrganizationsItems.Remove(orgzForDelete);

            // 从数据上下文中清除某个优信项目.
            myDB.OrganizationsTable.DeleteOnSubmit(orgzForDelete);

            // 在数据库中保存更改.
            myDB.SubmitChanges();

            App.NewViewModel.LoadCollectionsFromDatabase();

        }

        public void StartSaveAvatar(Organizations orgz)
        {
            Storage.SaveAvatar(Storage.GetSmallImage(orgz.Avatar));

            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                // 从DataContext中取出该Organizations
                Organizations theOrgz = myDB.OrganizationsTable.First(r => r.Id == orgz.Id);

                theOrgz.DisplayAvatar = orgz.Avatar;
                theOrgz.IsAvatarLocal = true;

                // 在数据库中保存更改.
                myDB.SubmitChanges();

                App.NewViewModel.LoadCollectionsFromDatabase();

            }));

        }

        #endregion

        #region Users相关方法

        // 在数据库和集合中添加一个人员项目.
        public void AddUsersItem(Users newUser)
        {
            var query = from user in myDB.UsersTable
                        where user.Id == newUser.Id
                        select user;

            if (query.Count() == 0)
            {
                // 在数据上下文中添加一个组织项目.
                myDB.UsersTable.InsertOnSubmit(newUser);

                // 在数据库中保存更改.
                myDB.SubmitChanges();

                // 在所有可观测集合中添加一个新的组织项目.
                AllUsersItems.Add(newUser);
            }

            else
            {
                // 从DataContext中取出该Receipts
                Users theUser = myDB.UsersTable.First(r => r.Id == newUser.Id);

                if (!theUser.Equals(newUser))
                {
                    if (theUser.Avatar != newUser.Avatar)
                    {
                        theUser.Avatar = newUser.Avatar;
                        theUser.DisplayAvatar = newUser.Avatar;
                        theUser.IsAvatarLocal = false;
                    }
                    theUser.Bio = newUser.Bio;
                    theUser.Blog = newUser.Blog;
                    theUser.Email = newUser.Email;
                    theUser.Gender = newUser.Gender;
                    theUser.Name = newUser.Name;
                    theUser.Phone = newUser.Phone;
                    theUser.Qq = newUser.Qq;
                    theUser.Uid = newUser.Uid;

                    // 在数据库中保存更改.
                    myDB.SubmitChanges();
                }
            }
            App.NewViewModel.LoadCollectionsFromDatabase();

            if (newUser.IsAvatarLocal == false)
            {
                StartSaveAvatar(newUser);
            }

        }

        public void StartSaveAvatar(Users user)
        {
            Storage.SaveAvatar(user.Avatar);

            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                // 从DataContext中取出该Receipts
                Users theUser = myDB.UsersTable.First(r => r.Id == user.Id);

                theUser.DisplayAvatar = user.Avatar;
                theUser.IsAvatarLocal = true;

                // 在数据库中保存更改.
                myDB.SubmitChanges();
            }));

        }


        #endregion

        // 在数据库和集合中添加一个私信项目.
        public void AddConversationsItem(Conversations newCov)
        {
            var query = from cov in myDB.ConversationsTable
                        where cov.Id == newCov.Id
                        select cov;

            if (query.Count() == 0)
            {
                // 在数据上下文中添加一个优信项目.
                myDB.ConversationsTable.InsertOnSubmit(newCov);

                // 在数据库中保存更改.
                myDB.SubmitChanges();

                // 在所有可观测集合中添加一个新的优信项目.
                AllConversationsItems.Add(newCov);

                App.NewViewModel.LoadCollectionsFromDatabase();

            }

            else
            {
                // 从DataContext中取出该Receipts
                Conversations theCov = myDB.ConversationsTable.First(r => r.Id == newCov.Id);

                if (!theCov.Equals(newCov))
                {
                    if (theCov.OriginatorAvatar != newCov.OriginatorAvatar)
                    {
                        theCov.OriginatorAvatar = newCov.OriginatorAvatar;
                        theCov.DisplayAvatar = newCov.OriginatorAvatar;
                        theCov.IsAvatarLocal = false;
                    }
                    theCov.Body = newCov.Body;
                    theCov.ParticipantsId = newCov.ParticipantsId;
                    theCov.ParticipantsName = newCov.ParticipantsName;
                    theCov.UpdatedAt = newCov.UpdatedAt;

                    // 在数据库中保存更改.
                    myDB.SubmitChanges();

                    App.NewViewModel.LoadCollectionsFromDatabase();

                }
            }

            if (newCov.IsAvatarLocal == false)
            {
                Task.Run(() =>
                {
                    StartSaveAvatar(newCov);
                });
            }

        }

        public void StartSaveAvatar(Conversations cov)
        {
            Storage.SaveAvatar(Storage.GetSmallImage(cov.OriginatorAvatar));
            Deployment.Current.Dispatcher.BeginInvoke(new Action(() => 
            {
                // 从DataContext中取出该Conversations
                Conversations theCov = myDB.ConversationsTable.First(r => r.Id == cov.Id);

                theCov.DisplayAvatar = cov.OriginatorAvatar;
                theCov.IsAvatarLocal = true;

                // 在数据库中保存更改.
                myDB.SubmitChanges();

                App.NewViewModel.LoadCollectionsFromDatabase();
            }));

        }

    }

}