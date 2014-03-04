using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BindingData.ViewModel;

namespace BindingData.Model
{
    [Table]
    public class Receipts : INotifyPropertyChanged, INotifyPropertyChanging
    {
        //版本号,它说有用的,虽然我也不知道有什么用
        [Column(IsVersion = true)]
        private Binary _version;

        //优信ID
        private string _id;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        //优信是否已读
        private bool _read;

        [Column]
        public bool Read
        {
            get { return _read; }
            set
            {
                if (_read != value)
                {
                    NotifyPropertyChanging("Read");
                    _read = value;
                    NotifyPropertyChanged("Read");
                }
            }
        }

        //优信是否收藏
        private bool _favorited;

        [Column]
        public bool Favorited
        {
            get { return _favorited; }
            set
            {
                if (_favorited != value)
                {
                    NotifyPropertyChanging("Favorited");
                    _favorited = value;
                    NotifyPropertyChanged("Favorited");
                }
            }
        }

        //优信是否为本人发送
        private bool _origin;

        [Column]
        public bool Origin
        {
            get { return _origin; }
            set
            {
                if (_origin != value)
                {
                    NotifyPropertyChanging("Origin");
                    _origin = value;
                    NotifyPropertyChanged("Origin");
                }
            }
        }

        //表单ID
        private string _formId;

        [Column]
        public string FormId
        {
            get { return _formId; }
            set
            {
                if (_formId != value)
                {
                    NotifyPropertyChanging("FormId");
                    _formId = value;
                    NotifyPropertyChanged("FormId");
                }
            }
        }

        //表单标题
        private string _formTitle;

        [Column]
        public string FormTitle
        {
            get { return _formTitle; }
            set
            {
                if (_formTitle != value)
                {
                    NotifyPropertyChanging("FormTitle");
                    _formTitle = value;
                    NotifyPropertyChanged("FormTitle");
                }
            }
        }
        
        //优信标题
        private string _title;

        [Column]
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    NotifyPropertyChanging("Title");
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        //优信所涉及组织ID集合
        private string _organizationsId;

        [Column]
        public string OrganizationsId
        {
            get { return _organizationsId; }
            set
            {
                if (_organizationsId != value)
                {
                    NotifyPropertyChanging("OrganizationsId");
                    _organizationsId = value;
                    NotifyPropertyChanged("OrganizationsId");
                }
            }
        }

        //优信所涉及组织ID集合
        private string _postId;

        [Column]
        public string PostId
        {
            get { return _postId; }
            set
            {
                if (_organizationsId != value)
                {
                    NotifyPropertyChanging("PostId");
                    _postId = value;
                    NotifyPropertyChanged("PostId");
                }
            }
        }

        //优信发件人姓名
        private string _authorName;

        [Column]
        public string AuthorName
        {
            get { return _authorName; }
            set
            {
                if (_authorName != value)
                {
                    NotifyPropertyChanging("AuthorName");
                    _authorName = value;
                    NotifyPropertyChanged("AuthorName");
                }
            }
        }

        //优信发件人ID
        private string _authorId;

        [Column]
        public string AuthorId
        {
            get { return _authorId; }
            set
            {
                if (_authorId != value)
                {
                    NotifyPropertyChanging("AuthorId");
                    _authorId = value;
                    NotifyPropertyChanged("AuthorId");
                }
            }
        }

        //优信发件人头像
        private string _authorAvatar;

        [Column]
        public string AuthorAvatar
        {
            get { return _authorAvatar; }
            set
            {
                if (_authorAvatar != value)
                {
                    NotifyPropertyChanging("AuthorAvatar");
                    _authorAvatar = value;
                    NotifyPropertyChanged("AuthorAvatar");
                }
            }
        }

        //优信文本
        private string _body;

        [Column]
        public string Body
        {
            get { return _body; }
            set
            {
                if (_body != value)
                {
                    NotifyPropertyChanging("Body");
                    _body = value;
                    NotifyPropertyChanged("Body");
                }
            }
        }

