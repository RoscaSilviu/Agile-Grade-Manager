using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;
using CatalogueServer.Models;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class BackupController : ControllerBase
{
    private readonly ClassRepository _classRepository;
    private readonly UserRepository _userRepository;
    private readonly AssignmentRepository _assignmentRepository;
    private readonly GradeRepository _gradeRepository;
    private readonly ClassStudentRepository _classStudentRepository;

    public BackupController(
        ClassRepository classRepository,
        UserRepository userRepository,
        AssignmentRepository assignmentRepository,
        GradeRepository gradeRepository,
        ClassStudentRepository classStudentRepository)
    {
        _classRepository = classRepository;
        _userRepository = userRepository;
        _assignmentRepository = assignmentRepository;
        _gradeRepository = gradeRepository;
        _classStudentRepository = classStudentRepository;
    }

    [HttpGet("text")]
    public ActionResult GetFormattedBackup()
    {
        var users = _userRepository.GetAll();
        var classes = _classRepository.GetAll();
        var classStudents = _classStudentRepository.GetAll();
        var assignments = _assignmentRepository.GetAll();
        var grades = _gradeRepository.GetAll();

        var teachers = users.Where(u => u.Role == "teacher").ToList();
        var students = users.Where(u => u.Role == "student").ToList();

        var sb = new StringBuilder();
        sb.AppendLine("📦 FULL DATABASE BACKUP");
        sb.AppendLine("========================\n");

        foreach (var teacher in teachers)
        {
            sb.AppendLine($"👨‍🏫 Teacher: {teacher.Name} ({teacher.Email})");

            var teacherClasses = classes.Where(c => c.TeacherId == teacher.Id);

            foreach (var cls in teacherClasses)
            {
                sb.AppendLine($"\n  📚 Class: {cls.Name}");

                var classAssignments = assignments.Where(a => a.ClassId == cls.Id);
                var enrolledStudentIds = classStudents
                    .Where(cs => cs.ClassId == cls.Id)
                    .Select(cs => cs.StudentId)
                    .ToList();

                var enrolledStudents = students.Where(s => enrolledStudentIds.Contains(s.Id)).ToList();

                foreach (var assignment in classAssignments)
                {
                    sb.AppendLine($"    📝 Assignment: {assignment.Name} | Due: {assignment.DueDate:d} | Max Points: {assignment.MaxGrade}");

                    foreach (var student in enrolledStudents)
                    {
                        var grade = grades.FirstOrDefault(g => g.StudentId == student.Id && g.AssignmentId == assignment.Id);
                        string points = grade?.Value.ToString() ?? "N/A";
                        sb.AppendLine($"        👤 {student.Name} {student.Surname} - 🎓 {points}");
                    }
                }
            }

            sb.AppendLine(); // spacing between teachers
        }
        var chestie = sb.ToString();
        return Ok(chestie);
    }



}
