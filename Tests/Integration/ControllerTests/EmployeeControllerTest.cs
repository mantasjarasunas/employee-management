using System.Net;
using Domain.Employee;
using FluentAssertions;
using Tests.Infrastructure;
using Xunit;

namespace Tests.Integration.ControllerTests
{
    public class ControllerControllerTests : ControllerIntegrationTestBase
    {
        public ControllerControllerTests(AppFactory fixture): base(fixture) {}

        [Fact]
        public async Task CreateEmployee_CorrectModel_ShouldCreateEmployee()
        {
            var request = new CreateEmployeeRequestModel
            {
                LastName = "Jarašūnas",
                FirstName = "Mantas",
                Title = "Programuotojas",
                TitleOfCourtesy = "P.",
                BirthDate = DateTime.UtcNow.AddYears(-25),
                HireDate = DateTime.UtcNow,
                Address = "Gedimino pr. 1",
                City = "Vilnius",
                Region = "Vilniaus",
                PostalCode = "01103",
                Country = "Lietuva",
                HomePhone = "+37060000000",
                Extension = "101",
                Photo = "iVBORw0KGgoAAAANSUhEUgAAAAUA",
                Notes = "Lietuviškas testas",
                ReportsTo = 1,
                PhotoPath = "/photos/mantas_jarasunas.png"
            };

            var response = await ApiPost<int>("/employee", request);

            response.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateEmployee_EmptyFirstName_ShouldReturnValidationError()
        {
            var request = new CreateEmployeeRequestModel
            {
                LastName = "Jarašūnas",
                FirstName = "",
                Title = "Programuotojas",
                TitleOfCourtesy = "P.",
                BirthDate = DateTime.UtcNow.AddYears(-25),
                HireDate = DateTime.UtcNow,
                Address = "Gedimino pr. 1",
                City = "Vilnius",
                Region = "Vilniaus",
                PostalCode = "01103",
                Country = "Lietuva",
                HomePhone = "+37060000000",
                Extension = "101",
                Photo = "iVBORw0KGgoAAAANSUhEUgAAAAUA",
                Notes = "Lietuviškas testas",
                ReportsTo = 1,
                PhotoPath = "/photos/mantas_jarasunas.png"
            };

            var response = await ApiPostReturnsValidationErrors("/employee", request);

            response.Errors.Should().ContainKey("FirstName");
            response.Errors["FirstName"].Should().Contain(e => e.Contains("must not be empty"));
        }

        [Fact]
        public async Task UpdateEmployee_ValidId_ShouldUpdateEmployee()
        {
            var request = new CreateEmployeeRequestModel
            {
                LastName = "Jarašūnas",
                FirstName = "Mantas",
                Title = "Programuotojas",
                TitleOfCourtesy = "P.",
                BirthDate = DateTime.UtcNow.AddYears(-25),
                HireDate = DateTime.UtcNow,
                Address = "Gedimino pr. 1",
                City = "Vilnius",
                Region = "Vilniaus",
                PostalCode = "01103",
                Country = "Lietuva",
                HomePhone = "+37060000000",
                Extension = "101",
                Photo = "iVBORw0KGgoAAAANSUhEUgAAAAUA",
                Notes = "Lietuviškas testas",
                ReportsTo = 1,
                PhotoPath = "/photos/mantas_jarasunas.png"
            };

            var employeeId = await ApiPost<int>("/employee", request);

            var updateRequest = new UpdateEmployeeRequestModel
            {
                LastName = "Jarašūnas",
                FirstName = "Mantas",
                Title = "Vyresnysis programuotojas",
                TitleOfCourtesy = "P.",
                BirthDate = DateTime.UtcNow.AddYears(-25),
                HireDate = DateTime.UtcNow,
                Address = "Konstitucijos pr. 7",
                City = "Vilnius",
                Region = "Vilniaus",
                PostalCode = "09308",
                Country = "Lietuva",
                HomePhone = "+37060000000",
                Extension = "101",
                Photo = "iVBORw0KGgoAAAANSUhEUgAAAAUA",
                Notes = "Atnaujintas testas",
                ReportsTo = 2,
                PhotoPath = "/photos/mantas_jarasunas_v2.png"
            };

            await ApiPut<EmployeeDetailsModel>($"/employee/{employeeId}", updateRequest);

            var allEmployees = await ApiGet<IEnumerable<EmployeeListItemModel>>("/employee");
            var updatedEmployee = allEmployees?.FirstOrDefault(x => x.EmployeeId == employeeId);

            updatedEmployee.Should().NotBeNull();
            updatedEmployee?.Title.Should().Be("Vyresnysis programuotojas");
            updatedEmployee?.Address.Should().Be("Konstitucijos pr. 7");
            updatedEmployee?.PostalCode.Should().Be("09308");
        }

        [Fact]
        public async Task DeleteEmployee_ValidId_ShouldRemoveEmployee()
        {
            var request = new CreateEmployeeRequestModel
            {
                LastName = "Jarašūnas",
                FirstName = "Mantas",
                Title = "Programuotojas",
                TitleOfCourtesy = "P.",
                BirthDate = DateTime.UtcNow.AddYears(-25),
                HireDate = DateTime.UtcNow,
                Address = "Gedimino pr. 1",
                City = "Vilnius",
                Region = "Vilniaus",
                PostalCode = "01103",
                Country = "Lietuva",
                HomePhone = "+37060000000",
                Extension = "101",
                Photo = "iVBORw0KGgoAAAANSUhEUgAAAAUA",
                Notes = "Lietuviškas testas",
                ReportsTo = 1,
                PhotoPath = "/photos/mantas_jarasunas.png"
            };

            var employeeId = await ApiPost<int>("/employee", request);

            var response = await _client.DeleteAsync($"/employee/{employeeId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var allEmployees = await ApiGet<IEnumerable<EmployeeListItemModel>>("/employee");
            var deletedEmployee = allEmployees?.FirstOrDefault(x => x.EmployeeId == employeeId);
            deletedEmployee.Should().BeNull();
        }
    }
}