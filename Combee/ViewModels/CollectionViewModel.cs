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
using Combee.ViewModels;

namespace BindingData.ViewModel
{
    public class TheViewModel : INotifyPropertyChanged
    {
        private static Object thisLock = new Object();

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

        // 当前页面组织集合
        //private ObservableCollection<Organizations> _organizationsItems;
        //public ObservableCollection<Organizations> OrganizationsItems
        //{
        //    get { return _organizationsItems; }
        //    set
        //    {
        //        _organizationsItems = value;
        //        NotifyPropertyChanged("OrganizationsItems");
        //    }
        //}

        // 当前页面优信集合
        //private ObservableCollection<Receipts> _receiptsItems;
        //public ObservableCollection<Receipts> ReceiptsItems
        //{
        //    get { return _receiptsItems; }
        //    set
        //    {
        //        _receiptsItems = value;
        //        NotifyPropertyChanged("ReceiptsItems");
        //    }
        //}

        // 当前页面人员项目.
        //private ObservableCollection<Users> _usersItems;
        //public ObservableCollection<Users> UsersItems
        //{
        //    get { return _usersItems; }
        //    set
        //    {
        //        _usersItems = value;
        //        NotifyPropertyChanged("UsersItems");
        //    }
        //}

        // 当前页面评论集合
        //private ObservableCollection<Comment> _commentItems;
        //public ObservableCollection<Comment> CommentItems
        //{
        //    get { return _commentItems; }
        //    set
        //    {
        //        _commentItems = value;
        //        NotifyPropertyChanged("CommentItems");
        //    }
        //}

        // 对本地数据库的 LINQ to SQL 的数据上下文.
        public MyDataContext myDB;

        // 类的构造方法, 创建数据上下文对象.
        public TheViewModel(string toDoDBConnectionString)
        {
            myDB = new MyDataContext(toDoDBConnectionString);

        }
        public TheViewModel() { }

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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        // 查询数据库并加载集合用于全景页面.
        public void LoadCollectionsFromDatabase()
        {
            lock (thisLock)
            {
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
                                             where o.Belong == true
                                             orderby o.Name
                                             select o;

                // 查询数据库并加载所有组织项目.
                AllOrganizationsItems = new ObservableCollection<Organizations>(OrganizationsItemsInDB);

                // 制定在数据库中对所有人员项目的查询.
                var UsersItemsInDB = from Users us in myDB.UsersTable
                                     select us;

                // 查询数据库并加载所有人员项目.
                AllUsersItems = new ObservableCollection<Users>(UsersItemsInDB);

                //ReceiptsItems = new ObservableCollection<Receipts>();
                //OrganizationsItems = new ObservableCollection<Organizations>();
                //UsersItems = new ObservableCollection<Users>();
                //CommentItems = new ObservableCollection<Comment>();
            }
        }

        #region Receipts相关方法

        // 在数据库和集合中添加一个优信项目.
        public void AddReceiptsItem(Receipts newRpt)
        {
            lock (thisLock)
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

                    int i = 0;
                    foreach (Receipts rpt in AllReceiptsItems)
                    {
                        if (rpt.CreatedAt > newRpt.CreatedAt)
                            i++;
                    }
                    // 在所有可观测集合中添加一个新的优信项目.
                    AllReceiptsItems.Insert(i, newRpt);

                }

                else
                {
                    // 从DataContext中取出该Receipts
                    Receipts theRpt = myDB.ReceiptsTable.First(r => r.Id == newRpt.Id);

                    if (!theRpt.Equals(newRpt))
                    {
                        if (theRpt.AuthorAvatar != newRpt.AuthorAvatar)
                        {
                            theRpt.AuthorAvatar = newRpt.AuthorAvatar;
                            theRpt.DisplayAvatar = @"https://combee.co" + newRpt.AuthorAvatar;
                            theRpt.IsAvatarLocal = false;
                        }
                        theRpt.AuthorName = newRpt.AuthorName;
                        theRpt.Favorited = newRpt.Favorited;
                        theRpt.Read = newRpt.Read;

                        // 在数据库中保存更改.
                        myDB.SubmitChanges();

                    }
                }
            }
            if (newRpt.IsAvatarLocal == false)
            {
                Storage.SaveAvatar(newRpt.AuthorAvatar);
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

        }

        #endregion

