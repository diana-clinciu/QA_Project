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
    public class PetViews
    {
        private readonly Fixture _fixture;
        private readonly ApplicationDbContext _context;
        private Mock<IWebHostEnvironment> _mockEnvironment;

        public PetViews()
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
        public async Task Index_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            var pets = _fixture.CreateMany<Pet>(25).ToList(); // Assuming _perPage = 12
            await _context.AddRangeAsync(pets);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "page", "2" } // Requesting the second page
            });

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model); // Ensure the model is not null
            var model = Assert.IsAssignableFrom<IEnumerable<Pet>>(viewResult.Model); // Correctly assert and cast the model type
            Assert.Equal(12, model.Count()); // Should return the second set of 12 pets for the second page
        }

        [Fact]
        public async Task Show_ReturnsViewResult_WithPet()
        {
            // Arrange
            var pet = _fixture.Create<Pet>();
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);

            // Act
            var result = controller.Show(pet.PetId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Pet>(viewResult.Model);
            Assert.Equal(pet.PetId, model.PetId);
        }


        [Fact]
        public void Show_PetNotFound_ReturnsNotFound()
        {
            // Arrange
            var controller = new PetsController(_context, _mockEnvironment.Object);

            // Act
            var result = controller.Show(999); 

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }



        [Fact]
        public async Task New_ReturnsView_WhenCalledWithHttpGet()
        {
            // Arrange
            var controller = new PetsController(_context, _mockEnvironment.Object);

            // Act
            var result = controller.New();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Pet>(viewResult.Model);
        }
        [Fact]
        public async Task New_AddsPetAndRedirects_WhenModelStateIsValid()
        {
            // Arrange
            var pet = _fixture.Create<Pet>();
            var controller = new PetsController(_context, _mockEnvironment.Object);
            var mockFile = new Mock<IFormFile>();
            // Setup mock file if necessary

            // Act
            var result = await controller.New(pet, mockFile.Object);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(1, await _context.Pets.CountAsync());
        }
        [Fact]
        public async Task New_UploadsImageAndSavesPetWithImagePath()
        {
            // Arrange
            var fixture = new Fixture();
            var pet = fixture.Create<Pet>();
            var mockFile = new Mock<IFormFile>();
            var fileName = "puffy.jpeg";
            var content = "Imagine test";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            _mockEnvironment.Setup(_ => _.WebRootPath).Returns("path/to/wwwroot");

            var controller = new PetsController(_context, _mockEnvironment.Object);

            // Act
            var result = await controller.New(pet, mockFile.Object);

            // Assert
            var savedPet = await _context.Pets.FirstOrDefaultAsync(p => p.Name == pet.Name);
            Assert.NotNull(savedPet);
            Assert.Equal("/images/puffy.jpeg", savedPet.Image);
        }

        [Fact]
        public async Task New_Post_InvalidModelState_ReturnsViewWithSamePet()
        {
            // Arrange
            var controller = new PetsController(_context, _mockEnvironment.Object);
            var pet = _fixture.Create<Pet>();
            controller.ModelState.AddModelError("Name", "Required"); // Simulating an invalid ModelState

            // Act
            var result = await controller.New(pet, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Pet>(viewResult.Model);
            Assert.Equal(pet.Name, model.Name); // Verify the returned model is the same as the submitted one
        }

        [Fact]
        public async Task Edit_ReturnsView_WithPet()
        {
            // Arrange
            var pet = _fixture.Create<Pet>();
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            var controller = new PetsController(_context, _mockEnvironment.Object);

            // Act
            var result = controller.Edit(pet.PetId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Pet>(viewResult.Model);
            Assert.Equal(pet.PetId, model.PetId);
        }

        [Fact]
        public async Task Edit_UpdatesPetAndRedirects_WhenModelStateIsValid()
        {
            // Arrange
            var pet = _fixture.Create<Pet>();
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            var controller = new PetsController(_context, _mockEnvironment.Object);
            var updatedPet = _fixture.Build<Pet>().With(p => p.PetId, pet.PetId).Create();
            var mockFile = new Mock<IFormFile>();
            // Setup mock file if necessary

            // Act
            var result = await controller.Edit(pet.PetId, updatedPet, mockFile.Object);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            var updatedEntry = await _context.Pets.FindAsync(pet.PetId);
            Assert.Equal(updatedPet.Name, updatedEntry.Name); // Validate one property for brevity; extend as needed
        }

        [Fact]
        public async Task Delete_RemovesPetAndRedirects()
        {
            // Arrange
            var pet = _fixture.Create<Pet>();
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            var controller = new PetsController(_context, _mockEnvironment.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>()); // Initialize TempData

            // Act
            var result = controller.Delete(pet.PetId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(0, await _context.Pets.CountAsync());
        }
        [Fact]
        public void GetDistinctSpecies_ReturnsDistinctSpecies()
        {
            // Arrange
            var fixture = new Fixture();
            var speciesList = new List<Pet>
            {
                fixture.Build<Pet>().With(p => p.Species, "Dog").Create(),
                fixture.Build<Pet>().With(p => p.Species, "Cat").Create(),
                fixture.Build<Pet>().With(p => p.Species, "Dog").Create() // Intentional duplicate
            };
            _context.Pets.AddRange(speciesList);
            _context.SaveChanges();

            var controller = new PetsController(_context, _mockEnvironment.Object);

            // Act
            var result = controller.GetDistinctSpecies();

            // Assert
            Assert.Equal(2, result.Count()); // Should only contain distinct "Dog" and "Cat"
            Assert.Contains("Dog", result);
            Assert.Contains("Cat", result);
        }


    }
}
