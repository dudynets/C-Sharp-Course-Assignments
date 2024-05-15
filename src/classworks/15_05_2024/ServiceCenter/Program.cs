
using System.Xml.Linq;

namespace ServiceCenter;

public class ProductCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int WarrantyYears { get; set; }

    public ProductCategory(int id, string name, int warrantyYears)
    {
        Id = id;
        Name = name;
        WarrantyYears = warrantyYears;
    }
}

public class Operation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }

    public Operation(int id, string name, double price)
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public static bool WarrantyActive(int warrantyYears, DateOnly releaseDate)
    {
        return DateTime.Now.Year - releaseDate.Year <= warrantyYears;
    }
}

public class ServiceReport
{
    public int ProductCategoryId { get; set; }
    public int OperationId { get; set; }
    public DateOnly ProductReleaseDate { get; set; }

    public ServiceReport(int productCategoryId, int operationId, string releaseDateIso = "")
    {
        ProductCategoryId = productCategoryId;
        OperationId = operationId;
        ProductReleaseDate = releaseDateIso == "" ? DateOnly.FromDateTime(DateTime.Now) : DateOnly.FromDateTime(DateTime.Parse(releaseDateIso));
    }
}

public class Program
{
    public static void Main()
    {
        var productCategories = GetProductCategories();
        var operations = GetOperations();
        var serviceReports = GetServiceReports();

        var task1Result = Task1(productCategories, operations, serviceReports, true);
        var task2Result = Task2(productCategories, operations, serviceReports, true);
        var task3Result = Task3(productCategories, operations, serviceReports, "Home Appliances", true);
    }

    public static Dictionary<string, Dictionary<string, int>> Task1(
        IEnumerable<ProductCategory> productCategories,
        IEnumerable<Operation> operations,
        IEnumerable<ServiceReport> serviceReports,
        bool saveToFile = false
    )
    {
        var query = from productCategory in productCategories
                    join serviceReport in serviceReports on productCategory.Id equals serviceReport.ProductCategoryId
                    join operation in operations on serviceReport.OperationId equals operation.Id
                    group operation by new { ProductCategoryName = productCategory.Name, OperationName = operation.Name } into g
                    orderby g.Key.ProductCategoryName ascending, g.Count() descending, g.Key.OperationName ascending
                    select new
                    {
                        ProductCategoryName = g.Key.ProductCategoryName,
                        OperationName = g.Key.OperationName,
                        Count = g.Count()
                    };

        var result = query.GroupBy(x => x.ProductCategoryName)
                          .ToDictionary(
                              g => g.Key,
                              g => g.ToDictionary(x => x.OperationName, x => x.Count));

        if (!saveToFile)
        {
            return result;
        }

        var csv = "ProductCategoryName,OperationName,Count\n";
        foreach (var productCategory in result)
        {
            foreach (var operation in productCategory.Value)
            {
                csv += $"{productCategory.Key},{operation.Key},{operation.Value}\n";
            }
        }

        EnsureOutputDirectoryExists();
        File.WriteAllText("output/Task1.csv", csv);

        return result;
    }

    public static Dictionary<string, Dictionary<string, double>> Task2(
        IEnumerable<ProductCategory> productCategories,
        IEnumerable<Operation> operations,
        IEnumerable<ServiceReport> serviceReports,
        bool saveToFile = false
    )
    {
        var query = from productCategory in productCategories
                    join serviceReport in serviceReports on productCategory.Id equals serviceReport.ProductCategoryId
                    join operation in operations on serviceReport.OperationId equals operation.Id
                    group operation by new { ProductCategoryName = productCategory.Name, OperationName = operation.Name } into g
                    orderby g.Key.ProductCategoryName ascending, g.Sum(x => x.Price) descending, g.Key.OperationName ascending
                    select new
                    {
                        ProductCategoryName = g.Key.ProductCategoryName,
                        OperationName = g.Key.OperationName,
                        Sum = g.Sum(x => x.Price)
                    };

        var result = query.GroupBy(x => x.ProductCategoryName)
                          .ToDictionary(
                              g => g.Key,
                              g => g.ToDictionary(x => x.OperationName, x => x.Sum));

        if (!saveToFile)
        {
            return result;
        }

        var xml = new XElement("RevenueReport");
        foreach (var productCategory in result)
        {
            var productCategoryElement = new XElement("ProductCategory", new XAttribute("Name", productCategory.Key));
            foreach (var operation in productCategory.Value)
            {
                var operationElement = new XElement("Operation", new XAttribute("Name", operation.Key), new XAttribute("Sum", operation.Value));
                productCategoryElement.Add(operationElement);
            }
            xml.Add(productCategoryElement);
        }

        EnsureOutputDirectoryExists();
        xml.Save("output/Task2.xml");

        return result;
    }

