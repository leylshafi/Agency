﻿using Agency.Models.Base;

namespace Agency.Areas.Admin.ViewModels
{
    public class PaginationVM<T> where T : class, new()
    {
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public List<T> Items { get; set; }
    }
}
