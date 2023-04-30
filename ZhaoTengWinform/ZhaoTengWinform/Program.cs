using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZhaoTengWinform
{
    public class User
    {
        public User(string name,string password)
        {
            this.Username = name;
            this.Password = password;
        }
        public string Username { get; set; }
        public string Password { get; set; }

    }

    public sealed class GlobalInformation
    {
        private static GlobalInformation instance = null;
        private static readonly object obj = new object();
        private GlobalInformation()
        {

            this.UsersList.Add(new User("Admin", "Admin"));
            this.UsersList.Add(new User("SeniorEngineer", "SeniorEngineer"));
            this.UsersList.Add(new User("Engineer", "Engineer"));
            this.UsersList.Add(new User("Operator", "Operator"));
        }
        // 定义一个公共的静态属性来访问该实例，使用lock语句来保证线程安全
        public static GlobalInformation Instance
        {
            get
            {
                lock (obj)
                {
                    if (instance == null)
                    {
                        instance = new GlobalInformation();
                    }
                    return instance;
                }
            }
        }

        //全局参数
        public List<User> UsersList = new List<User>();//保存用户名及密码
    }


    internal static class Program
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UserLoad());
        }
    }
}
