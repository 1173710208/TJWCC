using TJWCC.Code;
using System;

namespace TJWCC.Domain
{
    public class IEntity<TEntity>
    {
        public void Create()
        {
            var entity = this as ICreationAudited;
            entity.ID = Common.GuId();
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo != null)
            {
                entity.CREATORUSERID = LoginInfo.UserId;
            }
            entity.CREATORTIME = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
        }
        public void Modify(string keyValue)
        {
            var entity = this as IModificationAudited;
            entity.ID = keyValue;
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo != null)
            {
                entity.LASTMODIFYUSERID = LoginInfo.UserId;
            }
            entity.LASTMODIFYTIME = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
        }
        public void Remove()
        {
            var entity = this as IDeleteAudited;
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo != null)
            {
                entity.DELETEUSERID = LoginInfo.UserId;
            }
            entity.DELETETIME = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            entity.DELETEMARK = true;
        }
    }
}
