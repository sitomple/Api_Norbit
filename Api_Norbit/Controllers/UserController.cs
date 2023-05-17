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
using System.Net;

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
        public List<UserModel> GetAllUser()
        {
            using (DB db = new DB())
            {
                var users = db.User.OrderByDescending(x => x.grade).ToList();
                return users;
            }
        }


        /// <summary>
        /// Поиск уникального id для нового пользователя.
        /// </summary>
        /// <returns>Уникальный id для пользователя.</returns>
        private int FindMaxIdUser()
        {
            int maxId = 0;
            using (DB db = new DB())
            {
                maxId = db.User.Max(u => u.id);
            }
            return maxId + 1;
        }



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
        /// <param name="login">Получаемый логин на проверку.</param>
        /// <returns>Сформированное сообщение с ошибками.</returns>
        private string MessageErrorLogin(string login)
        {
            string message = "";

            if (CheckLogin(login))
                message += "Введённый логин уже используется. Придумайте новый логин.";

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
        /// Проверяет существует ли роль и класс в БД.
        /// </summary>
        /// <param name="role">Проверяемая роль.</param>
        /// <param name="grade">Проверяемый класс.</param>
        /// <returns>Булевое значение: true - если прошли проверку, false - в противном случае.</returns>
        private bool CheckRoleAndGrade(string role, string grade)
        {
            bool checkRole = false;
            bool checkGrade = false;

            List<RoleModel> roles = new List<RoleModel>();
            using (DB db = new DB())
            {
                roles = db.Role.ToList();
            }

            List<ClassModel> grades = new List<ClassModel>();
            using (DB db = new DB())
            {
                grades = db.Grade.ToList();
            }

            foreach (var e in roles)
                if (role == e.role)
                    checkRole = true;

            foreach (var e in grades)
                if (grade == e.grade)
                    checkGrade = true;

            if (checkRole && checkGrade)
                return true;

            return false;
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
        public IActionResult AddUser(string FIO, string role, string login, string password, string grade)
        {
            string errorMessage = MessageErrorLogin(login); 
            bool checkRoleAndGrade = CheckRoleAndGrade(role.ToLower(), grade.ToLower());

            if (errorMessage == "" && checkRoleAndGrade)
            {
                int newID = FindMaxIdUser();
                HashPassword hashPassword = new HashPassword(password);
                using (DB db = new DB())
                {
                    UserModel user = new UserModel { id = newID, FIO = FIO, role = role.ToLower(), login = login, password = hashPassword.hashPassword, grade = grade.ToLower() };

                    db.User.Add(user);
                    db.SaveChanges();
                }
                return Ok("Пользователь добавлен!");
            }
            return BadRequest($"{errorMessage}");
        }


        /// <summary>
        /// Удаляет пользователя из таблицы User.
        /// </summary>
        /// <param name="id">id пользователя которого нудно удалить.</param>
        /// <returns>Коментарий о выполненой работе.</returns>
        [HttpDelete(Name = "DeleteUser")]
        public IActionResult DeleteUser(string id)
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
                        return Ok("Пользователь удалён!");
                    }
                }
            }
            return BadRequest("Проверьте введённый id!");
        }
    }
}
