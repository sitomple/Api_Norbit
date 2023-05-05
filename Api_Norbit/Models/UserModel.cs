namespace Api_Norbit.Models
{
    /// <summary>
    /// Модель таблицы User.
    /// </summary>
    public class UserModel
    {
        public int id { get; set; }
        public string FIO { get; set; }
        public string role { get; set; }
        public string grade { get; set; }
        public string login { get; set; }
        public string password { get; set; }
    }
}
