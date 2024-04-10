using System.Text;
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
    public class PetsControllerIfStatementShould
    {
        private readonly Fixture _fixture;
        private readonly ApplicationDbContext _context;
        private Mock<IWebHostEnvironment> _mockEnvironment;

        public PetsControllerIfStatementShould()
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
        public async Task Index_SpeciesFilter_ReturnsViewResult()
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
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "speciesFilter", "Dog" }
                });
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

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
        public async Task Index_SpeciesFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Species, "Dog")
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "speciesFilter", "Cat" }
                });
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.NotEqual("Dog", viewBag.SpeciesFilter);
            Assert.Empty(viewBag.Pets);
        }

        [Fact]
        public async Task Index_BreedFilter_ReturnsViewResult()
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
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "breedFilter", "Golden Retriever" }
                });
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

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
        public async Task Index_BreedFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Breed, "Golden Retriever")
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "breedFilter", "Poodle" }
                });
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

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
        public async Task Index_AgeFilter_ReturnsViewResult()
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
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "ageFilter", "2" }
                });
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal("2", viewBag.AgeFilter);
            Assert.NotEmpty(viewBag.Pets);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(5, petsResult.Count());
        }

        [Fact]
        public async Task Index_AgeFilterNotFound_ReturnsViewResult()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Age, 2)
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "ageFilter", "3" }
                });
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

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
        public async Task Index_WhenTempDataMessageIsNotNull_SetsViewBagMessage()
        {
            // Arrange
            var pets = _fixture.Build<Pet>()
                .With(p => p.Sterilized, true) // true for Sterilized
                .CreateMany(5).ToList();
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>())
                {
                    ["message"] = "Test message"
                };

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Equal("Test message", viewBag.message);
        }

        [Fact]
        public async Task Index_WhenCurrentPageIsNotZero_CalculatesOffsetCorrectly()
        {
            // Arrange
            var pets = _fixture.CreateMany<Pet>(20).ToList(); // Create more than one page of pets
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "page", "2" } // Set current page to 2
                });
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            IEnumerable<Pet> petsResult = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewBag.Pets);
            Assert.Equal(8, petsResult.Count()); // Assert that the second page of pets is displayed
        }

        [Fact]
        public async Task Index_WhenSearchIsNotEmpty_SetsPaginationBaseUrlWithSearch()
        {
            // Arrange
            var pets = _fixture.CreateMany<Pet>(20).ToList(); // Create more than one page of pets
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "search", "TestSearch" }
                }
            );
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.Contains("TestSearch", viewBag.PaginationBaseUrl);
        }

        [Fact]
        public async Task Index_WhenSearchIsEmpty_SetsPaginationBaseUrlWithoutSearch()
        {
            // Arrange
            var pets = _fixture.CreateMany<Pet>(20).ToList(); // Create more than one page of pets
            _context.Pets.AddRange(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "search", "" } // Set search to an empty string
                });
            controller.TempData =
                new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBag = controller.ViewBag;
            Assert.NotNull(viewBag);
            Assert.DoesNotContain("search=", viewBag.PaginationBaseUrl);
        }

        [Fact]
        public async Task New_WhenModelStateIsInvalid_ReturnsViewResultWithPet()
        {
            // Arrange
            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.New(new Pet(), null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Pet>(viewResult.Model);
        }

        [Fact]
        public async Task New_WhenPetImageIsNull_ReturnsViewResultWithPet()
        {
            // Arrange
            var controller = new PetsController(_context, _mockEnvironment.Object);

            // Act
            var result = await controller.New(new Pet(), null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Pet>(viewResult.Model);
        }

        [Fact]
        public async Task New_WhenPetImageLengthIsZero_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("testpath"); // Set a dummy value for WebRootPath
            var controller = new PetsController(_context, mockEnvironment.Object);
            var mockFile = new Mock<IFormFile>();
            var sourceImg = "";
            var byteArray = Encoding.ASCII.GetBytes(sourceImg);
            var stream = new MemoryStream(byteArray);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.Length).Returns(0);

            var pet = new Pet
            {
                Name = "Test Name",
                Species = "Test Species",
                Breed = "Test Breed",
                Color = "Test Color",
                Description = "Test Description",
                Location = "Test Location"
            };

            // Act
            var result = await controller.New(pet, mockFile.Object);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task New_WhenPetImageLengthIsGreaterThanZero_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("testpath"); // Set a dummy value for WebRootPath
            var controller = new PetsController(_context, mockEnvironment.Object);
            var mockFile = new Mock<IFormFile>();
            var sourceImg =
                "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="; // A simple red dot image
            var byteArray = Convert.FromBase64String(sourceImg);
            var stream = new MemoryStream(byteArray);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.Length).Returns(stream.Length);
            mockFile.Setup(f => f.FileName).Returns("test.jpg"); // Set a dummy value for FileName

            var pet = new Pet
            {
                // Set required properties
                Name = "Test Name",
                Species = "Test Species",
                Breed = "Test Breed",
                Age = 1,
                Size = 1,
                Sex = true,
                Color = "Test Color",
                Description = "Test Description",
                Location = "Test Location"
            };

            // Act
            var result = await controller.New(pet, mockFile.Object);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_WhenPetImageIsNull_DoesNotUpdateImage()
        {
            // Arrange
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("testpath");
            var controller = new PetsController(_context, mockEnvironment.Object);
            var pet = new Pet
            {
                // Set required properties
                Name = "Test Name",
                Species = "Test Species",
                Breed = "Test Breed",
                Age = 1,
                Size = 1,
                Sex = true,
                Color = "Test Color",
                Description = "Test Description",
                Location = "Test Location"
            };
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            // Act
            var result = await controller.Edit(pet.PetId, pet, null);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            var updatedPet = _context.Pets.Find(pet.PetId);
            Assert.Null(updatedPet.Image);
        }

        [Fact]
        public async Task Edit_WhenExceptionIsThrown_RedirectsToIndex()
        {
            // Arrange
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("testpath");
            var controller = new PetsController(_context, mockEnvironment.Object);
            var pet = new Pet
            {
                // Set required properties
                Name = "Test Name",
                Species = "Test Species",
                Breed = "Test Breed",
                Age = 1,
                Size = 1,
                Sex = true,
                Color = "Test Color",
                Description = "Test Description",
                Location = "Test Location"
            };
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.jpg");
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            // Act
            var result = await controller.Edit(pet.PetId, pet, mockFile.Object);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}