using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeacherApi.Controllers;
using TeacherApi.Models;

namespace TeacherApi.Tests
{
    [TestFixture]
    public class TeachersControllerTests
    {
        private TeachersController controller;
        private TeachersContext teachersContext;
        private DbContextOptions<TeachersContext> options;

        [OneTimeSetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<TeachersContext>().UseInMemoryDatabase("TeachersList").Options;
            teachersContext = new TeachersContext(options);
            controller = new TeachersController(teachersContext);
            InitTeachersContexWithValues();
        }

        private void InitTeachersContexWithValues()
        {
            teachersContext.Teachers.Add(new Teacher { Name = "Vanya", Address = "Kyiv", IsWorking = true, Secret = "Vanya's secret" });
            teachersContext.Teachers.Add(new Teacher { Name = "Tanya", Address = "Dnipro", IsWorking = false, Secret = "Tanya's secret" });
            teachersContext.Teachers.Add(new Teacher { Name = "Sanya", Address = "Lviv", IsWorking = true, Secret = "Sanya's secret" });
            teachersContext.Teachers.Add(new Teacher { Name = "Danya", Address = "Lviv", IsWorking = false, Secret = "Danya's secret" });
            teachersContext.Teachers.Add(new Teacher { Name = "Jenya", Address = "Kyiv", IsWorking = true, Secret = "Jenya's secret" });
            teachersContext.SaveChanges();
        }

        #region Action GetTeachers tests
        [Test]
        public async Task GetTeachers_WhenCalled_ReturnsListOfTeachers()
        {
            // Arrange

            // Act
            var result = await controller.GetTeachers().ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<ActionResult<IEnumerable<TeacherDTO>>>(result);
            Assert.That(result.Value, Is.Not.Null);

        }
        #endregion

        #region Action GetTeacher(id) tests
        [Test]
        public async Task GetTeacher_WhenIdNotExistsInDatabase_ReturnsStatusCode404()
        {
            // Arrange

            // Act
            var result = await controller.GetTeacher(10).ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(404, (result.Result as StatusCodeResult).StatusCode);
        }

        [Test]
        public async Task GetTeacher_WhenIdIsInvalidValue_ReturnsStatusCode400()
        {
            // Arrange

            // Act
            var result = await controller.GetTeacher(-1).ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(400, (result.Result as StatusCodeResult).StatusCode);
        }

        [Test]
        public async Task GetTeacher_WhenIdExistsInDatabase_ReturnsTeacher()
        {
            // Arrange

            // Act
            var result = await controller.GetTeacher(2).ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<ActionResult<TeacherDTO>>(result);
            Assert.That(result.Value, Is.Not.Null);
            Assert.AreEqual(result.Value.Id, 2);
        }
        #endregion

        #region Action PutTeacher(id) tests
        [Test]
        public async Task PutTeacher_WhenIdNotExistsInDatabase_ReturnsStatusCode404()
        {
            // Arrange
            long id = 10;
            var teacherDTO = new TeacherDTO { Id = 10, Name = "SanyaNew", Address = "Lviv", IsWorking = true };

            // Act
            var result = await controller.PutTeacher(id, teacherDTO).ConfigureAwait(false) as StatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task PutTeacher_WhenIdInQueryIsNotEqualToIdInModel_ReturnsStatusCode400()
        {
            // Arrange
            long id = 2;
            var teacherDTO = new TeacherDTO { Id = 3, Name = "SanyaNew", Address = "Lviv", IsWorking = true };

            // Act
            var result = await controller.PutTeacher(id, teacherDTO).ConfigureAwait(false) as StatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task PutTeacher_WhenIdExistsInDatabase_ReturnsStatusCode204AndUpdatesTheItem()
        {
            // Arrange
            long id = 3;
            var teacherDTO = new TeacherDTO { Id = 3, Name = "SanyaNew", Address = "Lviv", IsWorking = true };

            // Act
            var result = await controller.PutTeacher(id, teacherDTO).ConfigureAwait(false) as StatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(204, result.StatusCode);
            Assert.AreEqual(teacherDTO.Name, teachersContext.Teachers.Find(id).Name);
        }
        #endregion

        #region Action PostTeacher tests
        [Test]
        public async Task PostTeacher_InputValidModel_ReturnsStatusCode201AndNewlyCreatedItem()
        {
            // Arrange
            var teacherDTO = new TeacherDTO {Name = "Svetlana", Address = "Kyiv", IsWorking = true };
            var countbefore = await teachersContext.Teachers.CountAsync();

            // Act
            var result = await controller.PostTeacher(teacherDTO).ConfigureAwait(false);

            var actRes = result.Result as CreatedAtActionResult;
            var createdteacher = teachersContext.Teachers.Find((actRes.Value as TeacherDTO).Id);//actRes.Value as TeacherDTO;
            var countafter = await teachersContext.Teachers.CountAsync();

            // Assert
            Assert.AreEqual((countbefore + 1), countafter);
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<ActionResult<TeacherDTO>>(result);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            Assert.AreEqual(201, actRes.StatusCode);
            Assert.AreEqual(teacherDTO.Name, createdteacher.Name);
            Assert.AreEqual(teacherDTO.Address, createdteacher.Address);
            Assert.AreEqual(teacherDTO.IsWorking, createdteacher.IsWorking);
        }
        #endregion

        #region Action DeleteTeacher tests
        [Test]
        [TestCase(-1)]
        [TestCase(long.MaxValue)]
        public async Task DeleteTeacher_InputNotExistingIdInDataBase_ReturnsStatusCode404(long id)
        {
            // Arrange

            // Act
            var result = await controller.DeleteTeacher(id).ConfigureAwait(false) as StatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        [TestCase(1)]
        public async Task DeleteTeacher_InputExistingIdInDataBase_ReturnsStatusCode204(long id)
        {
            // Arrange

            // Act
            var result = await controller.DeleteTeacher(id).ConfigureAwait(false) as StatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(204, result.StatusCode);
        }
        #endregion
    }
}
