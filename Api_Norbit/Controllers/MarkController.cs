using Api_Norbit.DataBase;
using Api_Norbit.Logick;
using Api_Norbit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.PostgresTypes;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Api_Norbit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkController : ControllerBase
    {

        /// <summary>
        /// Формирует подробное сообщение с ошибками.
        /// </summary>
        /// <param name="id">Проверяемый id пользователя.</param>
        /// <param name="subject">Проверяемый id предмета.</param>
        /// <param name="mark">Проверяемая оценка.</param>
        /// <returns>Сообщение ошибок.</returns>
        private string MessageError(string id, string subject, string mark)
        {
            string message = "";
            try
            {
                int checkID = int.Parse(id);
            }
            catch
            {
                message += "Введёный вами id имел не верный формат. Верным форматом является число(integer)";
            }

            try
            {
                int checkSubject = int.Parse(subject);
            }
            catch
            {
                message += "Введёный вами id предмета имел не верный формат. Верным форматом является число(integer)";
            }

            try
            {
                int checkMark = int.Parse(mark);
            }
            catch
            {
                message += "Введёная вами оценка имела не верный формат. Верным форматом является число(integer)";
            }

            return message;
        }

        /// <summary>
        /// Формирует подробное сообщение с ошибками.
        /// </summary>
        /// <param name="integer">Проверяемое число.</param>
        /// <returns>Сообщение ошибок.</returns>
        private string MessageError(string integer)
        {
            string message = "";
            try
            {
                int checkID = int.Parse(integer);
            }
            catch
            {
                message += "Введёный вами id имел не верный формат. Верным форматом является число(integer)";
            }

            return message;
        }



        /// <summary>
        /// Считывает все оценки указанного пользователя.
        /// </summary>
        /// <param name="userId">id пользователя, у которого надо найти оценки.</param>
        /// <returns>Список оценок.</returns>
        private List<User_markModel> ReadMarkFromDB(int userId)
        {
            List<User_markModel> marks = new List<User_markModel>();
            using (DB db = new DB())
            {
                var users = db.User_mark.Where(i => i.user_id == userId).ToList();

                foreach (var u in users)
                {
                    User_markModel mark = new User_markModel();
                    mark.subject_id = u.subject_id;
                    mark.user_id = u.user_id;
                    mark.mark = u.mark;
                    marks.Add(mark);
                }
            }
            return marks;
        }


        /// <summary>
        /// Получает списко предметов из БД.
        /// </summary>
        /// <returns>Список предметов</returns>
        private Dictionary<int, string> ReadSubjectFromDB()
        {
            Dictionary<int, string> subject = new Dictionary<int, string>();
            using (DB db = new DB())
            {
                var users = db.Subject.ToList();

                foreach (var u in users)
                {
                    subject.Add(u.id, u.subject);
                }
            }
            return subject;
        }


        /// <summary>
        /// Получает пользователя из БД.
        /// </summary>
        /// <param name="userId">id Пользоавтеля по которому нудно его определить.</param>
        /// <returns>ФИО пользователя.</returns>
        private string ReadUserFromDB(int userId)
        {
            string user = "";
            using (DB db = new DB())
            {
                user = db.User.Where(i => i.id == userId).FirstOrDefault().FIO;
            }
            return user;
        }


        /// <summary>
        /// Заменяет id предмета на его название.
        /// </summary>
        /// <param name="dataMark">Информация об оценках.</param>
        /// <param name="dataSubject">Информация о предметах.</param>
        /// <returns>Заменёный список предметов.</returns>
        private List<Mark> Replace(List<User_markModel> dataMark, Dictionary<int, string> dataSubject)
        {
            List<Mark> marks = new List<Mark>();
            for(int i = 0; i < dataMark.Count(); i++)
            {
                Mark mark = new Mark();
                mark.subject = dataSubject[dataMark[i].subject_id];
                mark.mark = dataMark[i].mark;
                marks.Add(mark);
            }
            return marks;
        }


        /// <summary>
        /// Получает информацию об оценках.
        /// </summary>
        /// <param name="userId">id ученика у которого требуется найти оценки.</param>
        /// <returns>Оценки по предметам.</returns>
        [HttpGet(Name = "GiveUserMarks")]
        public string GetMarks(string userId)
        {
            string problem = MessageError(userId);
            if (problem == "")
                return problem;


            List<User_markModel> dataMark = ReadMarkFromDB(int.Parse(userId));
            if (dataMark.Count() == 0)
                return "Нет ученика с таким id. Проверьте id и попробуйте снова";
            
            Dictionary<int, string> dataSubject = ReadSubjectFromDB();
            string dataUser = ReadUserFromDB(int.Parse(userId));
            List<Mark> listMarks = Replace(dataMark, dataSubject);

            string answer = $"Ученик: {dataUser}\n";
            foreach(var e in listMarks)
                answer += $"Предмет: {e.subject}, Оценка: {e.mark}\n";
            
            return answer;
        }


        /// <summary>
        /// Ищет максимальный id среди записей в таблице User_mark.
        /// </summary>
        /// <returns>Возвращает id для оценки</returns>
        private int findMaxIdMark()
        {
            int maxId = 0;
            try
            {
                using (DB db = new DB())
                {
                    maxId = db.User_mark.Max(u => u.id);
                }
            }
            catch
            {
            }
            return maxId + 1;
        }



        /// <summary>
        /// Добавляет указаному ученику оценку по предмету.
        /// </summary>
        /// <param name="userId">id пользователя.</param>
        /// <param name="idSubject">id предмета.</param>
        /// <param name="mark">Оценка.</param>
        /// <returns>Статус выполнения.</returns>
        [HttpPost(Name = "AddMarksUser")]
        public StatusCodeResult AddMarks(string userId, string idSubject, string mark)
        {
            string problem = MessageError(userId, idSubject, mark);
            int idMark = findMaxIdMark();

            if(problem == "")
                return StatusCode(400);

            try
            {
                using (DB db = new DB())
                {
                    User_markModel addMark = new User_markModel { id = idMark, user_id = int.Parse(userId), mark = int.Parse(mark), subject_id = int.Parse(idSubject)};
                    db.User_mark.Add(addMark);
                    db.SaveChanges();
                }
                return StatusCode(200);
            }
            catch
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Удаляет оценку по указанному id.
        /// </summary>
        /// <param name="idMarks">Принимает id оценки.</param>
        /// <returns>Статус выполнения.</returns>
        [HttpDelete(Name = "DeleteMarksUser")]
        public StatusCodeResult DeleteMarks(string idMarks)
        {
            string problem = MessageError(idMarks);
            if (problem == "")
                return StatusCode(400);

            using (DB db = new DB())
            {
                User_markModel? mark = db.User_mark.Where(d => d.id == int.Parse(idMarks)).FirstOrDefault();
                if (mark != null)
                {
                    db.User_mark.Remove(mark);
                    db.SaveChanges();
                    return StatusCode(200);
                }
            }
            return StatusCode(400);
        }

        /// <summary>
        /// Вспомогательная структура для вывода оценок пользователю.
        /// </summary>
        struct Mark
        {
            public string subject;
            public int mark;
        }
    }
}
