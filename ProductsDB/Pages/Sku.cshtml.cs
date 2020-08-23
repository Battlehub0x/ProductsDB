using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductsDB.Data;
using ProductsDB.Models;

namespace ProductsDB.Pages
{
    public class SkuModel : PageModel
    {
        public AppDbContext _context;
        public string ProductId { get; set; }
        public bool ShowProduct => !string.IsNullOrEmpty(ProductId);// && !ProductId.Equals("all");

        public IQueryable<Product> Products { get; set; }
        public IQueryable<PriceDetail> ProductPrices { get; set; }
        public List<PriceDetail> OptimisedPrices { get; set; }

        private readonly ILogger<SkuModel> _logger;

        public SkuModel(ILogger<SkuModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet(string id)
        {
            ProductId = id.Equals("all") ? null : id;

            if (ShowProduct)
            {
                // get price details for the product and sort by market->currency->date
                ProductPrices = _context.PriceDetails.AsNoTracking()
                    .Where(p => p.CatalogEntryCode == ProductId)
                    .OrderBy(p => p.MarketId)
                    .ThenBy(p => p.CurrencyCode)
                    .ThenBy(p => p.ValidUntil);

                calculate(ProductPrices.ToList());
            }
            else
            {
                // show list with 10 products
                Products = _context.Products.AsNoTracking().Take(10);
            }

            
        }

        private void calculate(List<PriceDetail> prices)
        {
            foreach(var line in prices)
            {
                Console.WriteLine($"{line.MarketId} {line.UnitPrice} {line.CurrencyCode} {line.ValidFrom} - {line.ValidUntil}");
            }
            OptimisedPrices = prices;
        }
    }
}
