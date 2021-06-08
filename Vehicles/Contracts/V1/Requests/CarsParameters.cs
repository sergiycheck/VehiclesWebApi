using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vehicles.Contracts.V1.Requests.Queries;

namespace vehicles.Contracts.V1.Requests
{
    public class CarsParameters: QueryStringParameters
    {
        public string searchText { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
        public float? minEnginePower { get; set; }
        public float? maxEnginePower { get; set; }
        public DateTime? minDate { get; set; }
        public DateTime? maxDate { get; set; }

    }

    public class UsersParameters: QueryStringParameters
    {

    }


}
