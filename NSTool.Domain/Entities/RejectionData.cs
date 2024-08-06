using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTool.Domain.Entities
{
    public class RejectionData
    {
        public string SR { get; set; }
        public string Creator { get; set; }
        public string Status { get; set; }
        public string SubArea { get; set; }
        public string OwnerTeam { get; set; }
        public string BS_Rejection { get; set; }
        public string IT_Rejection { get; set; }
        public string Justification { get; set; } = string.Empty;
    }
}
