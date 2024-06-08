using University;

namespace University.Tests;

public class UniversityFixture : IDisposable
{
    public IEnumerable<Task> tasks { get; private set; }
    public IEnumerable<Student> students { get; private set; }
    public IEnumerable<TaskResult> taskResults { get; private set; }

    public UniversityFixture()
    {
        tasks = UniversityApp.LoadTasks();
        students = UniversityApp.LoadStudents();
        taskResults = UniversityApp.LoadTaskResults();
    }

    public void Dispose()
    {
        // Clean up the test data
    }
}

public class UniversityTests : IClassFixture<UniversityFixture>
{
    private readonly UniversityFixture _fixture;

    public UniversityTests(UniversityFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test1()
    {
        // Arrange
        var tasks = _fixture.tasks;
        var students = _fixture.students;
        var taskResults = _fixture.taskResults;

        // Act
        var result = UniversityApp.Task1(tasks, students, taskResults);

        // Assert
        Assert.Equal(0, result);
    }
}
