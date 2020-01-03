using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoMedic.Model
{
    public class Users
    {
        public int ItbId { get; set; }
        public string  Username { get; set; }
        public string User_type { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
