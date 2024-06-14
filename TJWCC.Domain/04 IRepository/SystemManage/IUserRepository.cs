using TJWCC.Data;
using TJWCC.Domain.Entity.SystemManage;

namespace TJWCC.Domain.IRepository.SystemManage
{
    public interface IUserRepository : IRepositoryBase<UserEntity>
    {
        void DeleteForm(string keyValue);
        void SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue);
    }
}
