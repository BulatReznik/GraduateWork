using BPMNTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMNTest.Pages.Home
{
    public class ProjectsModel : PageModel
    {
        public List<Project> Projects { get; set; }

        public void OnGet()
        {
            Projects = new List<Project>
            {
                new() { Id = Guid.NewGuid(), Name = "���������� CRM-�������", Description = "�������� �������������� CRM-������� ��� ���������� ���������� �����", StartDate = new DateTime(2023, 1, 15), EndDate = new DateTime(2023, 12, 31), Status = "�������" },
                new() { Id = Guid.NewGuid(), Name = "������������� ���������", Description = "���������� ������� ��� ����������� ������������� ��������� � ������� �������� ����", StartDate = new DateTime(2023, 2, 1), EndDate = new DateTime(2023, 6, 30), Status = "��������" },
                new() { Id = Guid.NewGuid(), Name = "���������� �������������� �������", Description = "������ �� ���������� ������� � ����������� �������������� ������� ��������", StartDate = new DateTime(2023, 3, 20), EndDate = new DateTime(2023, 11, 20), Status = "� ����������" },
                new() { Id = Guid.NewGuid(), Name = "���������� ������� �������������� �����", Description = "������ �� ���������� ����� ������� �������������� ����� � ������������� IT-��������� ��������", StartDate = new DateTime(2023, 4, 10), EndDate = new DateTime(2023, 9, 30), Status = "� ��������" },
                new() { Id = Guid.NewGuid(), Name = "���������� ���������� ����������", Description = "�������� ���������� ���������� ��� �������������� ����� ������-������������", StartDate = new DateTime(2023, 5, 5), EndDate = new DateTime(2023, 10, 5), Status = "�����������" }
            };
        }

    }
}
