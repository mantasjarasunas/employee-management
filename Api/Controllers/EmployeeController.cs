using Business.Services;
using Domain.Employee;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    /// <summary>
    /// Create employee
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns>Created employeeId</returns>
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateEmployeeRequestModel requestModel)
    {
        var result = await employeeService.CreateEmployeeAsync(requestModel);
        return Ok(result);
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    /// <returns>List of all employees</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeListItemModel>>> Get()
    {
        var result = await employeeService.GetAllEmployeesAsync();
        return Ok(result);
    }
    
    /// <summary>
    /// Update 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateEmployeeRequestModel requestModel)
    {
        await employeeService.UpdateEmployeeByIdAsync(id, requestModel);
        return Ok();
    }

    /// <summary>
    /// Delete employee
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await employeeService.DeleteEmployeeByIdAsync(id);
        return Ok();
    }
}