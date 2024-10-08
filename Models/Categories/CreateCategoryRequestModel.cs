﻿using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Categories
{
    public class CreateCategoryRequestModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(5000)]
        public string? Description { get; set; }
    }
}
