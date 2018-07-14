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
        public IActionResult Add(int Withdraw, DateTime Date)
        {

            string Email = HttpContext.Session.GetString("Email");

            User logUser = _context.users.SingleOrDefault(user => user.Email == Email);

            List<User> Users = _context.users.Include(usr => usr.Bank).Include(usr => usr.Transactions).ToList();

            Transaction trn = new Transaction();
            trn.Created_At = Date;
            trn.Type = "Withdraw";
            trn.UserId = logUser.UserId;
            trn.Amount = Withdraw;


            logUser.Bank.Ballance -= Withdraw;

            logUser.Transactions.Add(trn);
            logUser.Transactions = logUser.Transactions.OrderByDescending(t => t.Created_At).ToList();
            _context.SaveChanges();


            return RedirectToAction("WithdrawView");
        }



        public IActionResult WithdrawView()
        {

            string Email = HttpContext.Session.GetString("Email");

            User logUser = _context.users.SingleOrDefault(user => user.Email == Email);



            List<User> Users = _context.users.Include(usr => usr.Bank).Include(usr => usr.Transactions).ToList();


            logUser.Transactions = logUser.Transactions.OrderByDescending(t => t.Created_At).ToList();




            ViewBag.logedUser = logUser;
            _context.SaveChanges();
            return View("Details");
        }







        [HttpPost("Home/Deposit")]
        public IActionResult Deposit(int Deposit, DateTime Date)
        {

            string Email = HttpContext.Session.GetString("Email");


            User logUser = _context.users.SingleOrDefault(user => user.Email == Email);

            List<User> Users = _context.users.Include(usr => usr.Bank).Include(usr => usr.Transactions).ToList();


            Transaction trn = new Transaction();
            trn.Created_At = Date;
            trn.Type = "Deposit";
            trn.UserId = logUser.UserId;

            trn.Amount = Deposit;

            logUser.Bank.Ballance = logUser.Bank.Ballance + Deposit;
            logUser.Transactions.Add(trn);

            _context.SaveChanges();
            ViewBag.logedUser = logUser;

            return RedirectToAction("DepositView");
        }



        public IActionResult DepositView()
        {
            string Email = HttpContext.Session.GetString("Email");


            User logUser = _context.users.SingleOrDefault(user => user.Email == Email);




            List<User> Users = _context.users.Include(usr => usr.Bank).Include(usr => usr.Transactions).ToList();
            logUser.Transactions = logUser.Transactions.OrderByDescending(t => t.Created_At).ToList();
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
            user.Transactions = user.Transactions.OrderByDescending(t => t.Created_At).ToList();
            Console.WriteLine("Valid Registery");
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            user.Password = Hasher.HashPassword(user, user.Password);
            user.Confirm_Password = Hasher.HashPassword(user, user.Confirm_Password);

            Transaction trn = new Transaction();



            BankAccount bofa = new BankAccount();
            bofa.Ballance = 500;

            bofa.Name = "Bank Of America";

            user.Bank = bofa;
            user.BankId = bofa.BankId;


            _context.Add(user);
            _context.SaveChanges();
            ViewBag.logedUser = user;
            HttpContext.Session.SetString("Email", user.Email);

            return View("Details");
        }


        [HttpPost("Home/login")]
        public IActionResult LogingMethod(string Email, string Password)
        {
  
       


            List<User> Users = _context.users.Include(usr => usr.Bank).Include(usr => usr.Transactions).ToList();

            User logUser = _context.users.SingleOrDefault(usr => usr.Email == Email );


            if (logUser != null)
            {
                logUser.Transactions = logUser.Transactions.OrderByDescending(t => t.Created_At).ToList();
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