        //优信文本
        private string _bodyHtml;

        [Column]
        public string BodyHtml
        {
            get { return _bodyHtml; }
            set
            {
                if (_bodyHtml != value)
                {
                    NotifyPropertyChanging("BodyHtml");
                    _bodyHtml = value;
                    NotifyPropertyChanged("BodyHtml");
                }
            }
        }

        //发送时间
        private DateTime? _createdAt;

        [Column]
        public DateTime? CreatedAt
        {
            get { return _createdAt; }
            set
            {
                if (_createdAt != value)
                {
                    NotifyPropertyChanging("CreatedAt");
                    _createdAt = value;
                    NotifyPropertyChanged("CreatedAt");
                }
            }
        }

        //会话显示图像
        private string _displayAvatar;

        [Column]
        public string DisplayAvatar
        {
            get { return _displayAvatar; }
            set
            {
                if (_displayAvatar != value)
                {
                    NotifyPropertyChanging("DisplayAvatar");
                    _displayAvatar = value;
                    NotifyPropertyChanged("DisplayAvatar");
                }
            }
        }

        //头像是否已存于本地
        private bool _isAvatarLocal;

        [Column]
        public bool IsAvatarLocal
        {
            get { return _isAvatarLocal; }
            set
            {
                if (_isAvatarLocal != value)
                {
                    NotifyPropertyChanging("IsAvatarLocal");
                    _isAvatarLocal = value;
                    NotifyPropertyChanged("IsAvatarLocal");
                }
            }
        }
        
        // 更改跟踪相关,其实我也看不太懂
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // 用来提示属性更改
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // 用来提示属性即将更改
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    [Table]
    public class Users : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // 用户ID(登录名)
        private string _id;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        // 用户电话
        private string _phone;

