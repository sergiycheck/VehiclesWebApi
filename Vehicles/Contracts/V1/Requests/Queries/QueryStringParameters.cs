using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vehicles.Contracts.V1.Requests.Queries
{
    public abstract class QueryStringParameters
    {
        private int _pageNum = 1;

        public int PageNum
        {
            get
            {
                return _pageNum;
            }
            set
            {
                if (value > 0 && value != 0)
                {
                    _pageNum = value;
                }
            }
        }

        const int maxPageSize = 50;
        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
