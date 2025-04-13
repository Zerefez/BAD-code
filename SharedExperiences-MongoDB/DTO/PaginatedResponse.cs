using System.Collections.Generic;

namespace SharedExperiences.DTO
{
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// Collection of items for the current page
        /// </summary>
        public IEnumerable<T> Items { get; set; } = null!;
        
        /// <summary>
        /// Current page number
        /// </summary>
        public int Page { get; set; }
        
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public long TotalCount { get; set; }
        
        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);
        
        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => Page > 1;
        
        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage => Page < TotalPages;
    }
} 