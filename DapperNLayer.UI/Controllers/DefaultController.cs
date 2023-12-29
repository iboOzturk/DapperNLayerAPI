﻿using DapperNLayer.UI.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Security.Claims;

namespace DapperNLayer.UI.Controllers
{
    [Authorize]

    public class DefaultController : Controller
    {
		private readonly ApiService _apiService;
		 CategoryService _categoryService=new CategoryService();
		ProductService _productService=new ProductService();

        public DefaultController(ApiService apiService)
        {
            _apiService = apiService;
            
        }

        public IActionResult Index()
        {
            return View();
        }
		public async Task<IActionResult> MyItems()
		{
			//string token = HttpContext.Session.GetString("JwtToken");
			string token = HttpContext.User.FindFirst(ClaimTypes.UserData)?.Value;

            if (string.IsNullOrEmpty(token))
			{
				return RedirectToAction("Index","Login");
			}

			var items = await _apiService.GetMyItemsAsync(token);

			return View(items);
		}
        #region Products
        public async Task<IActionResult> ProductList()
		{
			var result = await _apiService.GetProductsAsync();
			return View(result);
		}
        public async Task<IActionResult> ProductWithCategoryList()
        {
            var result = await _apiService.GetProductsWithCategoryAsync();
            return View(result);
        }
		
		public IActionResult DeleteProduct(int id)
		{
			_productService.DeleteProduct(id);
			return Json(new { success = true, message = "Ürün başarıyla silindi." });
		}
		[HttpGet]
		public IActionResult EditProduct(int id)
		{
			var result=_productService.GetProductById2(id);
			var categoryList = _categoryService.GetCategories();
            List < SelectListItem > categories= (from x in categoryList
                                                 select new SelectListItem
                                                 {
                                                     Text = x.Name.ToString(),
                                                     Value = x.CategoryId.ToString()
                                                 }).ToList();
            ViewBag.Categories = categories;

            return View(result);
		}
		[HttpPost]
		public IActionResult EditProduct(Product product)
		{
			_productService.UpdateProduct(product);
			return RedirectToAction("ProductWithCategoryList", "Default");
		}

        #endregion

        #region Category
        public IActionResult CategoryList()
		{
			var result = _categoryService.GetCategories();
			return View(result);
		}
        #endregion
        public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync();
			return RedirectToAction("Index", "Login");
		}
	}
}
