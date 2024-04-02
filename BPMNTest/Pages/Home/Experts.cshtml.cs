using BPMNTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMNTest.Pages.Home
{
    public class ExpertsModel : PageModel
    {
        public List<Expert> Experts { get; set; }

        public void OnGet()
        {
            // ������������� ��������� ������
            Experts = new List<Expert>
            {
                new() { Id = Guid.NewGuid(), Name = "���� ������", Email = "ivanov@example.com", Phone = "123-456-7890", Department = "����� ����������" },
                new() { Id = Guid.NewGuid(), Name = "����� �������", Email = "petrova@example.com", Phone = "987-654-3210", Department = "����� ����������" }
                // �������� ������ ��������� �� ��������
            };
        }
    }
}
