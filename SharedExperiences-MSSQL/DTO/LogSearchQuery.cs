using System;
using System.ComponentModel.DataAnnotations;

namespace SharedExperiences.DTO
{
    public class LogSearchQuery
    {
        // All fields are optional
        public string? UserId { get; set; }
        public string? Method { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        // Default values for pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
} 