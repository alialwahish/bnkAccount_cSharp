using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginReg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace LoginReg.Controllers
{


    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {

            _context = context;
            _context.SaveChanges();
        }



        [HttpGet("")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();


        }



        [HttpPost("Home/Withdraw")]
        public IActionResult Withdraw(int Withdraw,DateTime tDate)
        {
            
            string Email = HttpContext.Session.GetString("Email");
            Console.WriteLine(">>>>>>>>>>>>>>>>>Date from details " + tDate);

            User logUser = _context.users.SingleOrDefault(user => user.Email == Email);
            
            

            List<User> Users = _context.users.Include(usr => usr.Bank).ToList();
            

            
            


            if (logUser.Bank.Ballance - Withdraw < 0)
            {
               
                ViewBag.err="Not Enough Money in your account to withdraw! Amount requested "+Withdraw;
                ViewBag.logedUser = logUser;
                return View("Details");

            }


            logUser.Bank.Ballance = logUser.Bank.Ballance - Withdraw;
            
            _context.SaveChanges();
            ViewBag.logedUser = logUser;
            return View("Details");
        }


        [HttpPost("Home/Deposit")]
        public IActionResult Deposit(int Deposit)
        {
            string Email = HttpContext.Session.GetString("Email");
            Console.WriteLine(">>>>>>>>>>>>>>>>>Email in session " + Email);

            User logUser = _context.users.SingleOrDefault(user => user.Email == Email);

            List<User> Users = _context.users.Include(usr => usr.Bank).ToList();

            logUser.Bank.Ballance = logUser.Bank.Ballance + Deposit;
            _context.SaveChanges();
            ViewBag.logedUser = logUser;
            return View("Details");
        }




        [HttpGet("Registerd")]
        public IActionResult SignIn()
        {
            Console.WriteLine("Got inside registerd");


            return View("SignIn");
        }

        [HttpPost("Home/create")]
        public IActionResult Create(User user)
        {

            List<User> Users = _context.users.Include(usr => usr.Bank).ToList();
            // if (ModelState.IsValid)
            // {
            Console.WriteLine("Valid Registery");
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            user.Password = Hasher.HashPassword(user, user.Password);
            user.Confirm_Password = Hasher.HashPassword(user, user.Confirm_Password);
            BankAccount bofa = new BankAccount();
            bofa.Ballance = 500;

            bofa.Name = "Bank Of America";

            user.Bank = bofa;
            user.BankId = 2;

            _context.Add(user);
            _context.SaveChanges();
            ViewBag.logedUser = user;
            HttpContext.Session.SetString("Email", user.Email);

            return View("Details");
        }
        //     else
        //     {
        //         Console.WriteLine("Invalid Register");
        //         return View("Index");

        //     }

        // }







        [HttpPost("Home/login")]
        public IActionResult LogingMethod(string Email, string password)
        {
            List<User> Users = _context.users.Include(usr => usr.Bank).ToList();

            User logUser = _context.users.SingleOrDefault(user => user.Email == Email);

            if (logUser != null)
            {
                Console.WriteLine(logUser.Email);
                Console.WriteLine(logUser.Password);
                ViewBag.logedUser = logUser;
                HttpContext.Session.SetString("Email", logUser.Email);

                return View("Details");

            }
            else
            {
                Console.WriteLine("Invalid User");
                ViewBag.err = "Invalid User";
                return View("SignIn");
            }

        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
