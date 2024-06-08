using System.Xml.Linq;

namespace University;

public class Task
{
    public int Id { get; private set; }
    public string Subject { get; set; }
    public DateOnly DueDate { get; set; }

    public Task(
        int id,
        string subject,
        DateOnly dueDate
    )
    {
        Id = id;
        Subject = subject;
        DueDate = dueDate;
    }
}

public class Student
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Group { get; set; }

    public Student(
        int id,
        string name,
        string surname,
        string group
    )
    {
        Id = id;
        Name = name;
        Surname = surname;
        Group = group;
    }
}

public class TaskResult
{
    public int TaskId { get; set; }
    public int StudentId { get; set; }
    public double Mark { get; set; }
    public DateOnly Date { get; set; }

    public TaskResult(
        int taskId,
        int studentId,
        double mark,
        DateOnly date
    )
    {
        TaskId = taskId;
        StudentId = studentId;
        Mark = mark;
        Date = date;
    }

    public double GetFinalMark(DateOnly dueDate) => Date > dueDate ? Mark / 2 : Mark;
}

public static class UniversityApp
{
    public static void Task1(
        IEnumerable<Task> tasks,
        IEnumerable<Student> students,
        IEnumerable<TaskResult> taskResults,
        string outputFilePath = "output/Task1.xml"
    )
    {
        // xml-файл, xml-файл, де результати систематизовані для кожної
        // групи за схемою прізвище з ініціалом імені студента, перелік
        // його результатів за порядком номерів завдання з вказанням на-
        // зви теми і зарахованих балів>. Результати впорядкувати за на-
        // звою групи і прізвищем у лексико-графічному порядку (без по-
        // вторень);

        var query = from student in students
                    join taskResult in taskResults on student.Id equals taskResult.StudentId
                    join task in tasks on taskResult.TaskId equals task.Id
                    orderby student.Group, student.Surname
                    group new { TaskResult = taskResult, Task = task } by student into studentGroup
                    select studentGroup;

        var grouped = query.GroupBy(
            x => x.Key.Group,
            x => x
        );

        var xdoc = new XDocument(
            new XElement("University",
                grouped.Select(group =>
                    new XElement("Group",
                        new XAttribute("Name", group.Key),
                        group.Select(studentGroup =>
                            new XElement("Student",
                                new XAttribute("Name", $"{studentGroup.Key.Surname} {studentGroup.Key.Name[0]}."),
                                studentGroup.Select(result =>
                                    new XElement("TaskResult",
                                        new XAttribute("TaskId", result.Task.Id),
                                        new XAttribute("Subject", result.Task.Subject),
                                        new XAttribute("Mark", result.TaskResult.GetFinalMark(result.Task.DueDate))
                                    )
                                )
                            )
                        )
                    )
                )
            )
        );

        xdoc.Save(outputFilePath);
    }

    public static IEnumerable<Task> LoadTasks()
    {
        XDocument xdoc = XDocument.Load("input/Tasks.xml");

        return xdoc.Element("Tasks")
            .Elements("Task")
            .Select(taskElement => new Task(
                int.Parse(taskElement.Attribute("Id")?.Value ?? "0"),
                taskElement.Element("Subject")?.Value ?? "",
                DateOnly.FromDateTime(DateTime.Parse(taskElement.Element("DueDate")?.Value ?? ""))
            ));
    }

    public static IEnumerable<Student> LoadStudents()
    {
        XDocument xdoc = XDocument.Load("input/Students.xml");

        return xdoc.Element("Students")
            .Elements("Student")
            .Select(studentElement => new Student(
                int.Parse(studentElement.Attribute("Id")?.Value ?? "0"),
                studentElement.Element("Name")?.Value ?? "",
                studentElement.Element("Surname")?.Value ?? "",
                studentElement.Element("Group")?.Value ?? ""
            ));
    }

    public static IEnumerable<TaskResult> LoadTaskResults()
    {
        XDocument xdoc = XDocument.Load("input/TaskResults.xml");

        return xdoc.Element("TaskResults")
            .Elements("TaskResult")
            .Select(taskResultElement => new TaskResult(
                int.Parse(taskResultElement.Element("TaskId")?.Value ?? "0"),
                int.Parse(taskResultElement.Element("StudentId")?.Value ?? "0"),
                double.Parse(taskResultElement.Element("Mark")?.Value ?? "0"),
                DateOnly.FromDateTime(DateTime.Parse(taskResultElement.Element("Date")?.Value ?? ""))
            ));
    }
}

public static class Program
{
    public static void Main()
    {
        var tasks = UniversityApp.LoadTasks();
        var students = UniversityApp.LoadStudents();
        var taskResults = UniversityApp.LoadTaskResults();

        UniversityApp.Task1(tasks, students, taskResults, "output/Task1.xml");
    }


}