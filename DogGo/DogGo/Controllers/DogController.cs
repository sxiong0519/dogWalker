using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Repositories;
using DogGo.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DogGo.Controllers
{
    public class DogController : Controller
    {
            private readonly IDogRepository _dogRepo;

            // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
            public DogController(IDogRepository dogRepository)
            {
                _dogRepo = dogRepository;
            }

        // GET: WalkersController
        [Authorize]
        public ActionResult Index()
            {
                int ownerId = GetCurrentUserId();

                List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);

                return View(dogs);
            }


        // GET: Walkers/Details/5
        public ActionResult Details(int id)
            {
                Dog dog = _dogRepo.GetDogById(id);

                if (dog == null)
                {
                    return NotFound();
                }

                return View(dog);
            }


        // GET: WalkersController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: WalkersController/Create
        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog dog)
        {
            try
            {
                // update the dogs OwnerId to the current user's Id
                dog.OwnerId = GetCurrentUserId();

                _dogRepo.AddDog(dog);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }


        public ActionResult Delete(int id)
        {
            int ownerId = GetCurrentUserId();
            Dog dog = _dogRepo.GetDogById(id);
            if (ownerId == dog.Owner.Id)
            {
                return View(dog);
            } else
            {
                return NotFound();
            }
        }

        // POST: Owners/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Dog dog)
        {
            try
            {
              int ownerId = GetCurrentUserId();
                if (ownerId == dog.Owner.Id)
                {
                    _dogRepo.DeleteDog(id);
                }
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }

        // GET: Owners/Edit/5

        public ActionResult Edit(int id)
        {
            int ownerId = GetCurrentUserId();
            Dog dog = _dogRepo.GetDogById(id);
            if (ownerId != dog.Owner.Id || dog == null)
            {
                return NotFound();
                
            }            
            return View(dog);
            
        }

        // POST: Owners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog dog)
        {
            
            try
            {
                int ownerId = GetCurrentUserId();
                if (ownerId == dog.Owner.Id)
                {
                    _dogRepo.UpdateDog(dog);
                }
                    return RedirectToAction("Index");
                
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }


    }
}
    
