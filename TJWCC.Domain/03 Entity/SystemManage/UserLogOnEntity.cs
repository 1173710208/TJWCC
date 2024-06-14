using System;

namespace TJWCC.Domain.Entity.SystemManage
{
    public class UserLogOnEntity
    {
        public string ID { get; set; }
        public string USERID { get; set; }
        public string USERPASSWORD { get; set; }
        public string USERSECRETKEY { get; set; }
        public DateTime? ALLOWSTARTTIME { get; set; }
        public DateTime? ALLOWENDTIME { get; set; }
        public DateTime? LOCKSTARTDATE { get; set; }
        public DateTime? LOCKENDDATE { get; set; }
        public DateTime? FIRSTVISITTIME { get; set; }
        public DateTime? PREVIOUSVISITTIME { get; set; }
        public DateTime? LASTVISITTIME { get; set; }
        public DateTime? CHANGEPASSWORDDATE { get; set; }
        public bool? MULTIUSERLOGIN { get; set; }
        public int? LOGONCOUNT { get; set; }
        public bool? USERONLINE { get; set; }
        public string QUESTION { get; set; }
        public string ANSWERQUESTION { get; set; }
        public bool? CHECKIPADDRESS { get; set; }
        public string LANGUAGE { get; set; }
        public string THEME { get; set; }
    }
}
