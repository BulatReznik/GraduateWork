using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BPMN.Services;
using BPMN.Models.Login;

namespace BPMN.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApiService _apiService;
        public LoginFormModel LoginRequest { get; set; }

        public LoginModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public void OnGet()
        {
            // ���� ����� ���������� ��� ��������� GET ������� � �������� �����
        }

        /// <summary>
        /// ���������� ���� ������ �� �����
        /// </summary>
        public async Task<IActionResult> OnPostAsync(LoginFormModel loginRequest)
        {
            // ���������� ������ �� ������ ��� ��������������
            var response = await _apiService.PostAsync<LoginFormModel, LoginResponseModel>("v1/user/login", loginRequest);

            if (response.IsSuccess)
            {
                if (!string.IsNullOrEmpty(response.Data?.AccessToken))
                {
                    // ���� ����� ������� �������, �� �������������� �������
                    // � ������ �������� �������������� �������������� ������������ �� ������ ��������
                    return RedirectToPage("/Index");
                }
                else
                {
                    // ���� ����� ������� �� �������, ����� �������, ��� ������������ �� ������
                    TempData["ErrorMessage"] = "������������ �� ������";
                    return Page();
                }
            }
            else
            {
                // � ������ ������ ���������� ��������� �� ������
                TempData["ErrorMessage"] = response.ErrorMessage;
                return Page();
            }
        }
    }
}
