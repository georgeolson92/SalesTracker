﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesTracker.Data;
using SalesTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace SalesTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewData["ItemId"] = new SelectList(_context.Item, "ItemId", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult NewSale(int ItemId, int Quantity, int UserId)
        {
            Sale newSale = new Sale(UserId, ItemId, Quantity);
            var selectedItem = _context.Item.FirstOrDefault(i => i.ItemId == ItemId);
            selectedItem.Quantity -= Quantity;
            _context.Sale.Add(newSale);
            _context.SaveChanges();
            return Json(newSale);
        }

        [HttpPost]
        public IActionResult ReturnItem(FormCollection collection)
        {
            var sale = _context.Sale.FirstOrDefault(s => s.SaleId == int.Parse(Request.Form["id"]));
             var selectedItem = _context.Item.FirstOrDefault(i => i.ItemId == sale.ItemId);
            selectedItem.Quantity += sale.Quantity;
            _context.Sale.Remove(sale);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult DisplayInventory()
        {
            var items = _context.Item.ToList();
            return View(items);
        }

        public IActionResult DisplaySales()
        {
            var sales = _context.Sale.Include(s => s.Item).ToList();
            return View(sales);
        }

        public IActionResult GetRevenue()
        {
            int revenue = 0;
            var sales = _context.Sale.Include(s => s.Item);
            foreach (var sale in sales)
            {
                revenue += sale.Quantity *= sale.Item.Price;
            }
            return Json(revenue);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult HelloAjax()
        {
            return Content("Hello from the controller!", "text/plain");
        }
    }
}
