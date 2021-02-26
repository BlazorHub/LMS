using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Shared
{
    public class PagedData<T>
    {
        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<T> Data { get; set; }

        public PagedData() { }

        public PagedData(IQueryable<T> baseData, int page, int pageSize)
        {
            this.CurrentPage = page;
            this.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(baseData.Count()) / pageSize));
            this.PageSize = pageSize;
            this.Data = baseData.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
