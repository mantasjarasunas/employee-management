using Dapper;
using Domain.Employee;
using Persistence.Infrastructure;

namespace Persistence.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<EmployeeListItemModel>> GetAllEmployeesAsync();
    Task<int> CreateEmployeeAsync(CreateEmployeeRequestModel model);
    Task UpdateEmployeeAsync(int id, UpdateEmployeeRequestModel model);
    Task DeleteEmployeeAsync(int id);
}

public class EmployeeRepository(IDbContext dbContext) : DbRepository(dbContext), IEmployeeRepository
{
    public Task<IEnumerable<EmployeeListItemModel>> GetAllEmployeesAsync()
    {
        const string query = "SELECT * FROM employees";

        return Connection.QueryAsync<EmployeeListItemModel>(query);
    }

    public async Task<int> CreateEmployeeAsync(CreateEmployeeRequestModel requestModel)
    {
        const string query = @"
            INSERT INTO employees (
                lastname,
                firstname,
                title,
                titleofcourtesy,
                birthdate,
                hiredate,
                address,
                city,
                region,
                postalcode,
                country,
                homephone,
                extension,
                photo,
                notes,
                reportsto,
                photopath
            ) 
            VALUES (
                @LastName,
                @FirstName,
                @Title,
                @TitleOfCourtesy,
                @BirthDate,
                @HireDate,
                @Address,
                @City,
                @Region,
                @PostalCode,
                @Country,
                @HomePhone,
                @Extension,
                decode(@Photo, 'base64'),
                @Notes,
                @ReportsTo,
                @PhotoPath
            )
            RETURNING employeeid;
        ";

        var employeeId = await Connection.ExecuteScalarAsync<int>(
            query,
            new
            {
                requestModel.LastName,
                requestModel.FirstName,
                requestModel.Title,
                requestModel.TitleOfCourtesy,
                requestModel.BirthDate,
                requestModel.HireDate,
                requestModel.Address,
                requestModel.City,
                requestModel.Region,
                requestModel.PostalCode,
                requestModel.Country,
                requestModel.HomePhone,
                requestModel.Extension,
                requestModel.Photo,
                requestModel.Notes,
                requestModel.ReportsTo,
                requestModel.PhotoPath
            }
        );

        return employeeId;
    }

    public async Task UpdateEmployeeAsync(int id, UpdateEmployeeRequestModel model)
    {
        const string query = @"
            UPDATE employees 
            SET 
                lastname = @LastName,
                firstname = @FirstName,
                title = @Title,
                titleofcourtesy = @TitleOfCourtesy,
                birthdate = @BirthDate,
                hiredate = @HireDate,
                address = @Address,
                city = @City,
                region = @Region,
                postalcode = @PostalCode,
                country = @Country,
                homephone = @HomePhone,
                extension = @Extension,
                photo = decode(@Photo, 'base64'),
                notes = @Notes,
                reportsto = @ReportsTo,
                photopath = @PhotoPath
            WHERE employeeid = @EmployeeID;
        ";

        await Connection.QueryAsync(
            query,
            new
            {
                EmployeeID = id,
                model.LastName,
                model.FirstName,
                model.Title,
                model.TitleOfCourtesy,
                model.BirthDate,
                model.HireDate,
                model.Address,
                model.City,
                model.Region,
                model.PostalCode,
                model.Country,
                model.HomePhone,
                model.Extension,
                model.Photo,
                model.Notes,
                model.ReportsTo,
                model.PhotoPath
            }
        );
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        const string query = @"
            DELETE FROM employees 
            WHERE employeeid = @EmployeeID
        ";

        await Connection.ExecuteAsync(
            query,
            new { EmployeeID = id }
        );
    }
}