        [Column]
        public string Phone
        {
            get { return _phone; }
            set
            {
                NotifyPropertyChanging("Phone");
                _phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        // 用户电话
        private string _organizations;

        [Column]
        public string organizations
        {
            get { return _organizations; }
            set
            {
                NotifyPropertyChanging("organizations");
                _organizations = value;
                NotifyPropertyChanged("organizations");
            }
        }

        // 用户头像
        private string _avatar;

        [Column]
        public string Avatar
        {
            get { return _avatar; }
            set
            {
                NotifyPropertyChanging("Avatar");
                _avatar = value;
                NotifyPropertyChanged("Avatar");
            }
        }

        // 用户显示头像
        private string _displayAvatar;

        [Column]
        public string DisplayAvatar
        {
            get { return _displayAvatar; }
            set
            {
                NotifyPropertyChanging("DisplayAvatar");
                _displayAvatar = value;
                NotifyPropertyChanged("DisplayAvatar");
            }
        }

        //头像是否已存于本地
        private bool _isAvatarLocal;

        [Column]
        public bool IsAvatarLocal
        {
            get { return _isAvatarLocal; }
            set
            {
                if (_isAvatarLocal != value)
                {
                    NotifyPropertyChanging("IsAvatarLocal");
                    _isAvatarLocal = value;
                    NotifyPropertyChanged("IsAvatarLocal");
                }
            }
        }

        // 用户背景图
        private string _header;

        [Column]
        public string Header
        {
            get { return _header; }
            set
            {
                NotifyPropertyChanging("Header");
                _header = value;
                NotifyPropertyChanged("Header");
            }
        }

        // 用户名称
        private string _name;

        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        // 用户座右铭
        private string _bio;

        [Column]
        public string Bio
        {
            get { return _bio; }
            set
            {
                NotifyPropertyChanging("Bio");
                _bio = value;
                NotifyPropertyChanged("Bio");
            }
        }

        // 用户性别
        private string _gender;

        [Column]
        public string Gender
        {
            get { return _gender; }
            set
            {
                NotifyPropertyChanging("Gender");
                _gender = value;
                NotifyPropertyChanged("Gender");
            }
        }

        // 用户QQ号
        private string _qq;

        [Column]
        public string Qq
        {
            get { return _qq; }
            set
            {
                NotifyPropertyChanging("Qq");
                _qq = value;
                NotifyPropertyChanged("Qq");
            }
        }

        // 用户博客地址
        private string _blog;

        [Column]
        public string Blog
        {
            get { return _blog; }
            set
            {
                NotifyPropertyChanging("Blog");
                _blog = value;
                NotifyPropertyChanged("Blog");
            }
        }

        // 用户ID号(学号/工号)
        private string _uid;

        [Column]
        public string Uid
        {
            get { return _uid; }
            set
            {
                NotifyPropertyChanging("Uid");
                _uid = value;
                NotifyPropertyChanged("Uid");
            }
        }

        // 用户电子邮件
        private string _email;

        [Column]
        public string Email
        {
            get { return _email; }
            set
            {
                NotifyPropertyChanging("Email");
                _email = value;
                NotifyPropertyChanged("Email");
            }
        }

        // 用户创建时间
        private DateTime? _createdAt;

        [Column]
        public DateTime? CreatedAt
        {
            get { return _createdAt; }
            set
            {
                NotifyPropertyChanging("CreatedAt");
                _createdAt = value;
                NotifyPropertyChanged("CreatedAt");
            }
        }


        //
        // TODO: Add columns and associations, as applicable, here.
        //

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    [Table]
    public class Organizations : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // 组织ID
        private string _id;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        // 组织名称
        private string _name;

        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        // 组织创建时间
        private DateTime? _createdAt;

        [Column]
        public DateTime? CreatedAt
        {
            get { return _createdAt; }
            set
            {
                NotifyPropertyChanging("CreatedAt");
                _createdAt = value;
                NotifyPropertyChanged("CreatedAt");
            }
        }

        // 组织头像
        private string _avatar;

        [Column]
        public string Avatar
        {
            get { return _avatar; }
            set
            {
                NotifyPropertyChanging("Avatar");
                _avatar = value;
                NotifyPropertyChanged("Avatar");
            }
        }

        // 组织来自人员
        private bool _belong;

        [Column]
        public bool Belong
        {
            get { return _belong; }
            set
            {
                NotifyPropertyChanging("Belong");
                _belong = value;
                NotifyPropertyChanged("Belong");
            }
        }

        // 组织的上级组织
        private string _parentId;

        [Column]
        public string ParentId
        {
            get { return _parentId; }
            set
            {
                NotifyPropertyChanging("ParentId");
                _parentId = value;
                NotifyPropertyChanged("ParentId");
            }
        }

        // 组织人员数
        private string _members;

        [Column]
        public string Members
        {
            get { return _members; }
            set
            {
                NotifyPropertyChanging("MemberNumber");
                _members = value;
                NotifyPropertyChanged("MemberNumber");
            }
        }
        
        // 组织座右铭
        private string _bio;

        [Column]
        public string Bio
        {
            get { return _bio; }
            set
            {
                NotifyPropertyChanging("Bio");
                _bio = value;
                NotifyPropertyChanged("Bio");
            }
        }

        // 组织背景图
        private string _header;

        [Column]
        public string Header
        {
            get { return _header; }
            set
            {
                NotifyPropertyChanging("Header");
                _header = value;
                NotifyPropertyChanged("Header");
            }
        }

        // 组织的用户加入时间
        private DateTime? _joinedAt;

        [Column]
        public DateTime? JoinedAt
        {
            get { return _joinedAt; }
            set
            {
                NotifyPropertyChanging("JoinedAt");
                _joinedAt = value;
                NotifyPropertyChanged("JoinedAt");
            }
        }

        //组织显示图像
        private string _displayAvatar;

        [Column]
        public string DisplayAvatar
        {
            get { return _displayAvatar; }
            set
            {
                if (_displayAvatar != value)
                {
                    NotifyPropertyChanging("DisplayAvatar");
                    _displayAvatar = value;
                    NotifyPropertyChanged("DisplayAvatar");
                }
            }
        }

        //头像是否已存于本地
        private bool _isAvatarLocal;

        [Column]
        public bool IsAvatarLocal
        {
            get { return _isAvatarLocal; }
            set
            {
                if (_isAvatarLocal != value)
                {
                    NotifyPropertyChanging("IsAvatarLocal");
                    _isAvatarLocal = value;
                    NotifyPropertyChanged("IsAvatarLocal");
                }
            }
        }
       
        //
        // TODO: Add columns and associations, as applicable, here.
        //

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // 用来提示属性更改
        private void NotifyPropertyChanged(string propertyName)
        {

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // 用来提示属性即将更改
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    [Table]
    public class Conversations : INotifyPropertyChanged, INotifyPropertyChanging
    {
        //版本号,它说有用的,虽然我也不知道有什么用
        [Column(IsVersion = true)]
        private Binary _version;

        //会话ID
        private string _id;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        //会话创建时间
        private DateTime? _createdAt;

        [Column]
        public DateTime? CreatedAt
        {
            get { return _createdAt; }
            set
            {
                if (_createdAt != value)
                {
                    NotifyPropertyChanging("CreatedAt");
                    _createdAt = value;
                    NotifyPropertyChanged("CreatedAt");
                }
            }
        }

        //会话更新时间
        private DateTime? _updatedAt;

        [Column]
        public DateTime? UpdatedAt
        {
            get { return _updatedAt; }
            set
            {
                if (_updatedAt != value)
                {
                    NotifyPropertyChanging("UpdatedAt");
                    _updatedAt = value;
                    NotifyPropertyChanged("UpdatedAt");
                }
            }
        }

        //会话所涉及人员ID集合
        private string _participantsId;

        [Column]
        public string ParticipantsId
        {
            get { return _participantsId; }
            set
            {
                if (_participantsId != value)
                {
                    NotifyPropertyChanging("ParticipantsId");
                    _participantsId = value;
                    NotifyPropertyChanged("ParticipantsId");
                }
            }
        }

        //会话更新人员名称
        private string _lastName;

        [Column]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    NotifyPropertyChanging("LastName");
                    _lastName = value;
                    NotifyPropertyChanged("LastName");
                }
            }
        }

        //会话显示图像
        private string _displayAvatar;

        [Column]
        public string DisplayAvatar
        {
            get { return _displayAvatar; }
            set
            {
                if (_displayAvatar != value)
                {
                    NotifyPropertyChanging("DisplayAvatar");
                    _displayAvatar = value;
                    NotifyPropertyChanged("DisplayAvatar");
                }
            }
        }

        //会话所涉及人员姓名集合
        private string _participantsName;

        [Column]
        public string ParticipantsName
        {
            get { return _participantsName; }
            set
            {
                if (_participantsName != value)
                {
                    NotifyPropertyChanging("ParticipantsName");
                    _participantsName = value;
                    NotifyPropertyChanged("ParticipantsName");
                }
            }
        }

        //会话最后人头像
        private string _lastAvatar;

        [Column]
        public string LastAvatar
        {
            get { return _lastAvatar; }
            set
            {
                if (_lastAvatar != value)
                {
                    NotifyPropertyChanging("LastAvatar");
                    _lastAvatar = value;
                    NotifyPropertyChanged("LastAvatar");
                }
            }
        }

        //会话发起人头像
        private string _originatorAvatar;

        [Column]
        public string OriginatorAvatar
        {
            get { return _originatorAvatar; }
            set
            {
                if (_originatorAvatar != value)
                {
                    NotifyPropertyChanging("OriginatorAvatar");
                    _originatorAvatar = value;
                    NotifyPropertyChanged("OriginatorAvatar");
                }
            }
        }

        //会话发起人ID
        private string _originatorId;

        [Column]
        public string OriginatorId
        {
            get { return _originatorId; }
            set
            {
                if (_originatorId != value)
                {
                    NotifyPropertyChanging("OriginatorId");
                    _originatorId = value;
                    NotifyPropertyChanged("OriginatorId");
                }
            }
        }

        //会话最后项文本
        private string _body;

        [Column]
        public string Body
        {
            get { return _body; }
            set
            {
                if (_body != value)
                {
                    NotifyPropertyChanging("Body");
                    _body = value;
                    NotifyPropertyChanged("Body");
                }
            }
        }

        //头像是否已存于本地
        private bool _isAvatarLocal;

        [Column]
        public bool IsAvatarLocal
        {
            get { return _isAvatarLocal; }
            set
            {
                if (_isAvatarLocal != value)
                {
                    NotifyPropertyChanging("IsAvatarLocal");
                    _isAvatarLocal = value;
                    NotifyPropertyChanged("IsAvatarLocal");
                }
            }
        }

        // 更改跟踪相关,其实我也看不太懂
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // 用来提示属性更改
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // 用来提示属性即将更改
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    [Table]
    public class Comment : INotifyPropertyChanged, INotifyPropertyChanging
    {
        //版本号,它说有用的,虽然我也不知道有什么用
        [Column(IsVersion = true)]
        private Binary _version;

        //评论ID
        private string _id;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        //评论创建时间
        private DateTime? _createdAt;

        [Column]
        public DateTime? CreatedAt
        {
            get { return _createdAt; }
            set
            {
                if (_createdAt != value)
                {
                    NotifyPropertyChanging("CreatedAt");
                    _createdAt = value;
                    NotifyPropertyChanged("CreatedAt");
                }
            }
        }

        //评论显示图像
        private string _displayAvatar;

        [Column]
        public string DisplayAvatar
        {
            get { return _displayAvatar; }
            set
            {
                if (_displayAvatar != value)
                {
                    NotifyPropertyChanging("DisplayAvatar");
                    _displayAvatar = value;
                    NotifyPropertyChanged("DisplayAvatar");
                }
            }
        }

        //评论人员姓名
        private string _userName;

        [Column]
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    NotifyPropertyChanging("UserName");
                    _userName = value;
                    NotifyPropertyChanged("UserName");
                }
            }
        }

