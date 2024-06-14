using System;

namespace TJWCC.Domain.Entity.Test
{
    public class TestEntity : IEntity<TestEntity>
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string ADDRESS { get; set; }

        public string TEL { get; set; }
    }
}
