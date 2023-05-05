using Api_Norbit.DataBase;
using Api_Norbit.Logick;
using Api_Norbit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Api_Norbit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        /// <summary>
        /// Получает всех пользователей из таблицы User.
        /// </summary>
        /// <returns>Возвращает всех пользователей.</returns>
        [HttpGet(Name = "GetAllUser")]
        public string GetAllUser()
        {
            string infoUsers = "";
            using (DB db = new DB())
            {
                var users = db.User.OrderByDescending(x => x.grade).ToList();

                foreach (UserModel u in users)
                {
                    infoUsers += $"Фамилия имя отчество:{u.FIO}; Является: {u.role}; Класс:{u.grade}\n";
                }
            }
            return infoUsers;
        }


        /// <summary>
        /// Поиск уникального id для нового пользователя.
        /// </summary>
        /// <returns>Уникальный id для пользователя.</returns>
        private int findMaxIdUser()
        {
            int maxId = 0;
            using (DB db = new DB())
            {
                maxId = db.User.Max(u => u.id);
            }
            return maxId + 1;
        }


        /// <summary>
        /// Массив ролей.
        /// </summary>
        private string[] CheckRole = new string[]
        {
            "student",
            "teacher",
            "director"
        };


        /// <summary>
        /// Массив классов.
        /// </summary>
        private string[] CheckGrade = new string[]
        {
            "5а",
            "5б",
            "6а",
            "6б",
            "7а",
            "7б",
            "8а",
            "8б",
            "9",
            "10",
            "11"
        };



        /// <summary>
        /// Проверяет логин на уникольность.
        /// </summary>
        /// <param name="login">Проверяемый логин.</param>
        /// <returns>Булевое значение проврки(true - повторения найдены, false - повторения не найдены).</returns>
        private bool CheckLogin(string login)
        {
            using (DB db = new DB())
            {
                var users = db.User.OrderByDescending(x => x.grade).ToList();

                foreach (UserModel u in users)
                {
                    if (u.login == login)
                        return true;
                }
            }
            return false;
        }



        /// <summary>
        /// Формирует подробное сообщение с ошибками.
        /// </summary>
        /// <param name="role">Получаемая роль на проверку.</param>
        /// <param name="grade">Получаемый класс на проверку.</param>
        /// <param name="login">Получаемый логин на проверку.</param>
        /// <returns>Сформированное сообщение с ошибками.</returns>
        private string MessageError(string role, string grade, string login)
        {
            string message = "";

            int errorRole = 0;
            for (int i = 0; i < CheckRole.Count(); i++)
                if (CheckRole[i] != role.ToLower())
                    errorRole++;

            if (errorRole == CheckRole.Count())
            {
                message += "Введённой вами роли не существует. Доступны следующие роли: ";
                foreach (var e in CheckRole)
                    message += $"{e}, ";
                message += "\n";
            }

            int errorGrade = 0;
            for (int i = 0; i < CheckGrade.Count(); i++)
                if (CheckGrade[i] != grade)
                    errorGrade++;

            if (errorGrade == CheckGrade.Count())
            {
                message += "Введённого вами класса не существует. Доступны следуюищие классы: ";
                foreach (var e in CheckGrade)
                    message += $"{e}, ";
                message += "\n";
            }

            if (CheckLogin(login))
            {
                message += "Введённый логин уже используется. Придумайте новый логин.";
            }

            return message;
        }


        /// <summary>
        /// Формирует подробное сообщение с ошибками.
        /// </summary>
        /// <param name="id">Проверяет введённый id.</param>
        /// <returns>Сформированное сообщение с ошибками.</returns>
        private string MessageError(string id)
        {
            string message = "";

            try
            {
                int check = int.Parse(id);
            }
            catch
            {
                message += "Введёный вами id имел не верный формат. Верным форматом является число(integer)";
            }

            return message;
        }



        /// <summary>
        /// Добавляет в таблицу User нового пользователя.
        /// </summary>
        /// <param name="FIO">Фамилия имя отчество пользователя.</param>
        /// <param name="role">Роль пользователя.</param>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <param name="grade">Класс пользователя.</param>
        /// <returns>Коментарий о выполненой работе.</returns>
        [HttpPost(Name = "AddmemberUser")]
        public string AddUser(string FIO, string role, string login, string password, string grade)
        {
            string errorMessage = MessageError(role, grade, login);

            if (errorMessage == "")
            {
                int newID = findMaxIdUser();
                HashPassword hashPassword = new HashPassword(password);
                using (DB db = new DB())
                {
                    UserModel user = new UserModel { id = newID, FIO = FIO, role = role.ToLower(), login = login, password = hashPassword.hashPassword, grade = grade.ToLower() };

                    db.User.Add(user);
                    db.SaveChanges();
                }
                return "Пользователь успешно добавлен!";
            }
            return errorMessage;
        }


        /// <summary>
        /// Удаляет пользователя из таблицы User.
        /// </summary>
        /// <param name="id">id пользователя которого нудно удалить.</param>
        /// <returns>Коментарий о выполненой работе.</returns>
        [HttpDelete(Name = "DeleteUser")]
        public string DeleteUser(string id)
        {
            string problem = MessageError(id);
            if (problem != "")
            {
                using (DB db = new DB())
                {
                    UserModel? user = db.User.Where(d => d.id == int.Parse(id)).FirstOrDefault();
                    if (user != null)
                    {
                        db.User.Remove(user);
                        db.SaveChanges();
                        return "Пользователь успешно удалён";
                    }
                }
            }
            return problem;
        }
    }
}
