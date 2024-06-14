using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Domain.IRepository.SystemManage;
using TJWCC.Domain.ViewModel;
using TJWCC.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TJWCC.Application.SystemManage
{
    public class RoleAuthorizeApp
    {
        private IRoleAuthorizeRepository service = new RoleAuthorizeRepository();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public List<RoleAuthorizeEntity> GetList(string ObjectId)
        {
            return service.IQueryable(t => t.OBJECTID == ObjectId).ToList();
        }
        /// <summary>
        /// 根据角色获取菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<ModuleEntity> GetMenuList(string roleId)
        {
            var data = new List<ModuleEntity>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = moduleApp.GetList();
            }
            else
            {
                var moduledata = moduleApp.GetList();
                var authorizedata = service.IQueryable(t => t.OBJECTID == roleId && t.ITEMTYPE == 1).ToList();
                foreach (var item in authorizedata)
                {
                    ModuleEntity moduleEntity = moduledata.Find(t => t.ID == item.ITEMID);
                    if (moduleEntity != null)
                    {
                        data.Add(moduleEntity);
                    }
                }
            }
            return data.OrderBy(t => t.SORTCODE).ToList();
        }
        public List<ModuleButtonEntity> GetButtonList(string roleId)
        {
            var data = new List<ModuleButtonEntity>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = moduleButtonApp.GetList();
            }
            else
            {
                var buttondata = moduleButtonApp.GetList();
                var authorizedata = service.IQueryable(t => t.OBJECTID == roleId && t.ITEMTYPE == 2).ToList();
                foreach (var item in authorizedata)
                {
                    ModuleButtonEntity moduleButtonEntity = buttondata.Find(t => t.ID == item.ITEMID);
                    if (moduleButtonEntity != null)
                    {
                        data.Add(moduleButtonEntity);
                    }
                }
            }
            return data.OrderBy(t => t.SORTCODE).ToList();
        }
        /// <summary>
        /// 判断模块权限
        /// </summary>
        /// <param name="roleId">角色编号</param>
        /// <param name="moduleId">模块编号</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public bool ActionValidate(string roleId, string moduleId, string action)
        {

            ////针对大屏展示的两个界面进行单独验证；所有用户都具有权限
            if (action.ToUpper() == ("/DataDisplay/Main/Index").ToUpper() || action.ToUpper() == ("/DataDisplay/Main/Secondary").ToUpper() || action.ToUpper() == ("/Home/Index").ToUpper())
            {
                return true;
            }
            var authorizeurldata = new List<AuthorizeActionModel>();
            var cachedata = CacheFactory.Cache().GetCache<List<AuthorizeActionModel>>("authorizeurldata_" + roleId);
            if (cachedata == null)
            {
                var moduledata = moduleApp.GetList();
                var buttondata = moduleButtonApp.GetList();
                var authorizedata = service.IQueryable(t => t.OBJECTID == roleId).ToList();
                foreach (var item in authorizedata)
                {
                    if (item.ITEMTYPE == 1)
                    {
                        ModuleEntity moduleEntity = moduledata.Find(t => t.ID == item.ITEMID);
                        authorizeurldata.Add(new AuthorizeActionModel { Id = moduleEntity.ID, UrlAddress = moduleEntity.URLADDRESS });
                    }
                    else if (item.ITEMTYPE == 2)
                    {
                        ModuleButtonEntity moduleButtonEntity = buttondata.Find(t => t.ID == item.ITEMID);
                        authorizeurldata.Add(new AuthorizeActionModel { Id = moduleButtonEntity.MODULEID, UrlAddress = moduleButtonEntity.URLADDRESS });
                    }
                }
                CacheFactory.Cache().WriteCache(authorizeurldata, "authorizeurldata_" + roleId, DateTime.Now.AddMinutes(5));
            }
            else
            {
                authorizeurldata = cachedata;
            }
            authorizeurldata = authorizeurldata.FindAll(t => t.Id.Equals(moduleId));
            foreach (var item in authorizeurldata)
            {
                if (!string.IsNullOrEmpty(item.UrlAddress))
                {
                    string[] url = item.UrlAddress.Split('?');
                    if (item.Id == moduleId && url[0] == action)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
