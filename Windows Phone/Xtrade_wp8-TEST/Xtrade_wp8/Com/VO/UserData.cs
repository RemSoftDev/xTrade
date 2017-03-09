namespace Xtrade_wp8.Com.VO
{
    class UserData
    {
        public string Login { get; set; }

        public string Password { get; set; }

        // данные для синхронизации языка
        public string DataJson { get; set; }

        public UserData(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}