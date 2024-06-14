using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;

namespace TJWCC.Application.SystemManage
{
    public class UserLogOnApp
    {
        private IUserLogOnRepository service = new UserLogOnRepository();

        public UserLogOnEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void UpdateForm(UserLogOnEntity userLogOnEntity)
        {
            service.Update(userLogOnEntity);
        }
        public void RevisePassword(string userPassword,string keyValue)
        {
            UserLogOnEntity userLogOnEntity = new UserLogOnEntity();
            userLogOnEntity.ID = keyValue;
            userLogOnEntity.USERSECRETKEY = Md5.md5(Common.CreateNo(), 16).ToLower();
            userLogOnEntity.USERPASSWORD = Md5.md5(DESEncrypt.Encrypt(Md5.md5(userPassword, 32).ToLower(), userLogOnEntity.USERSECRETKEY).ToLower(), 32).ToLower();
            service.Update(userLogOnEntity);
        }
    }
}
