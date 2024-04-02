namespace BPMNTest.ViewModels
{
    public class Expert
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; } // Заметка: Хранить пароли в открытом виде небезопасно
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
    }
}
