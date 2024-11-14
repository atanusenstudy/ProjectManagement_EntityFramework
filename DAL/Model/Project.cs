using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; } // Navigation Property
    }
}
