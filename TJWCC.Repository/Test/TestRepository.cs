﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Data;
using TJWCC.Domain.Entity.Test;
using TJWCC.Domain.IRepository.Test; 

namespace TJWCC.Repository.Test
{
    public class TestRepository :RepositoryBase<TestEntity>,ITestRepository
    {
    }
}