        #region Organizations相关方法
        // 在数据库和集合中添加一个组织项目.
        public void AddOrganizationsItem(Organizations orgz)
        {
            lock (thisLock)
            {
                var query = from o in myDB.OrganizationsTable
                            where o.Id == orgz.Id
                            select o;

                if (query.Count() == 0)
                {
                    // 在数据上下文中添加一个组织项目.
                    myDB.OrganizationsTable.InsertOnSubmit(orgz);

                    // 在数据库中保存更改.
                    myDB.SubmitChanges();

                    if (orgz.Belong == true)
                    {
                        // 在所有可观测集合中添加一个新的组织项目.
                        AllOrganizationsItems.Add(orgz);
                    }
                }

                else
                {
                    // 从DataContext中取出该Receipts
                    Organizations theOrgz = myDB.OrganizationsTable.First(r => r.Id == orgz.Id);

                    if (!theOrgz.Equals(orgz))
                    {
                        if (theOrgz.Avatar != orgz.Avatar)
                        {
                            theOrgz.Avatar = orgz.Avatar;
                            theOrgz.DisplayAvatar = orgz.Avatar;
                            theOrgz.IsAvatarLocal = false;
                        }
                        if (orgz.Bio != null)
                        {
                            theOrgz.Bio = orgz.Bio;
                        }
                        if (orgz.Header != null)
                        {
                            theOrgz.Header = orgz.Header;
                        }
                        if (orgz.JoinedAt != null)
                        {
                            theOrgz.JoinedAt = orgz.JoinedAt;
                        }
                        theOrgz.Name = orgz.Name;
                        theOrgz.Members = orgz.Members;
                        theOrgz.ParentId = orgz.ParentId;

                        // 在数据库中保存更改.
                        myDB.SubmitChanges();
                    }
                }
            }

            if (orgz.IsAvatarLocal == false)
            {
                Storage.SaveAvatar(orgz.Avatar);
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

        }

        #endregion

        #region Users相关方法

        // 在数据库和集合中添加一个人员项目.
        public void AddUsersItem(Users newUser)
        {
            lock (thisLock)
            {
                var query = from user in myDB.UsersTable
                            where user.Id == newUser.Id
                            select user;

                if (query.Count() == 0)
                {
                    // 在数据上下文中添加一个人员项目.
                    myDB.UsersTable.InsertOnSubmit(newUser);

                    // 在数据库中保存更改.
                    myDB.SubmitChanges();

                    // 在所有可观测集合中添加一个新的人员项目.
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
            }
            if (newUser.IsAvatarLocal == false)
            {
                //Task.Run(() =>
                //{
                Storage.SaveAvatar(newUser.Avatar);
                //});
            }

        }

        public void AddCommentItem(Comment newCmt)
        {
            lock (thisLock)
            {
                var query = from cmt in myDB.CommentTable
                            where cmt.Id == newCmt.Id
                            select cmt;

                if (query.Count() == 0)
                {
                    // 在数据上下文中添加一个人员项目.
                    myDB.CommentTable.InsertOnSubmit(newCmt);

                    // 在数据库中保存更改.
                    myDB.SubmitChanges();
                }

            }
            if (newCmt.IsAvatarLocal == false)
            {
                Storage.SaveAvatar(newCmt.UserAvatar);
            }

        }

        public void AlterAvatar(string avatar)
        {


            lock (thisLock)
            {
                var query_user = from user in myDB.UsersTable
                                 where user.Avatar == avatar
                                 select user;
                foreach (Users u in query_user)
                {
                    u.DisplayAvatar = avatar;
                    u.IsAvatarLocal = true;
                }

                var query_receipt = from rpt in myDB.ReceiptsTable
                                    where rpt.AuthorAvatar == avatar
                                    select rpt;

                foreach (Receipts r in query_receipt)
                {
                    r.DisplayAvatar = avatar;
                    r.IsAvatarLocal = true;
                }

                var query_organization = from orgz in myDB.OrganizationsTable
                                         where orgz.Avatar == avatar
                                         select orgz;

                foreach (Organizations o in query_organization)
                {
                    o.DisplayAvatar = avatar;
                    o.IsAvatarLocal = true;
                }

                var query_conversation = from cov in myDB.ConversationsTable
                                         where cov.LastAvatar == avatar
                                         select cov;

                foreach (Conversations c in query_conversation)
                {
                    c.DisplayAvatar = avatar;
                    c.IsAvatarLocal = true;
                }

                var query_comment = from cmt in myDB.CommentTable
                                    where cmt.UserAvatar == avatar
                                    select cmt;

                foreach (Comment c in query_comment)
                {
                    c.DisplayAvatar = avatar;
                    c.IsAvatarLocal = true;
                }

                // 在数据库中保存更改.
                myDB.SubmitChanges();
            }

        }


        #endregion

        // 在数据库和集合中添加一个私信项目.
        public void AddConversationsItem(Conversations newCov)
        {
            lock (thisLock)
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

                }

                else
                {
                    // 从DataContext中取出该Receipts
                    Conversations theCov = myDB.ConversationsTable.First(r => r.Id == newCov.Id);

                    if (!theCov.Equals(newCov))
                    {
                        if (theCov.LastAvatar != newCov.LastAvatar)
                        {
                            theCov.LastAvatar = newCov.LastAvatar;
                            theCov.DisplayAvatar = newCov.LastAvatar;
                            theCov.IsAvatarLocal = false;
                        }
                        theCov.Body = newCov.Body;
                        theCov.ParticipantsId = newCov.ParticipantsId;
                        theCov.ParticipantsName = newCov.ParticipantsName;
                        theCov.UpdatedAt = newCov.UpdatedAt;

                        // 在数据库中保存更改.
                        myDB.SubmitChanges();

                    }
                }
            }
            if (newCov.IsAvatarLocal == false)
            {
                Storage.SaveAvatar(newCov.LastAvatar);
            }
        }
    }
}