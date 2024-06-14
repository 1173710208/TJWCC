using TJWCC.Code;
using TJWCC.Domain.Entity.Test;
using TJWCC.Domain.IRepository.Test;
using TJWCC.Repository.Test;
using System;
using System.Collections.Generic;

namespace TJWCC.Application.Test
{
    public class TestApp
    {
        private ITestRepository service = new TestRepository();

        //查找所有test信息
        public List<TestEntity> GetList(Pagination pagination)
        {

            return service.FindList(pagination);
        }

        public void InsertTest()
        {
            TestEntity entity = new TestEntity();
            entity.ID = Common.GuId();
            entity.NAME = "张三";
            entity.ADDRESS = "新增地址";
            entity.TEL = "13800000000";
            service.Insert(entity);

        }

    }
}
