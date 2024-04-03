class Computer
{
    public string Brand { get; }
    public int ProcessorSpeed { get; set; }
    public int RAM { get; set; }
    public int Disk { get; set; }
    public int Price { get; set; }

    public Computer(string brand, int processorSpeed, int ram, int disk, int price)
    {
        Brand = brand;
        ProcessorSpeed = processorSpeed;
        RAM = ram;
        Disk = disk;
        Price = price;
    }

    public override string ToString()
    {
        return $@"Brand: {Brand}
        Processor speed: {ProcessorSpeed} MHz
        RAM: {RAM}GB
        Main disk: {Disk}GB
        Price: ${Price}";
    }
}

class Server : Computer
{
    public List<int> AdditionalDisks { get; }

    public Server(string brand, int processorSpeed, int ram, int disk, int price, List<int> additionalDisks) : base(brand, processorSpeed, ram, disk, price)
    {
        AdditionalDisks = additionalDisks;
    }

    public override string ToString()
    {
        return $@"Server:
        {base.ToString()}
        Additional disks: {string.Join("GB, ", AdditionalDisks)}GB";
    }
}

class WorkStation : Computer
{
    public int MonitorSize { get; }

    public WorkStation(string brand, int processorSpeed, int ram, int disk, int price, int monitorSize) : base(brand, processorSpeed, ram, disk, price)
    {
        MonitorSize = monitorSize;
    }

    public override string ToString()
    {
        return $@"WorkStation:
        {base.ToString()}
        Monitor size: {MonitorSize}'";
    }
}

class Program
{
    static void Main()
    {
        List<Computer> computers =
        [
            new Server("Dell", 2000, 8, 1000, 1000, [2000]),
            new Server("HP", 3000, 16, 2000, 2000, [4000, 4000, 4000]),
            new Server("Lenovo", 2500, 12, 1500, 1500, [3000]),
            new WorkStation("Dell", 2000, 8, 1000, 1000, 20),
            new WorkStation("HP", 3000, 16, 2000, 2000, 40),
            new WorkStation("Lenovo", 2500, 12, 1500, 1500, 30),
        ];

        PrintAllComputers(computers);
        PrintTotalPrice(computers);
        PrintTotalDiskSpace(computers);
        PrintMaxMonitorSize(computers);



        //  List<Computer> computersgroupedbybrand= computers.GroupBy(c => c.Brand).SelectMany(g => g).ToList();


        var groupedComputers = computers.GroupBy(c => c.Brand);

        foreach (var group in groupedComputers)
        {
            Console.WriteLine(group.Key);
            foreach (var computer in group)
            {
                Console.WriteLine(computer);
            }
        }



    }


    static void PrintAllComputers(List<Computer> computers)
    {
        Console.WriteLine("All computers:");

        foreach (var computer in computers)
        {
            Console.WriteLine($"    {computer}\n");
        }
    }

    static void PrintTotalPrice(List<Computer> computers)
    {
        Console.WriteLine("Total price for each brand:");

        var brands = computers.Select(c => c.Brand).Distinct();
        foreach (var brand in brands)
        {
            var totalPrice = computers.Where(c => c.Brand == brand).Sum(c => c.Price);
            Console.WriteLine($"    {brand}: ${totalPrice}");
        }

        Console.WriteLine();
    }

    static void PrintTotalDiskSpace(List<Computer> computers)
    {
        Console.WriteLine("Total disk space for servers:");

        var servers = computers.OfType<Server>();
        foreach (var server in servers)
        {
            var totalDiskSpace = server.Disk + server.AdditionalDisks.Sum();
            Console.WriteLine($"    {server.Brand}: {totalDiskSpace}GB");
        }

        Console.WriteLine();
    }

    static void PrintMaxMonitorSize(List<Computer> computers)
    {
        Console.WriteLine("Computers with max monitor size:");

        var workStations = computers.OfType<WorkStation>();
        var maxMonitorSize = workStations.Max(ws => ws.MonitorSize);
        var maxMonitorSizeComputers = workStations.Where(ws => ws.MonitorSize == maxMonitorSize);
        foreach (var computer in maxMonitorSizeComputers)
        {
            Console.WriteLine($"    {computer}\n");
        }
    }
}