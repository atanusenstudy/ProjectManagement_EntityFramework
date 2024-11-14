using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Model
{
    public class TicketUpdate
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}
