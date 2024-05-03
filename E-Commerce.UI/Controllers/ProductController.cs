using E_Commerce.DAL.Abstract;
using E_Commerce.Data.Context;
using E_Commerce.Data.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace E_Commerce.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly E_CommerceContext _CommerceContext;
        private readonly IProductDAL _productDAL;

        public ProductController(E_CommerceContext commerceContext, IProductDAL productDAL)
        {
            _CommerceContext = commerceContext;
            _productDAL = productDAL;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _CommerceContext.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _CommerceContext == null)
                return NotFound();
            var product = _productDAL.Get(Convert.ToInt32(id));
            if (product == null)
                return NotFound();
            return View(product);
        }
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_CommerceContext.Categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CategoryId,Image,Stock,Price,IsHome,IsApproved")]Product product,IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                if (image!=null&&image.Length>0)
                {
                    using (var stream=new MemoryStream())
                    {
                        await image.CopyToAsync(stream);
                        product.Image=stream.ToArray();
                    }
                }
                _productDAL.Add(product);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_CommerceContext.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
    }
}
