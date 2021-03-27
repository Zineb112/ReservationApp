using AppReservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppReservation.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        public RoleController(RoleManager<IdentityRole> roleManager,
                               UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(Role model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = model.RoleName
                };

                IdentityResult result = await this.roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = this.roleManager.Roles;
            return View(roles);
        }


        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (id is null)
            {
                return View("../Errors/NotFound", "Please add the role Id in URL");
            }
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role is null)
            {
                return View("../Errors/NotFound", $"The role Id : {id} cannot be found");
            }

            EditRole model = new EditRole()
            {
                Id = role.Id,
                RoleName = role.Name,
                Users = new List<string>()
            };

            foreach (IdentityUser user in await userManager.Users.ToListAsync()) // userManager.Users is not awaitble so change to (await userManager.Users.ToListAsync())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRole model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(model.Id);
                if (role is null)
                {
                    return View("../Errors/NotFound", $"The role Id : {model.Id} cannot be found");
                }
                role.Name = model.RoleName;

                IdentityResult result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);

        }




        [HttpPost]
        public async Task<ActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (!(role is null))
            {
                var result = await roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(ListRoles));
        }


        //************************************************************** EditUsersRole ******************************************************************************

        [HttpGet]
        public async Task<IActionResult> EditUsersRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return View("../Errors/NotFound", $"The role must be exist and not empty in Url");

            }
            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                return View("../Errors/NotFound", $"The role Id : {role.Id} cannot be found");
            }

            var Models = new List<EditUsersRole>();

            foreach (var user in await userManager.Users.ToListAsync())
            {
                EditUsersRole model = new EditUsersRole()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = false
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.IsSelected = true;
                }
                else
                {
                    model.IsSelected = false;
                }

                Models.Add(model);
            }
            ViewBag.RoleId = roleId;
            return View(Models);

        }



        [HttpPost]
        public async Task<IActionResult> EditUsersRole(List<EditUsersRole> model, string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return View("../Errors/NotFound", $"The role must be exist and not empty in Url");

            }
            var role = await this.roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                return View("../Errors/NotFound", $"The role Id : {role.Id} cannot be found");
            }

            // role if deja affectté et in model is select il faut le supprimer , ou l'affecté si il est selecté au model mais non affecté before

            IdentityResult result = null;

            for (int i = 0; i < model.Count; i++)
            {
                IdentityUser user = await userManager.FindByNameAsync(model[i].UserName);

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if ((await userManager.IsInRoleAsync(user, role.Name)) && !model[i].IsSelected)
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);

                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
                //if (result.Succeeded)
                //{
                //    foreach (var error in result.Errors)
                //    {
                //        ModelState.AddModelError("", error.Description);
                //    }

                //}
            }

            return RedirectToAction("EditRole", new { id = roleId });

        }


    }


}
