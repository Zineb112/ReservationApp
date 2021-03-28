﻿using AppReservation.Data;
using AppReservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppReservation.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class TypeReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TypeReservationController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: TypeReservationController
        public ActionResult Index()
        {
            var allType = _context.TypeReservations.ToList();
            return View(allType);
        }

        // GET: TypeReservationController/Details/5
        public ActionResult Details(int id)
        {
            
            return View();
        }

        // GET: TypeReservationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypeReservationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Create(TypeReservation typeReservations)
        {
            try
            {

                _context.TypeReservations.Add(typeReservations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TypeReservationController/Edit/5
        public ActionResult Edit(int id)
        {
            var typeRes = _context.TypeReservations.Find(id);
            return View(typeRes);
        }

        // POST: TypeReservationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Edit(int id, TypeReservation typeReservation)
        {
            try
            {
                _context.Update(typeReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TypeReservationController/Delete/5
        public ActionResult Delete(int id, TypeReservation typeReservation)
        {
            var deletera = _context.TypeReservations.Find(id);
            return View(deletera);
        }

        // POST: TypeReservationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleteres = _context.TypeReservations.Find(id);
                _context.TypeReservations.Remove(deleteres);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
