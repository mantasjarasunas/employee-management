# Employee management API.

### dotnet 8.0 version

## Includes CRUD for employees and controller/repository tests.

# Additional SQL Queries

### 1. Get all Products within Category "X" from Suppliers in NY

```sql
SELECT
    p.ProductID,
    p.ProductName,
    p.QuantityPerUnit,
    p.UnitPrice,
    s.CompanyName,
    s.City,
    c.CategoryName
FROM Products p
         INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
         INNER JOIN Categories c ON p.CategoryID = c.CategoryID
WHERE c.CategoryName = 'X' AND s.City = 'New York';
    
```

### 2. Create a query that shows the average order price per customer. Sort by average price descending.

```sql 
SELECT
    c.CompanyName AS Customer,
    c.City,
    AVG((od.UnitPrice::NUMERIC) * od.Quantity * (1 - od.Discount)) AS AverageOrderPrice
FROM Customers c
         INNER JOIN Orders o ON c.CustomerID = o.CustomerID
         INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
GROUP BY c.CompanyName, c.City
ORDER BY AverageOrderPrice DESC;
```

### 3. Display employees with no sale in the last 3 months to customers who are from "USA"

```sql
SELECT
    e.EmployeeID,
    e.LastName,
    e.FirstName,
    o.orderdate,
    c.country
FROM Employees e
         LEFT JOIN Orders o ON e.employeeid = o.employeeid
         LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
WHERE (o.OrderDate IS NULL OR o.OrderDate < CURRENT_DATE - 90 AND c.Country = 'USA');
```
