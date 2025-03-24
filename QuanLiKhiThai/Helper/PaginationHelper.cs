// Helpers/PaginationHelper.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLiKhiThai.Helper
{
    /// <summary>
    /// Helper class for handling pagination of collections
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    public class PaginationHelper<T>
    {
        /// <summary>
        /// The entire collection of items to be paginated
        /// </summary>
        private List<T> _items;

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Total number of items in the collection
        /// </summary>
        public int TotalItems { get; private set; }

        /// <summary>
        /// Whether there is a previous page available
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Whether there is a next page available
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Initialize a new pagination helper with a collection of items
        /// </summary>
        /// <param name="items">Collection of items to paginate</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="initialPage">Initial page to display (1-based)</param>
        public PaginationHelper(IEnumerable<T> items, int pageSize = 10, int initialPage = 1)
        {
            _items = items?.ToList() ?? new List<T>();
            PageSize = pageSize > 0 ? pageSize : 10; // Ensure page size is at least 1
            TotalItems = _items.Count;
            TotalPages = CalculateTotalPages();
            SetPage(initialPage);
        }

        /// <summary>
        /// Updates the underlying collection and recalculates pagination info
        /// </summary>
        /// <param name="items">New collection of items</param>
        public void UpdateItems(IEnumerable<T> items)
        {
            _items = items?.ToList() ?? new List<T>();
            TotalItems = _items.Count;
            TotalPages = CalculateTotalPages();

            // Ensure current page is still valid
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
            else if (TotalPages == 0)
            {
                CurrentPage = 1;
            }
        }

        /// <summary>
        /// Set the page size and recalculate pagination
        /// </summary>
        /// <param name="pageSize">New page size</param>
        public void SetPageSize(int pageSize)
        {
            if (pageSize < 1) throw new ArgumentException("Page size must be at least 1");

            PageSize = pageSize;
            TotalPages = CalculateTotalPages();

            // Adjust current page if needed
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
        }

        /// <summary>
        /// Move to a specific page
        /// </summary>
        /// <param name="pageNumber">Page number to move to (1-based)</param>
        /// <returns>true if the page was changed, false otherwise</returns>
        public bool SetPage(int pageNumber)
        {
            if (TotalPages == 0)
            {
                CurrentPage = 1;
                return false;
            }

            if (pageNumber < 1 || pageNumber > TotalPages)
                return false;

            CurrentPage = pageNumber;
            return true;
        }

        /// <summary>
        /// Move to the next page if available
        /// </summary>
        /// <returns>true if moved to next page, false if already on last page</returns>
        public bool NextPage()
        {
            return HasNextPage && SetPage(CurrentPage + 1);
        }

        /// <summary>
        /// Move to the previous page if available
        /// </summary>
        /// <returns>true if moved to previous page, false if already on first page</returns>
        public bool PreviousPage()
        {
            return HasPreviousPage && SetPage(CurrentPage - 1);
        }

        /// <summary>
        /// Move to the first page
        /// </summary>
        public void FirstPage()
        {
            SetPage(1);
        }

        /// <summary>
        /// Move to the last page
        /// </summary>
        public void LastPage()
        {
            SetPage(TotalPages);
        }

        /// <summary>
        /// Get the current page of items
        /// </summary>
        /// <returns>List of items for the current page</returns>
        public List<T> GetCurrentPage()
        {
            if (TotalItems == 0)
                return new List<T>();

            return _items
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        /// <summary>
        /// Get a specific page of items
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <returns>List of items for the requested page</returns>
        public List<T> GetPage(int pageNumber)
        {
            if (TotalItems == 0 || pageNumber < 1 || pageNumber > TotalPages)
                return new List<T>();

            return _items
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        /// <summary>
        /// Calculate total pages based on current page size
        /// </summary>
        private int CalculateTotalPages()
        {
            if (TotalItems == 0)
                return 0;

            return (TotalItems + PageSize - 1) / PageSize;
        }
    }
}
