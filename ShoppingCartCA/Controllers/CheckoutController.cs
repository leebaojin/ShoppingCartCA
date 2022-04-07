﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DBContext dbContext;
        public CheckoutController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "Continue Shopping" });
            return View();
        }
    }
}