    public static Dictionary<string, int> Task3(
        IEnumerable<ProductCategory> productCategories,
        IEnumerable<Operation> operations,
        IEnumerable<ServiceReport> serviceReports,
        string categoryName,
        bool saveToFile = false
    )
    {
        var query = from productCategory in productCategories
                    join serviceReport in serviceReports on productCategory.Id equals serviceReport.ProductCategoryId
                    join operation in operations on serviceReport.OperationId equals operation.Id
                    where Operation.WarrantyActive(productCategory.WarrantyYears, serviceReport.ProductReleaseDate)
                    group operation by operation.Name into g
                    orderby g.Count() descending, g.Key ascending
                    select new
                    {
                        OperationName = g.Key,
                        Count = g.Count()
                    };

        var result = query.ToDictionary(x => x.OperationName, x => x.Count);

        if (!saveToFile)
        {
            return result;
        }

        var xml = new XElement("WarrantyReport");
        foreach (var operation in result)
        {
            var operationElement = new XElement("Operation", new XAttribute("Name", operation.Key), new XAttribute("Count", operation.Value));
            xml.Add(operationElement);
        }

        EnsureOutputDirectoryExists();
        xml.Save("output/Task3.xml");

        return result;
    }

    public static IEnumerable<ProductCategory> GetProductCategories()
    {
        XDocument xdoc = XDocument.Load("input/ProductCategories.xml");

        return xdoc.Element("ProductCategories")
            .Elements("ProductCategory")
            .Where(productCategoryElement =>
                productCategoryElement.Element("Name") != null &&
                productCategoryElement.Element("WarrantyYears") != null)
            .Select(productCategoryElement => new ProductCategory(
                int.Parse(productCategoryElement.Attribute("Id")?.Value ?? "0"),
                productCategoryElement.Element("Name")?.Value ?? "",
                int.Parse(productCategoryElement.Element("WarrantyYears")?.Value ?? "0")
            ));
    }

    public static IEnumerable<Operation> GetOperations()
    {
        XDocument xdoc = XDocument.Load("input/Operations.xml");

        return xdoc.Element("Operations")
            .Elements("Operation")
            .Where(operationElement =>
                operationElement.Element("Price") != null)
            .Select(operationElement => new Operation(
                int.Parse(operationElement.Attribute("Id")?.Value ?? "0"),
                operationElement.Element("Name")?.Value ?? "",
                double.Parse(operationElement.Element("Price")?.Value ?? "0")
            ));
    }

    public static IEnumerable<ServiceReport> GetServiceReports()
    {
        XDocument xdoc = XDocument.Load("input/ServiceReports.xml");

        return xdoc.Element("ServiceReports")
            .Elements("ServiceReport")
            .Where(serviceReportElement =>
                serviceReportElement.Element("ProductCategoryId") != null &&
                serviceReportElement.Element("OperationId") != null &&
                serviceReportElement.Element("ProductReleaseDate") != null)
            .Select(serviceReportElement => new ServiceReport(
                int.Parse(serviceReportElement.Element("ProductCategoryId").Value ?? "0"),
                int.Parse(serviceReportElement.Element("OperationId").Value ?? "0"),
                serviceReportElement.Element("ProductReleaseDate").Value ?? "")
            );
    }

    public static void EnsureOutputDirectoryExists()
    {
        if (!Directory.Exists("output"))
        {
            Directory.CreateDirectory("output");
        }
    }
}