using Domain.Employee;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using Tests.Infrastructure;
using Xunit;

namespace Tests.Integration.RepositoryTests
{
    public class EmployeeRepositoryIntegrationTests(AppFactory fixture) : ControllerIntegrationTestBase(fixture)
    {
        private readonly IEmployeeRepository _employeeRepository = fixture.Services.CreateScope().ServiceProvider.GetRequiredService<IEmployeeRepository>();

        [Fact]
        public async Task CreateEmployeeAsync_ShouldInsertEmployee()
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

            var employeeId = await _employeeRepository.CreateEmployeeAsync(request);

            employeeId.Should().BeGreaterThan(0);

            var allEmployees = await _employeeRepository.GetAllEmployeesAsync();
            var createdEmployee = allEmployees.FirstOrDefault(x => x.EmployeeId == employeeId);

            createdEmployee.Should().NotBeNull();
            createdEmployee?.FirstName.Should().Be("Mantas");
            createdEmployee?.LastName.Should().Be("Jarašūnas");
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ShouldUpdateEmployee()
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

            var employeeId = await _employeeRepository.CreateEmployeeAsync(request);

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

            await _employeeRepository.UpdateEmployeeAsync(employeeId, updateRequest);

            var allEmployees = await _employeeRepository.GetAllEmployeesAsync();
            var updatedEmployee = allEmployees.FirstOrDefault(x => x.EmployeeId == employeeId);

            updatedEmployee.Should().NotBeNull();
            updatedEmployee?.Title.Should().Be("Vyresnysis programuotojas");
            updatedEmployee?.Address.Should().Be("Konstitucijos pr. 7");
            updatedEmployee?.PostalCode.Should().Be("09308");
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ShouldRemoveEmployee()
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

            var employeeId = await _employeeRepository.CreateEmployeeAsync(request);

            await _employeeRepository.DeleteEmployeeAsync(employeeId);

            var allEmployees = await _employeeRepository.GetAllEmployeesAsync();
            var deletedEmployee = allEmployees.FirstOrDefault(x => x.EmployeeId == employeeId);

            deletedEmployee.Should().BeNull();
        }
    }
}
