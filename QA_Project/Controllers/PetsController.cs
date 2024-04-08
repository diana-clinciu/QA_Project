using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QA_Project.Data;
using QA_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace QA_Project.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IWebHostEnvironment _env;

        public PetsController(ApplicationDbContext db,
                              IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }


        public IActionResult Index()
        {
            var pets = _db.Pets.ToList();

            var search = "";

            var speciesFilter = "";

            var breedFilter = "";

            var colorFilter = "";

            var locationFilter = "";

            var ageFilter = "";

            var sizeFilter = "";

            var sexFilter = "";

            var vaccineFilter = "";

            var sterilizeFilter = "";

            // retinem valorile distincte existente pentru fiecare atribut

            ViewBag.SpeciesList = GetDistinctSpecies();
            ViewBag.BreedsList = GetDistinctBreed();
            ViewBag.ColorList = GetDistinctColor();
            ViewBag.LocationList = GetDistinctLocation();
            ViewBag.AgeList = GetDistinctAge();
            ViewBag.SizeList = GetDistinctSize();
            ViewBag.SexList = GetDistinctSex();
            ViewBag.VaccineList = GetDistinctVaccined();
            ViewBag.SterilizeList = GetDistinctSterilized();


            //filtrare

            if (Convert.ToString(HttpContext.Request.Query["speciesFilter"]) != null)
            {
                speciesFilter = Convert.ToString(HttpContext.Request.Query["speciesFilter"]).Trim();

                ViewBag.SpeciesFilter = speciesFilter;
            }

            if (!string.IsNullOrEmpty(speciesFilter))
            {
                pets = pets.Where(p => p.Species == speciesFilter).ToList();
            }

            if (Convert.ToString(HttpContext.Request.Query["breedFilter"]) != null)
            {
                breedFilter = Convert.ToString(HttpContext.Request.Query["breedFilter"]).Trim();

                ViewBag.BreedFilter = breedFilter;
            }


            if (!string.IsNullOrEmpty(breedFilter))
            {
                pets = pets.Where(p => p.Breed == breedFilter).ToList();
            }


            if (Convert.ToString(HttpContext.Request.Query["colorFilter"]) != null)
            {
                colorFilter = Convert.ToString(HttpContext.Request.Query["colorFilter"]).Trim();

                ViewBag.ColorFilter = colorFilter;
            }

            if (!string.IsNullOrEmpty(colorFilter))
            {
                pets = pets.Where(p => p.Color == colorFilter).ToList();
            }

            if (Convert.ToString(HttpContext.Request.Query["locationFilter"]) != null)
            {
                locationFilter = Convert.ToString(HttpContext.Request.Query["locationFilter"]).Trim();

                ViewBag.LocationFilter = locationFilter;
            }

            if (!string.IsNullOrEmpty(locationFilter))
            {
                pets = pets.Where(p => p.Location == locationFilter).ToList();
            }

            if (Convert.ToString(HttpContext.Request.Query["ageFilter"]) != null)
            {
                ageFilter = Convert.ToString(HttpContext.Request.Query["ageFilter"]).Trim();

                ViewBag.AgeFilter = ageFilter;
            }

            if (!string.IsNullOrEmpty(ageFilter))
            {
                pets = pets.Where(p => p.Age.ToString() == ageFilter).ToList();
            }

            if (Convert.ToString(HttpContext.Request.Query["sizeFilter"]) != null)
            {
                sizeFilter = Convert.ToString(HttpContext.Request.Query["sizeFilter"]).Trim();

                ViewBag.SizeFilter = sizeFilter;
            }

            if (!string.IsNullOrEmpty(sizeFilter))
            {
                pets = pets.Where(p => p.Size.ToString() == sizeFilter).ToList();
            }

            if (Convert.ToString(HttpContext.Request.Query["sexFilter"]) != null)
            {
                sexFilter = Convert.ToString(HttpContext.Request.Query["sexFilter"]).Trim();

                ViewBag.SexFilter = sexFilter;
            }

            if (!string.IsNullOrEmpty(sexFilter))
            {
                pets = pets.Where(p => (p.Sex == false && sexFilter == "Femela") || (p.Sex == true && sexFilter == "Mascul")).ToList();
            }

            if (Convert.ToString(HttpContext.Request.Query["vaccineFilter"]) != null)
            {
                vaccineFilter = Convert.ToString(HttpContext.Request.Query["vaccineFilter"]).Trim();

                ViewBag.VaccineFilter = vaccineFilter;
            }


            if (!string.IsNullOrEmpty(vaccineFilter))
            {
                pets = pets.Where(p => (p.Vaccined == false && vaccineFilter == "Nevaccinat") || 
                                       (p.Vaccined == true && vaccineFilter == "Vaccinat")).ToList();
            }

            if (Convert.ToString(HttpContext.Request.Query["sterilizeFilter"]) != null)
            {
                sterilizeFilter = Convert.ToString(HttpContext.Request.Query["sterilizeFilter"]).Trim();

                ViewBag.SterilizeFilter = sterilizeFilter;
            }

            if (!string.IsNullOrEmpty(sterilizeFilter))
            {
                pets = pets.Where(p => (p.Sterilized == false && sterilizeFilter == "Nesterilizat") || 
                                 (p.Sterilized == true && sterilizeFilter == "Sterilizat")).ToList();
            }


            int _perPage = 12; //numarul pe articole per pagina

            if (TempData.ContainsKey("message") && TempData["message"] != null)
            {
                ViewBag.message = TempData["message"].ToString();
            }

            int totalItems = pets.Count(); //verificam de fiecare data, e un nr variabil de anunturi

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]); //se preia pagina curenta din view-ul asocial (val. param. page din ruta)

            var offset = 0; //offset 0 pt prima pagina

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage; //calculam offset-ul pt celelalte pagini
            }

            var paginatedPets = pets.Skip(offset).Take(_perPage); //se preiau articolele dupa offset

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage); //ultima pagina

            ViewBag.Pets = paginatedPets;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = $"/Pets/Index/?search={search}&speciesFilter={speciesFilter}&breedFilter={breedFilter}&colorFilter={colorFilter}&locationFilter={locationFilter}&ageFilter={ageFilter}&sizeFilter={sizeFilter}&sexFilter={sexFilter}&vaccineFilter={vaccineFilter}&sterilizeFilter={sterilizeFilter}&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = $"/Pets/Index/?speciesFilter={speciesFilter}&breedFilter={breedFilter}&colorFilter={colorFilter}&locationFilter={locationFilter}&ageFilter={ageFilter}&sizeFilter={sizeFilter}&sexFilter={sexFilter}&vaccineFilter={vaccineFilter}&sterilizeFilter={sterilizeFilter}&page";
            }

            return View();

        }
        public ActionResult Show(int id)
        {
            Pet pet = _db.Pets.First(pet => pet.PetId == id);

            return View(pet);
        }

        public IActionResult New()
        {
            Pet pet = new Pet();

            return View(pet);
        }

        [HttpPost]
        public async Task<IActionResult> New(Pet pet, IFormFile PetImage)
        {
            var databaseFileName = "";
            if (ModelState.IsValid && PetImage != null)
            {
                if (PetImage.Length > 0)
                {
                    // Generam calea de stocare a fisierului
                    var storagePath = Path.Combine(
                    _env.WebRootPath, // Luam calea folderului wwwroot
                    "images", // Adaugam calea folderului images
                    PetImage.FileName // Numele fisierului
                    );

                    databaseFileName = "/images/" + PetImage.FileName;
                    // Uploadam fisierul la calea de storage
                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await PetImage.CopyToAsync(fileStream);
                    }
                }

                //Salvam storagePath-ul in baza de date

                pet.Image = databaseFileName;
                _db.Pets.Add(pet);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(pet);
        }

        public IActionResult Edit(int id)
        {
            Pet pet = _db.Pets.Find(id);

            return View(pet);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, Pet requestPet, IFormFile PetImage)
        {
            Pet pet = _db.Pets.Find(id);

            try
            {
                pet.Name = requestPet.Name;
                pet.Species = requestPet.Species;
                pet.Breed = requestPet.Breed;
                pet.Age = requestPet.Age;
                pet.Size = requestPet.Size;
                pet.Sex = requestPet.Sex;
                pet.Color = requestPet.Color;
                pet.Vaccined = requestPet.Vaccined;
                pet.Sterilized = requestPet.Sterilized;
                pet.Description = requestPet.Description;
                pet.Location = requestPet.Location;

                if (PetImage != null && PetImage.Length > 0)
                {
                    // Generați o nouă cale pentru imagine
                    var storagePath = Path.Combine(
                        _env.WebRootPath,
                        "images",
                        PetImage.FileName
                    );

                    // Încărcați imaginea la calea de stocare
                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await PetImage.CopyToAsync(fileStream);
                    }

                    // Actualizați calea imaginii în obiectul Pet
                    pet.Image = "/images/" + PetImage.FileName;
                }

                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            Pet pet = _db.Pets.Where(pet => pet.PetId == id)
                             .First();

            _db.Pets.Remove(pet);
            _db.SaveChanges();
            TempData["message"] = "Anuntul  a fost sters";

            return RedirectToAction("Index");
        }


        //functions to get lists of pets for dropdown filtering
        public List<string> GetDistinctSpecies()
        {
            return _db.Pets.Select(p => p.Species).Distinct().ToList(); //gets a list of distinct species found in Pets table
        }

        public List<string> GetDistinctBreed()
        {
            return _db.Pets.Select(p => p.Breed).Distinct().ToList();
        }

        public List<string> GetDistinctColor()
        {
            return _db.Pets.Select(p => p.Color).Distinct().ToList();
        }
        public List<string> GetDistinctLocation()
        {
            return _db.Pets.Select(p => p.Location).Distinct().ToList();
        }

        public List<string> GetDistinctAge()
        {
            return _db.Pets.Select(p => (p.Age).ToString()).Distinct().ToList();
        }

        public List<string> GetDistinctSize()
        {
            return _db.Pets.Select(p => (p.Size).ToString()).Distinct().ToList();
        }

        public List<string> GetDistinctSex()
        {
            var distinctSexValues = _db.Pets.Select(p => p.Sex).Distinct().ToList();

            List<string> sexList = new List<string>();

            foreach (var sexValue in distinctSexValues)
            {
                string sexString = !sexValue ? "Femela" : "Mascul";
                sexList.Add(sexString);
            }

            return sexList;
        }

        public List<string> GetDistinctVaccined()
        {
            var distinctVaccinevalues = _db.Pets.Select(p => p.Vaccined).Distinct().ToList();

            List<string> vaccineList = new List<string>();

            foreach (var vaccval in distinctVaccinevalues)
            {
                string vaccstring = !vaccval ? "Nevaccinat" : "Vaccinat";
                vaccineList.Add(vaccstring);
            }

            return vaccineList;
        }

        public List<string> GetDistinctSterilized()
        {
            var distinctSterilizedvalues = _db.Pets.Select(p => p.Sterilized).Distinct().ToList();

            List<string> sterList = new List<string>();

            foreach (var sterval in distinctSterilizedvalues)
            {
                string vaccstring = !sterval ? "Nesterilizat" : "Sterilizat";
                sterList.Add(vaccstring);
            }

            return sterList;
        }

    }
}
