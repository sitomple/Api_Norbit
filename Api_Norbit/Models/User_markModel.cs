using Microsoft.EntityFrameworkCore;

namespace Api_Norbit.Models
{
    /// <summary>
    /// Модель таблицы User_mark.
    /// </summary>
    public class User_markModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int subject_id { get; set; }
        public int mark { get; set; }
    }
}
