using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Models
{
    public class Client
    {

        public long Id { get; set; }
        public string ClName { get; set; }
        public string ClSurname { get; set; }
        public string ClEmail { get; set; }
        public string ClPhone { get; set; }
    }
}
