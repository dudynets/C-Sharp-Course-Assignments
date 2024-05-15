namespace Hospital;

using System.Linq;

public class Patient
{
    public int Id { get; private set; }
    public string Surname { get; set; }
    public DateTime RegistrationDate { get; set; }

    public Patient(int id, string surname, DateTime registrationDate)
    {
        Id = id;
        Surname = surname;
        RegistrationDate = registrationDate;
    }

    public bool IsOlderThanThreeYears(DateTime baseDate)
    {
        return baseDate.Year - RegistrationDate.Year > 3;
    }
}

public class Service
{
    public int Id { get; private set; }
    public string Title { get; set; }
    public decimal Price { get; set; }

    public Service(int id, string title, decimal price)
    {
        Id = id;
        Title = title;
        Price = price;
    }
}

public class Doctor
{
    public int Id { get; private set; }
    public string Surname { get; set; }

    public Doctor(int id, string surname)
    {
        Id = id;
        Surname = surname;
    }
}

public class ServiceReport
{
    public DateTime Date { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int ServiceId { get; set; }
    public int Quantity { get; set; }

    public ServiceReport(DateTime date, int patientId, int doctorId, int serviceId, int quantity)
    {
        Date = date;
        PatientId = patientId;
        DoctorId = doctorId;
        ServiceId = serviceId;
        Quantity = quantity;
    }
}

public class Program
{
    static void Main()
    {
        var patients = new List<Patient>
        {
            new Patient(1, "John", new DateTime(2021, 1, 1)),
            new Patient(2, "Jane", new DateTime(2020, 1, 1)),
            new Patient(3, "Jack", new DateTime(2019, 1, 1)),
            new Patient(4, "Jill", new DateTime(2018, 1, 1)),
            new Patient(5, "Jim", new DateTime(2017, 1, 1)),
        };

        var services = new List<Service>
        {
            new Service(1, "Service1", 100),
            new Service(2, "Service2", 200),
            new Service(3, "Service3", 300),
            new Service(4, "Service4", 400),
            new Service(5, "Service5", 500),
        };

        var doctors = new List<Doctor>
        {
            new Doctor(1, "Doctor1"),
            new Doctor(2, "Doctor2"),
            new Doctor(3, "Doctor3"),
            new Doctor(4, "Doctor4"),
            new Doctor(5, "Doctor5"),
        };

        var serviceReports = new List<ServiceReport>
        {
            new ServiceReport(new DateTime(2021, 1, 1), 1, 1, 1, 1),
            new ServiceReport(new DateTime(2020, 1, 1), 2, 2, 2, 2),
            new ServiceReport(new DateTime(2019, 1, 1), 3, 3, 3, 3),
            new ServiceReport(new DateTime(2018, 1, 1), 4, 4, 4, 4),
            new ServiceReport(new DateTime(2017, 1, 1), 5, 5, 5, 5),
        };


        var task1Result = Task1(patients, services, doctors, serviceReports);

        Console.WriteLine("\nTask 1:");
        foreach (var item in task1Result)
        {
            Console.WriteLine($"- {item.Key.Surname} has made {item.Value} UAH.");
        }

        var task2Result = Task2(patients, services, serviceReports, "John", new DateTime(2021, 1, 1), new DateTime(2021, 12, 31));

        Console.WriteLine("\nTask 2:");
        Console.WriteLine($"- John has spent {task2Result} UAH.");
    }

    public static List<KeyValuePair<Doctor, decimal>> Task1(List<Patient> patients, List<Service> services, List<Doctor> doctors, List<ServiceReport> serviceReports)
    {
        return (from serviceReport in serviceReports
                join patient in patients on serviceReport.PatientId equals patient.Id
                join service in services on serviceReport.ServiceId equals service.Id
                join doctor in doctors on serviceReport.DoctorId equals doctor.Id
                select new
                {
                    Doctor = doctor,
                    Revenue = patient.IsOlderThanThreeYears(serviceReport.Date) ? service.Price * serviceReport.Quantity * 0.9m : service.Price * serviceReport.Quantity
                })
                .GroupBy(x => x.Doctor)
                .Select(x => new KeyValuePair<Doctor, decimal>(x.Key, x.Sum(y => y.Revenue)))
                .OrderByDescending(x => x.Value)
                .ToList();
    }

    public static decimal Task2(List<Patient> patients, List<Service> services, List<ServiceReport> serviceReports, string surname, DateTime startDate, DateTime endDate)
    {
        return (from serviceReport in serviceReports
                join patient in patients on serviceReport.PatientId equals patient.Id
                join service in services on serviceReport.ServiceId equals service.Id
                where patient.Surname == surname && serviceReport.Date >= startDate && serviceReport.Date <= endDate
                select patient.IsOlderThanThreeYears(serviceReport.Date) ? service.Price * serviceReport.Quantity * 0.9m : service.Price * serviceReport.Quantity)
                .Sum();
    }
}