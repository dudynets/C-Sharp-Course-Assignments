namespace Hospital.Tests;

public class DataFixture : IDisposable
{
    public List<Patient> Patients { get; private set; }
    public List<Service> Services { get; private set; }
    public List<Doctor> Doctors { get; private set; }
    public List<ServiceReport> ServiceReports { get; private set; }

    public DataFixture()
    {
        Patients = new List<Patient>
        {
            new Patient(1, "John", new DateTime(2021, 1, 1)),
            new Patient(2, "Jane", new DateTime(2020, 1, 1)),
            new Patient(3, "Jack", new DateTime(2019, 1, 1)),
            new Patient(4, "Jill", new DateTime(2018, 1, 1)),
            new Patient(5, "Jim", new DateTime(2017, 1, 1)),
        };

        Services = new List<Service>
        {
            new Service(1, "Service1", 100),
            new Service(2, "Service2", 200),
            new Service(3, "Service3", 300),
            new Service(4, "Service4", 400),
            new Service(5, "Service5", 500),
        };

        Doctors = new List<Doctor>
        {
            new Doctor(1, "Doctor1"),
            new Doctor(2, "Doctor2"),
            new Doctor(3, "Doctor3"),
            new Doctor(4, "Doctor4"),
            new Doctor(5, "Doctor5"),
        };

        ServiceReports = new List<ServiceReport>
        {
            new ServiceReport(new DateTime(2021, 1, 1), 1, 1, 1, 1),
            new ServiceReport(new DateTime(2020, 1, 1), 2, 2, 2, 2),
            new ServiceReport(new DateTime(2019, 1, 1), 3, 3, 3, 3),
            new ServiceReport(new DateTime(2018, 1, 1), 4, 4, 4, 4),
            new ServiceReport(new DateTime(2017, 1, 1), 5, 5, 5, 5),
        };
    }

    public void Dispose()
    { }
}


public class HospitalTests : IClassFixture<DataFixture>
{
    private readonly DataFixture _data;

    public HospitalTests(DataFixture data)
    {
        _data = data;
    }

    [Fact]
    public void Task1_ReturnsCorrectDoctorRevenueList()
    {
        var result = Program.Task1(_data.Patients, _data.Services, _data.Doctors, _data.ServiceReports);

        Assert.Equal("Doctor5", result[0].Key.Surname);
        Assert.Equal("Doctor4", result[1].Key.Surname);
        Assert.Equal("Doctor3", result[2].Key.Surname);
        Assert.Equal("Doctor2", result[3].Key.Surname);
        Assert.Equal("Doctor1", result[4].Key.Surname);

        Assert.Equal(2500, result[0].Value);
        Assert.Equal(1600, result[1].Value);
        Assert.Equal(900, result[2].Value);
        Assert.Equal(400, result[3].Value);
        Assert.Equal(100, result[4].Value);
    }

    [Theory]
    [InlineData("John", "2021-01-01", "2021-12-31", 100)]
    [InlineData("Jane", "2020-01-01", "2020-12-31", 400)]
    [InlineData("Jack", "2019-01-01", "2019-12-31", 900)]
    public void Task2_ReturnsCorrectTotalCost(string patientName, string startDate, string endDate, decimal expected)
    {
        var result = Program.Task2(_data.Patients, _data.Services, _data.ServiceReports, patientName, DateTime.Parse(startDate), DateTime.Parse(endDate));
        Assert.Equal(expected, result);
    }
}