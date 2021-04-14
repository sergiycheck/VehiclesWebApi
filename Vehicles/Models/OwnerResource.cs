using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vehicles.Models
{
    public class OwnerResource
    {
        public int Id { get; set; }
        public IEnumerable<string> OwnersId { get; set; }

    }
}
