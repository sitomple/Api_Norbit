using Api_Norbit.DataBase;
using Api_Norbit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Norbit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {

        /// <summary>
        /// Получает из БД все учебные предметы.
        /// </summary>
        /// <returns>Список учебных предметов.</returns>
        [HttpGet(Name = "GiveSubject")]
        public List<SubjectModel> GetAllSubject() 
        {
            using (DB db = new DB())
            {
                var subject = db.Subject.ToList();
                return subject;
            }
        }

        /// <summary>
        /// Проверяет существует ли уже предмет, который хотят добавить.
        /// </summary>
        /// <param name="nameSubject">Название предмета</param>
        /// <returns>Булевое значение: True - пройдена проверка на повторения, false - проверка на повторения не пройдена.</returns>
        private bool checkSubjectName(string nameSubject)
        {
            using (DB db = new DB())
            {
                var subject = db.Subject.Where(n => n.subject == nameSubject).ToList();
                if (subject.Count == 0)
                    return true;
            }
            return false;
        }



        /// <summary>
        /// Добавляет новый учебный предмет.
        /// </summary>
        /// <param name="nameSubject">Название предмета</param>
        /// <returns>Отчётность о ходе выполнения.</returns>
        [HttpPost(Name = "AddSubject")]
        public ActionResult AddNewSubject(string nameSubject)
        {
            if (checkSubjectName(nameSubject))
            {
                int id = 1;
                using (DB db = new DB())
                {
                    id = db.Subject.Max(u => u.id)+1;
                }
                try
                {
                    using (DB db = new DB())
                    {
                        SubjectModel sb = new SubjectModel();
                        sb.id = id;
                        sb.subject = nameSubject;

                        db.Subject.Add(sb);
                    }
                    return Ok("Предмет успешно добавлен!");
                }
                catch
                {
                    return BadRequest("Произошла ошибка!");
                }
            }
            return BadRequest("Такой предмет уже есть!");
        }

        /// <summary>
        /// Проверяет являлось ли передаваемое id числом
        /// </summary>
        /// <param name="idDel">проверяемая переменная на число.</param>
        /// <returns>Булевое значение: True - пройдено проверку на число, false - проверка на число не пройдена.</returns>
        private bool CheckIdSubject(string idDel)
        {
            try
            {
                int check = int.Parse(idDel);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Удаляет предмет по указанному id.
        /// </summary>
        /// <param name="idDel">id удаляемого предмета.</param>
        /// <returns>Отчётность о ходе выполнения.</returns>
        [HttpGet(Name = "DeleteSubject")]
        public ActionResult DeleteSubject(string idDel)
        {
            if (CheckIdSubject(idDel))
                using (DB db = new DB())
                {
                    SubjectModel? sb = db.Subject.Where(i => i.id == int.Parse(idDel)).FirstOrDefault();

                    if (sb != null)
                    {
                        db.Subject.Remove(sb);
                        return Ok("Предмет успешно удалён!");
                    }
                    else
                        return BadRequest("Предмета с таким id не было найдено!");
                }
            else
                return BadRequest("Вы ввели не верный формат");
        }
    }
}
