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
    }
}
