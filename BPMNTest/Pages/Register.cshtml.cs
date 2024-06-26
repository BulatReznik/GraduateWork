using BPMN.Models.Login;
using BPMN.Models.Register;
using BPMN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPMN.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ApiService _apiService;
        public RegisterFormModel RegisterFormModel { get; set; }

        public RegisterModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(RegisterFormModel registerFormModel)
        {
            // ���������� ������ �� ������ ��� �����������
            var response = await _apiService.PostAsync<RegisterFormModel, RegisterResponseModel>("v1/user/register", registerFormModel);

            if (response.IsSuccess)
            {
                if (!string.IsNullOrEmpty(response.Data?.AccessToken))
                {
                    // ���� token ������� �������, �� �������������� �������
                    // � ������ �������� �������������� �������������� ������������ �� ������ ��������
                    HttpContext.Session.SetString("AccessToken", response.Data.AccessToken);
                    HttpContext.Session.SetString("RefreshToken", response.Data.RefreshToken);

                    return RedirectToPage("/Index");
                }

                // ���� token ������� �� �������, ����� �������, ��� ������������ �� ������
                TempData["ErrorMessage"] = "�� ������� ���������������� ������������";
                return Page();
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
