using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class UserApp
    {
        private IUserRepository service = new UserRepository();
        private UserLogOnApp userLogOnApp = new UserLogOnApp();

        public List<UserEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<UserEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.ACCOUNT.Contains(keyword));
                expression = expression.Or(t => t.REALNAME.Contains(keyword));
                expression = expression.Or(t => t.MOBILEPHONE.Contains(keyword));
            }
            expression = expression.And(t => t.ACCOUNT != "admin");
            return service.FindList(expression, pagination);
        }
        public List<UserEntity> GetList()
        {
            var expression = ExtLinq.True<UserEntity>();
            expression = expression.And(t => t.ACCOUNT != "admin");
            return service.IQueryable(expression).ToList();
        }
        public UserEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
        }
        public void SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                userEntity.Modify(keyValue);
            }
            else
            {
                userEntity.Create();
            }
            service.SubmitForm(userEntity, userLogOnEntity, keyValue);
        }
        public void UpdateForm(UserEntity userEntity)
        {
            service.Update(userEntity);
        }
        public UserEntity CheckLogin(string username, string password)
        {
            UserEntity userEntity = service.FindEntity(t => t.ACCOUNT == username);
            if (userEntity != null)
            {
                if (userEntity.ENABLEDMARK == true)
                {
                    UserLogOnEntity userLogOnEntity = userLogOnApp.GetForm(userEntity.ID);
                    string dbPassword = Md5.md5(DESEncrypt.Encrypt(password.ToLower(), userLogOnEntity.USERSECRETKEY).ToLower(), 32).ToLower();
                    if (dbPassword == userLogOnEntity.USERPASSWORD)
                    {
                        DateTime lastVisitTime = Convert.ToDateTime(DateTime.Now.ToDateTimeString());                        
                        int LogOnCount = (userLogOnEntity.LOGONCOUNT).ToInt() + 1;
                        if (userLogOnEntity.LASTVISITTIME != null)
                        {
                            userLogOnEntity.PREVIOUSVISITTIME = userLogOnEntity.LASTVISITTIME.ToDate();
                        }
                        userLogOnEntity.LASTVISITTIME = lastVisitTime;
                        userLogOnEntity.LOGONCOUNT = LogOnCount;
                        userLogOnApp.UpdateForm(userLogOnEntity);
                        return userEntity;
                    }
                    else
                    {
                        throw new Exception("密码不正确，请重新输入");
                    }
                }
                else
                {
                    throw new Exception("账户被系统锁定,请联系管理员");
                }
            }
            else
            {
                throw new Exception("账户不存在，请重新输入");
            }
        }
    }
}
