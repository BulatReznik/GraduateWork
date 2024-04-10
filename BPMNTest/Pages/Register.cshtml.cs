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
            // Отправляем данные на сервер для регистрации
            var response = await _apiService.PostAsync<RegisterFormModel, RegisterResponseModel>("v1/user/register", registerFormModel);

            if (response.IsSuccess)
            {
                if (!string.IsNullOrEmpty(response.Data?.AccessToken))
                {
                    // Если токен доступа получен, то аутентификация успешна
                    // В случае успешной аутентификации перенаправляем пользователя на другую страницу
                    return RedirectToPage("/Index");
                }
                else
                {
                    // Если токен доступа не получен, можно считать, что пользователь не найден
                    TempData["ErrorMessage"] = "Не удалось зарегестрировать пользователя";
                    return Page();
                }
            }
            else
            {
                // В случае ошибки отображаем сообщение об ошибке
                TempData["ErrorMessage"] = response.ErrorMessage;
                return Page();
            }
        }
    }
}
