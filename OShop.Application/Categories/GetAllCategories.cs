﻿using Microsoft.EntityFrameworkCore;
using OShop.Database;
using System.Collections.Generic;
using System.Linq;

namespace OShop.Application.Categories
{
    public class GetAllCategories
    {
        private readonly OnlineShopDbContext _context;

        public GetAllCategories(OnlineShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CategoryVMUI> Do() =>
            _context.Categories.AsNoTracking().ToList().Select(categ => new CategoryVMUI
            {
                CategoryId = categ.CategoryId,
                Name = categ.Name,
                Photo = categ.Photo,
            });
    }
}
