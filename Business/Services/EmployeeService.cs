using Domain.Employee;
using Persistence.Infrastructure;
using Persistence.Repositories;

namespace Business.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeListItemModel>> GetAllEmployeesAsync();
    Task<int> CreateEmployeeAsync(CreateEmployeeRequestModel requestModel);
    Task UpdateEmployeeByIdAsync(int id, UpdateEmployeeRequestModel model);
    Task DeleteEmployeeByIdAsync(int id);
}

public class EmployeeService(IDbContext dbContext, IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<IEnumerable<EmployeeListItemModel>> GetAllEmployeesAsync()
    {
        return await employeeRepository.GetAllEmployeesAsync();
    }

    public async Task<int> CreateEmployeeAsync(CreateEmployeeRequestModel requestModel)
    {
        var employeeId = await employeeRepository.CreateEmployeeAsync(requestModel);
        dbContext.Commit();

        return employeeId;
    }
    
    public async Task UpdateEmployeeByIdAsync(int id, UpdateEmployeeRequestModel model)
    {
        await employeeRepository.UpdateEmployeeAsync(id, model);
        dbContext.Commit();
    }

    public async Task DeleteEmployeeByIdAsync(int id)
    {
        await employeeRepository.DeleteEmployeeAsync(id);
        dbContext.Commit();
    }
}