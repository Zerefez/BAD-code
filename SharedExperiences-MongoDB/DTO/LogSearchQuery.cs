using System;

namespace SharedExperiences.DTO
{
    public class LogSearchQuery
    {
        /// <summary>
        /// Filter logs by user ID
        /// </summary>
        public string? UserId { get; set; }
        
        /// <summary>
        /// Filter logs by start date (inclusive)
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// Filter logs by end date (inclusive)
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// Filter logs by HTTP method (POST, PUT, DELETE)
        /// </summary>
        public string? Method { get; set; }
        
        /// <summary>
        /// Filter logs by operation description (e.g., "Creating new service")
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int Page { get; set; } = 1;
        
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
} 