        //评论人员头像
        private string _userAvatar;

        [Column]
        public string UserAvatar
        {
            get { return _userAvatar; }
            set
            {
                if (_userAvatar != value)
                {
                    NotifyPropertyChanging("UserAvatar");
                    _userAvatar = value;
                    NotifyPropertyChanged("UserAvatar");
                }
            }
        }

        //评论人员ID
        private string _userId;

        [Column]
        public string UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    NotifyPropertyChanging("UserId");
                    _userId = value;
                    NotifyPropertyChanged("UserId");
                }
            }
        }

        //评论文本
        private string _body;

        [Column]
        public string Body
        {
            get { return _body; }
            set
            {
                if (_body != value)
                {
                    NotifyPropertyChanging("Body");
                    _body = value;
                    NotifyPropertyChanged("Body");
                }
            }
        }

        //头像是否已存于本地
        private bool _isAvatarLocal;

        [Column]
        public bool IsAvatarLocal
        {
            get { return _isAvatarLocal; }
            set
            {
                if (_isAvatarLocal != value)
                {
                    NotifyPropertyChanging("IsAvatarLocal");
                    _isAvatarLocal = value;
                    NotifyPropertyChanged("IsAvatarLocal");
                }
            }
        }

        // 更改跟踪相关,其实我也看不太懂
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // 用来提示属性更改
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // 用来提示属性即将更改
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class MyDataContext : DataContext
    {
        // 传递连接字符串给base类
        public MyDataContext(string connectionString)
            : base(connectionString)
        { }

        // 给Receipts项指定一个表
        public Table<Receipts> ReceiptsTable;

        // 给Organizations项指定一个表
        public Table<Organizations> OrganizationsTable;

        // 给People项指定一个表
        public Table<Users> UsersTable;

        // 给Conversations项指定一个表
        public Table<Conversations> ConversationsTable;

        // 给Comment项指定一个表
        public Table<Comment> CommentTable;

    }
}
