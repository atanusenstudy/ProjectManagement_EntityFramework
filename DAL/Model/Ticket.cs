using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string Description { get; set; }
        public DateTime AssignedDate { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } // Navigation Property
        public int ProjectId { get; set; }
        public Project Project { get; set; } // Navigation Property
    }
}
