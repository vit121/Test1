using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleDataGenerator.Model
{
    public class HostItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public object Image { get; set; }
        public List<ScheduleItem> Schedule { get; set; }
    }
}
