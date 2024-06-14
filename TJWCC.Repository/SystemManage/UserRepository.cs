using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Repository.SystemManage;

namespace TJWCC.Repository.SystemManage
{
    public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Delete<UserEntity>(t => t.ID == keyValue);
                db.Delete<UserLogOnEntity>(t => t.USERID == keyValue);
                db.Commit();
            }
        }
        public void SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    db.Update(userEntity);
                }
                else
                {
                    userLogOnEntity.ID = userEntity.ID;
                    userLogOnEntity.USERID = userEntity.ID;
                    userLogOnEntity.USERSECRETKEY = Md5.md5(Common.CreateNo(), 16).ToLower();
                    userLogOnEntity.USERPASSWORD = Md5.md5(DESEncrypt.Encrypt(Md5.md5(userLogOnEntity.USERPASSWORD, 32).ToLower(), userLogOnEntity.USERSECRETKEY).ToLower(), 32).ToLower();
                    db.Insert(userEntity);
                    db.Insert(userLogOnEntity);
                }
                db.Commit();
            }
        }
    }
}
