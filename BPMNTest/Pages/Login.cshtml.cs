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
            // Этот метод вызывается при получении GET запроса к странице входа
        }

        /// <summary>
        /// Отправляем пост запрос на логин
        /// </summary>
        public async Task<IActionResult> OnPostAsync(LoginFormModel loginRequest)
        {
            // Отправляем данные на сервер для аутентификации
            var response = await _apiService.PostAsync<LoginFormModel, LoginResponseModel>("v1/user/login", loginRequest);

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
                    TempData["ErrorMessage"] = "Пользователь не найден";
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
