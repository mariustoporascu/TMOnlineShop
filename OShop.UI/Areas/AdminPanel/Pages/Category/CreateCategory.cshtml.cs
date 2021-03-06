﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OShop.Application.Categories;
using OShop.Application.FileManager;
using OShop.Database;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OShop.UI.Areas.AdminPanel.Pages.Category
{
    [Authorize(Roles = "SuperAdmin")]
    public class CreateCategoryModel : PageModel
    {
        private readonly OnlineShopDbContext _context;
        private readonly IFileManager _fileManager;

        public CreateCategoryModel(OnlineShopDbContext context, IFileManager fileManager)
        {
            _context = context;
            _fileManager = fileManager;
        }

        [BindProperty]
        public CategoryVMUI Category { get; set; }

        public void OnGet(int? categId)
        {
            if (categId == null)
                Category = new CategoryVMUI();
            else
            {
                var getCategory = new GetCategory(_context).Do(categId);
                Category = new CategoryVMUI
                {
                    CategoryId = getCategory.CategoryId,
                    Name = getCategory.Name,
                    Photo = getCategory.Photo,
                };
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (Request.Form.Files.Count > 0)
                {
                    var extensionAccepted = new string[] { ".jpg", ".png", ".jpeg" };
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    var extension = Path.GetExtension(file.FileName);
                    if (!extensionAccepted.Contains(extension.ToLower()))
                        return RedirectToPage("/Error", new { Area = "" });
                    else
                    {
                        if (!string.IsNullOrEmpty(Category.Photo))
                        {
                            _fileManager.RemoveImage(Category.Photo, "CategoryPhoto");
                        }
                        Category.Photo = await _fileManager.SaveImage(file, "CategoryPhoto");
                    }

                }
                else if (Request.Form.Files.Count == 0)
                {
                    Category.Photo = Category.Photo;
                }
                if (Category.CategoryId > 0)
                {
                    var category = new CategoryVMUI
                    {
                        CategoryId = Category.CategoryId,
                        Name = Category.Name,
                        Photo = Category.Photo,
                    };
                    await new UpdateCategory(_context).Do(category);
                }
                else
                    await new CreateCategory(_context).Do(Category);
                return RedirectToPage("./Index");
            }
            return RedirectToPage("Error", new { Area = "" });
        }
    }
}
