using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SQLite;
using Newtonsoft.Json;
using Dapper;

#region NuGet資訊
//Mod by 奇緯 20200424 新增 Dapper & SQLite.Core
#endregion


namespace Introduction.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        //連線路徑
        static string dbPath = "";
        //連線字串
        static string cnStr = "";
        #region SQLite連線參考資料
        /// <summary>
        /// SQL連線
        /// </summary>
        public void SQLiteDB()
        {
            //賦予路徑
            dbPath = HttpContext.Server.MapPath("~/SQLiteDB/GrowUp.db");
            //測試路徑(使用正式路徑時把這段註解)
            dbPath = HttpContext.Server.MapPath("~/SQLiteDB/Player.db");
            //賦予連線字串
            cnStr = "data source=" + dbPath;
            #region 測試連線
            //建立DB
            InitSQLiteDb();
            //新增資料
            TestInsert();
            //查詢資料
            TestSelect();
            #endregion      
        }
        static void InitSQLiteDb()
        {
            if (System.IO.File.Exists(dbPath)) return;
            using (var cn = new SQLiteConnection(cnStr))
            {
                cn.Execute(@" CREATE TABLE Player (
                              Id VARCHAR(16),
                              Name VARCHAR(32),
                              RegDate DATETIME,
                              Score INTEGER,
                              BinData BLOB,
                              CONSTRAINT Player_PK PRIMARY KEY (Id)  )");
            }
        }

        static void TestInsert()
        {
            using (var cn = new SQLiteConnection(cnStr))
            {
                cn.Execute("DELETE FROM Player");
                //參數是用@paramName
                var insertScript = "INSERT INTO Player VALUES (@Id, @Name, @RegDate, @Score, @BinData)";
                cn.Execute(insertScript, TestData);
                //測試Primary Key
                try
                {
                    //故意塞入錯誤資料
                    cn.Execute(insertScript, TestData[0]);
                    throw new ApplicationException("失敗：未阻止資料重複");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"測試成功:{ex.Message}");
                }
            }
        }

        static void TestSelect()
        {
            using (var cn = new SQLiteConnection(cnStr))
            {
                var list = cn.Query("SELECT * FROM Player");
                var Mapper_list = cn.Query<MapperPlayer>("SELECT * FROM Player");
            }
        }

        class MapperPlayer
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        class Player
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime RegDate { get; set; }
            public int Score { get; set; }
            public byte[] BinData { get; set; }
            public Player(string id, string name, DateTime regDate, int score)
            {
                Id = id;
                Name = name;
                RegDate = regDate;
                Score = score;
                BinData = Guid.NewGuid().ToByteArray().Take(4).ToArray();
            }
        }
        static Player[] TestData = new Player[]
        {
            new Player("P01", "Jeffrey", DateTime.Now, 32767),
            new Player("P02", "Darkthread", DateTime.Now, 65535),
        };

        #endregion



    }
}