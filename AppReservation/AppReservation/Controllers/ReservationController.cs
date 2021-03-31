using AppReservation.Common;
using AppReservation.Data;
using AppReservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppReservation.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: ReservationController
        public async Task<ActionResult> Index()
        {

            ViewBag.List = Utile.StatusToList();


            var list = await _context.Reservations
                .Include(t => t.Reserv)
                .Include(s => s.Student)
                .OrderBy(r => r.Date)
                .Where(r => r.Status == Status.Attente.ToString())
                .ToListAsync();
            return View("Index", list);

        }

        // GET: ReservationController
        public async Task<ActionResult> History()
        {

            ViewBag.List = Utile.StatusToList();


            var list = await _context.Reservations
                .Include(t => t.Reserv)
                .Include(s => s.Student)
                .OrderBy(r => r.Date)
                .ToListAsync();
            return View(list);

        }

        public async Task<ActionResult> filterParDate(DateTime filterDate)
        {
            if (filterDate.Year == 0001)
            {
                return RedirectToAction(nameof(History));
            }
            else
            {

                var list = await _context.Reservations
                               .Include(t => t.Reserv)
                               .Include(s => s.Student)
                               .OrderBy(r => r.Date)
                               .Where(r => r.Date == filterDate)
                               .ToListAsync();
                return View("History", list);
            }
        }

        // GET: ReservationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReservationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReservationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*public async Task <ActionResult> Create(Reservation reservation)
        {
            try
            {
                await _context.Reservations.AddAsync(reservation);
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/

        // GET: ReservationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReservationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReservationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        public async Task<ActionResult> ChangeStatusAsync(string Id, string status)
        {

            var reservation = await _context.Reservations.FirstAsync(r => r.Id.ToString() == Id);
            reservation.Status = status;
            _context.Update(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
