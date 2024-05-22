using System.Linq;
using System.Collections.Generic;
using Moq;
using AutoFixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using QA_Project.Controllers;
using Microsoft.EntityFrameworkCore;
using QA_Project.Data;
using QA_Project.Models;
using Xunit;
using Assert = Xunit.Assert;


namespace QATestProject
{
    public class PetsControllerShould
    {
        private readonly Fixture _fixture;
        private readonly ApplicationDbContext _context;
        private Mock<IWebHostEnvironment> _mockEnvironment;

        public PetsControllerShould()
        {
            _fixture = new Fixture();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);
            _context = context;
            _mockEnvironment = new Mock<IWebHostEnvironment>();
        }

        [Fact]
        public async Task Index_NoFilter_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.CreateMany<Pet>(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Null(viewBag.SpeciesFilter);
            Assert.Null(viewBag.BreedFilter);
            Assert.Null(viewBag.AgeFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task Index_FilterSpecies_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Dog")
                .CreateMany(5).ToList();

            pets.Add(_fixture.Build<Pet>()
                .With(p => p.Species, "Cat")
                .Create());

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"speciesFilter", "Dog"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal("Dog", viewBag.SpeciesFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task Index_FilterBreed_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Breed, "Golden Retriever")
                .CreateMany(5).ToList();

            pets.Add(_fixture.Build<Pet>()
                .With(p => p.Breed, "Poodle")
                .Create());

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"breedFilter", "Golden Retriever"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal("Golden Retriever", viewBag.BreedFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task Index_FilterAge_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();

            pets.Add(_fixture.Build<Pet>()
                .With(p => p.Age, 3)
                .Create());

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"ageFilter", "2"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equivalent(2, viewBag.AgeFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task Index_SpeciesAndBreedFilter_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Lizard")
                .With(p => p.Breed, "Gecko")
                .CreateMany(5).ToList();

            pets.Add(_fixture.Build<Pet>()
                .With(p => p.Species, "Mouse")
                .With(p => p.Breed, "Big Mouse")
                .Create());

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"speciesFilter", "Lizard"},
                {"breedFilter", "Gecko"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal("Lizard", viewBag.SpeciesFilter);
            Assert.Equal("Gecko", viewBag.BreedFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task Index_SpeciesAndAgeFilter_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Lizard")
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();

            pets.Add(_fixture.Build<Pet>()
                .With(p => p.Species, "Mouse")
                .With(p => p.Age, 3)
                .Create());

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"speciesFilter", "Lizard"},
                {"ageFilter", "2"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal("Lizard", viewBag.SpeciesFilter);
            Assert.Equivalent(2, viewBag.AgeFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task Index_BreedAndAgeFilter_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Breed, "Golden Retriever")
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();

            pets.Add(_fixture.Build<Pet>()
                .With(p => p.Breed, "Poodle")
                .With(p => p.Age, 3)
                .Create());

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"breedFilter", "Golden Retriever"},
                {"ageFilter", "2"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal("Golden Retriever", viewBag.BreedFilter);
            Assert.Equivalent(2, viewBag.AgeFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task Index_SpeciesBreedAndAgeFilter_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Lizard")
                .With(p => p.Breed, "Gecko")
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();

            pets.Add(_fixture.Build<Pet>()
                .With(p => p.Species, "Mouse")
                .With(p => p.Breed, "Big Mouse")
                .With(p => p.Age, 3)
                .Create());

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"speciesFilter", "Lizard"},
                {"breedFilter", "Gecko"},
                {"ageFilter", "2"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal("Lizard", viewBag.SpeciesFilter);
            Assert.Equal("Gecko", viewBag.BreedFilter);
            Assert.Equivalent(2, viewBag.AgeFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task SpeciesFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Lizard")
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"speciesFilter", "Dog"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.NotEqual("Lizard", viewBag.SpeciesFilter);
            Assert.Empty(viewBag.Pets);
        }

        [Fact]
        public async Task BreedFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Breed, "Golden Retriever")
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"breedFilter", "Poodle"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.NotEqual("Golden Retriever", viewBag.BreedFilter);
            Assert.Empty(viewBag.Pets);
        }

        [Fact]
        public async Task AgeFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"ageFilter", "3"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.NotEqual("2", viewBag.AgeFilter);
            Assert.Empty(viewBag.Pets);
        }

        [Fact]
        public async Task SpeciesAndBreedFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Lizard")
                .With(p => p.Breed, "Gecko")
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"speciesFilter", "Mouse"},
                {"breedFilter", "Big Mouse"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.NotEqual("Lizard", viewBag.SpeciesFilter);
            Assert.NotEqual("Gecko", viewBag.BreedFilter);
            Assert.Empty(viewBag.Pets);
        }

        [Fact]
        public async Task SpeciesAndAgeFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Lizard")
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"speciesFilter", "Mouse"},
                {"ageFilter", "3"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.NotEqual("Lizard", viewBag.SpeciesFilter);
            Assert.NotEqual("2", viewBag.AgeFilter);
            Assert.Empty(viewBag.Pets);
        }

        [Fact]
        public async Task BreedAndAgeFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Breed, "Golden Retriever")
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"breedFilter", "Poodle"},
                {"ageFilter", "3"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.NotEqual("Golden Retriever", viewBag.BreedFilter);
            Assert.NotEqual("2", viewBag.AgeFilter);
            Assert.Empty(viewBag.Pets);
        }

        [Fact]
        public async Task SpeciesBreedAndAgeFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Lizard")
                .With(p => p.Breed, "Gecko")
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                {"speciesFilter", "Mouse"},
                {"breedFilter", "Big Mouse"},
                {"ageFilter", "3"}
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.NotEqual("Lizard", viewBag.SpeciesFilter);
            Assert.NotEqual("Gecko", viewBag.BreedFilter);
            Assert.NotEqual("2", viewBag.AgeFilter);
            Assert.Empty(viewBag.Pets);
        }

        // Test case for specie filter with minimum and maximum values
        [Fact]
        public async Task Index_SpeciesFilter_MinValues_ReturnsViewResult()
        {
            // Arrange
            var minSpecies = "aaa";

            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, minSpecies)
                .CreateMany(2).ToList();

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "speciesFilter", minSpecies }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(minSpecies, viewBag.SpeciesFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(2, petsResult.Count()); // 2 pets with minSpecies
        }

        [Fact]
        public async Task Index_SpeciesFilter_MaxValues_ReturnsViewResult()
        {
            // Arrange
            var maxSpecies = "zzz";

            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, maxSpecies)
                .CreateMany(2).ToList();

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "speciesFilter", maxSpecies }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(maxSpecies, viewBag.SpeciesFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(2, petsResult.Count()); // 2 pets with minSpecies
        }


        // Test cases for invalid specie filter 
        [Fact]
        public async Task Index_InvalidSpecieMinFilter_ReturnsViewResult()
        {
            // Arrange
            var invalidSpecieMin = "a"; // Non-existent specie

            var pets = _fixture.Build<Pet>().CreateMany(5);
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "speciesFilter", invalidSpecieMin }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(invalidSpecieMin, viewBag.SpeciesFilter); // Filter value is still set
            Assert.Empty(viewBag.Pets); // No pets should be returned with an invalid filter
        }

        [Fact]
        public async Task Index_InvalidSpecieMaxFilter_ReturnsViewResult()
        {
            // Arrange
            var invalidSpecieMax = "z"; // Non-existent specie

            var pets = _fixture.Build<Pet>().CreateMany(5);
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "speciesFilter", invalidSpecieMax }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(invalidSpecieMax, viewBag.SpeciesFilter); // Filter value is still set
            Assert.Empty(viewBag.Pets); // No pets should be returned with an invalid filter
        }

        // Test cases for breed filter with minimum and maximum values 
        [Fact]
        public async Task Index_BreedFilter_MinValues_ReturnsViewResult()
        {
            // Arrange
            var minBreed = "aaa";

            var pets = _fixture.Build<Pet>()
                .With(p => p.Breed, minBreed)
                .CreateMany(2).ToList();

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Creaza un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "breedFilter", minBreed }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(minBreed, viewBag.BreedFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(2, petsResult.Count()); // 2 pets with minBreed
        }

        [Fact]
        public async Task Index_BreedFilter_MaxValues_ReturnsViewResult()
        {
            // Arrange
            var maxBreed = "zzz";

            var pets = _fixture.Build<Pet>()
                .With(p => p.Breed, maxBreed)
                .CreateMany(2).ToList();

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Creaza un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "breedFilter", maxBreed }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(maxBreed, viewBag.BreedFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(2, petsResult.Count()); // 2 pets with minBreed
        }

        // Test cases for invalid specie filter 
        [Fact]
        public async Task Index_InvalidBreedMinFilter_ReturnsViewResult()
        {
            // Arrange
            var invalidBreedMin = "a"; // Non-existent breed

            var pets = _fixture.Build<Pet>().CreateMany(5);
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "breedFilter", invalidBreedMin }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(invalidBreedMin, viewBag.BreedFilter); // Filter value is still set
            Assert.Empty(viewBag.Pets); // No pets should be returned with an invalid filter
        }

        [Fact]
        public async Task Index_InvalidBreedMaxFilter_ReturnsViewResult()
        {
            // Arrange
            var invalidBreedMin = "z"; // Non-existent breed

            var pets = _fixture.Build<Pet>().CreateMany(5);
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "breedFilter", invalidBreedMin }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(invalidBreedMin, viewBag.BreedFilter); // Filter value is still set
            Assert.Empty(viewBag.Pets); // No pets should be returned with an invalid filter
        }

        // Test cases for year filter with minimum and maximum values
        [Fact]
        public async Task Index_AgeFilter_MinValues_ReturnsViewResult()
        {
            // Arrange
            var minAge = 0; // Minimum possible age

            var pets = _fixture.Build<Pet>()
                .With(p => p.Age, minAge)
                .CreateMany(2).ToList();

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "ageFilter", minAge.ToString() }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(minAge.ToString(), viewBag.AgeFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(2, petsResult.Count()); // 2 pets with minAge
        }

        [Fact]
        public async Task Index_AgeFilter_MaxValues_ReturnsViewResult()
        {
            // Arrange
            var maxAge = 0; // Minimum possible age

            var pets = _fixture.Build<Pet>()
                .With(p => p.Age, maxAge)
                .CreateMany(2).ToList();

            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Crează un HttpContext default
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "ageFilter", maxAge.ToString() }
            });
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal(maxAge.ToString(), viewBag.AgeFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(2, petsResult.Count()); // 2 pets with minAge
        }

    }
}



