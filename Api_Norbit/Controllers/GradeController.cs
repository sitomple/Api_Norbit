using Api_Norbit.DataBase;
using Api_Norbit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Norbit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        /// <summary>
        /// Получает из БД все классы.
        /// </summary>
        /// <returns>Список классов.</returns>
        [HttpGet(Name = "GiveClass")]
        public List<ClassModel> GetAllGrade()
        {
            using (DB db = new DB())
            {
                var grade = db.Grade.ToList();
                return grade;
            }
        }

        /// <summary>
        /// Проверяет существует ли уже класс, который хотят добавить.
        /// </summary>
        /// <param name="nameSubject">Название класса</param>
        /// <returns>Булевое значение: True - пройдена проверка на повторения, false - проверка на повторения не пройдена.</returns>
        private bool checkClassName(string nameClass)
        {
            using (DB db = new DB())
            {
                var grade = db.Grade.Where(n => n.grade == nameClass).ToList();
                if (grade.Count == 0)
                    return true;
            }
            return false;
        }



        /// <summary>
        /// Добавляет новый класс.
        /// </summary>
        /// <param name="nameSubject">Название класса</param>
        /// <returns>Отчётность о ходе выполнения.</returns>
        [HttpPost(Name = "AddClass")]
        public ActionResult AddNewClass(string nameClass)
        {
            if (checkClassName(nameClass))
            {
                try
                {
                    using (DB db = new DB())
                    {
                        ClassModel newClass = new ClassModel();
                        newClass.grade = nameClass;

                        db.Grade.Add(newClass);
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
        /// Удаляет предмет по указанному Названию.
        /// </summary>
        /// <param name="idDel">id удаляемого предмета.</param>
        /// <returns>Отчётность о ходе выполнения.</returns>
        [HttpGet(Name = "DeleteSubject")]
        public ActionResult DeleteSubject(string nameDel)
        {
            using (DB db = new DB())
            {
                ClassModel? deleteClass = db.Grade.Where(i => i.grade == nameDel.ToLower()).FirstOrDefault();

                if (deleteClass != null)
                {
                    db.Grade.Remove(deleteClass);
                    return Ok("Предмет успешно удалён!");
                }
                else
                    return BadRequest("Предмета с таким названием не было найдено!");
            }
        }
    }
}
