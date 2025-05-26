using System.Collections.Generic;
using UnityEngine;

namespace Pckgs
{
    public class PaginatedData<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int From { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
        public int PerPage { get; set; }
        public int PageCount { get; set; }
    }

    public static class PaginatedDataExtensions
    {
        public static int GetCurrentPage<T>(this PaginatedData<T> data)
        {
            return (data.From / Mathf.Max(1, data.PerPage)) + 1;
        }

        public static int GetTotalPages<T>(this PaginatedData<T> data)
        {
            return Mathf.Max(1, Mathf.CeilToInt((float)data.Total / Mathf.Max(1, data.PerPage)));
        }

        public static List<string> GetPaginationPages<T>(this PaginatedData<T> data)
        {
            int currentPage = data.GetCurrentPage();
            int totalPages = data.GetTotalPages();
            var pages = new List<string>();

            pages.Add("1");

            if (currentPage > 3)
                pages.Add("...");

            for (int i = currentPage - 1; i <= currentPage + 1; i++)
            {
                if (i > 1 && i < totalPages)
                    pages.Add(i.ToString());
            }

            if (currentPage < totalPages - 2)
                pages.Add("...");

            if (totalPages > 1)
                pages.Add(totalPages.ToString());

            return pages;
        }

    }

}