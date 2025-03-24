// Services/PaginationService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using QuanLiKhiThai.Helper;

namespace QuanLiKhiThai.DAO
{
    /// <summary>
    /// Service for managing paginated data collections
    /// </summary>
    public class PaginationService
    {
        /// <summary>
        /// Dictionary of active pagination helpers by key
        /// </summary>
        private Dictionary<string, Dictionary<Type, object>> _paginationHelpers = new Dictionary<string, Dictionary<Type, object>>();

        /// <summary>
        /// Create or update a pagination helper for a collection
        /// </summary>
        /// <typeparam name="T">Type of items in the collection</typeparam>
        /// <param name="key">Unique identifier for this paginated collection</param>
        /// <param name="items">Collection to paginate</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="currentPage">Initial page number (1-based)</param>
        /// <returns>A pagination helper for the collection</returns>
        public PaginationHelper<T> GetOrCreatePaginator<T>(
            string key,
            IEnumerable<T> items,
            int pageSize = 10,
            int currentPage = 1)
        {
            Type itemType = typeof(T);

            // ensure the dictionary have the key
            if (!_paginationHelpers.ContainsKey(key)) 
            {
                _paginationHelpers[key] = new Dictionary<Type, object>();
            }

            // Check if a helper for this type already exists
            if (_paginationHelpers[key].TryGetValue(itemType, out var existingHelper))
            {
                var typedHelper = (PaginationHelper<T>)existingHelper;
                typedHelper.UpdateItems(items);

                // Update page size if it changed
                if (typedHelper.PageSize != pageSize)
                {
                    typedHelper.SetPageSize(pageSize);
                }

                // Try to maintain current page position
                typedHelper.SetPage(currentPage);
                return typedHelper;
            }
            // create new paginator
            var newHelper = new PaginationHelper<T>(items, pageSize, currentPage);
            _paginationHelpers[key][itemType] = newHelper;
            return newHelper;
        }

        /// <summary>
        /// Get an existing pagination helper if it exists
        /// </summary>
        /// <typeparam name="T">Type of items in the collection</typeparam>
        /// <param name="key">Unique identifier for the paginated collection</param>
        /// <returns>The pagination helper if found, otherwise null</returns>
        public PaginationHelper<T>? GetPaginator<T>(string key)
        {
            Type itemType = typeof(T);

            if (_paginationHelpers.TryGetValue(key, out var typeDictionary) && typeDictionary.TryGetValue(itemType, out var helper))
            {
                return (PaginationHelper<T>)helper;
            }

            return null;
        }

        /// <summary>
        /// Remove a pagination helper
        /// </summary>
        /// <param name="key">Unique identifier for the paginated collection</param>
        /// <returns>true if removed, false if not found</returns>
        public bool RemovePaginator<T>(string key)
        {
            Type itemType = typeof(T);

            if (_paginationHelpers.TryGetValue(key, out var typeDictionary))
            {
                return typeDictionary.Remove(itemType);
            }
            return false;
        }

        public bool RemovePaginator(string key)
        {
            return _paginationHelpers.Remove(key);
        }

        /// <summary>
        /// Create a paginated collection without storing it in the service
        /// </summary>
        /// <typeparam name="T">Type of items in the collection</typeparam>
        /// <param name="items">Collection to paginate</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="currentPage">Initial page number (1-based)</param>
        /// <returns>A pagination helper for the collection</returns>
        public static PaginationHelper<T> CreatePaginator<T>(
            IEnumerable<T> items,
            int pageSize = 10,
            int currentPage = 1)
        {
            return new PaginationHelper<T>(items, pageSize, currentPage);
        }
    }
}
