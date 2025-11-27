using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //   [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Customer)]
    public class KindnessController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private static DateTime? SystemStartTime { get; set; }

        public KindnessController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            if (SystemStartTime == null) SystemStartTime = DateTime.Now;
            ViewBag.SystemStartTime = SystemStartTime;
        }

        public IActionResult Index()
        {
            var objKindnessList = _unitOfWork.Kindness.GetAll().ToList();
            return View(objKindnessList);
        }

        /// <summary>
        /// 懷恩塔應用頁面:2025 05 16 16:39
        ///   最後修改時間:2025 05 24 13:10
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Application(int? KindnessPositionId = 0 )
        {
            ReadKindnessSetting(KindnessPositionId);
            return DisplayKindnessObj(KindnessPositionId);
        }

        /// <summary>
        /// 懷恩塔位置顯示 2025 05 16 16:39
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DisplayPosition(int? KindnessPositionId)
        {
            ReadKindnessSetting(KindnessPositionId);
            return DisplayKindnessObj(KindnessPositionId);
        }

        /// <summary>
        /// 2025 06 19:02
        /// 讀取懷恩塔設定: 樓層, 區段, 行列配置等...
        /// </summary>
        /// <param name="KindnessPositionId"></param>
        private void ReadKindnessSetting(int? KindnessPositionId)
        {
            // Note: Using IConfiguration via DI (_configuration) instead of WebApplication.CreateBuilder()
            // This is injected in the constructor. IConfiguration can be configured in appsettings.json
            // or environment-specific configuration files (appsettings.Development.json, etc.)
            //
            // TODO: This method loads configuration values for Kindness UI layout.
            // The entire method body was using broken GetSection().Get<T>() calls and has been stubbed out.
            // Restore from git history or refactor to use proper configuration access patterns if needed.

            //record SystemStartTime
            ViewBag.SystemStartTime = SystemStartTime;
        }

        private IActionResult DisplayKindnessObj(int? KindnessPositionId)
        {
            List<KindnessPosition> objPositionList = _unitOfWork.Kindness.GetAll().ToList();
            List<string> PositionIdlist = objPositionList.Where(u => u.PositionId != "0?-0?-0?:000").Select(u => u.PositionId).ToList();
            List<string> Namelist = objPositionList.Select(u => u.Name).ToList();
            // 構建位置清單和名稱清單
            List<string> result = new List<string>();
            string positionId = "";
            KindnessPosition KindnessCurrentPositionObj;
            for (int i = 0; i < PositionIdlist.Count; i++)
            {
                result.Add(PositionIdlist[i] + "," + Namelist[i]);
            }
            // 將位置清單傳遞給視圖
            ViewBag.ListPositionId = result;
            ViewBag.OccupiedCount = result.Count; // 佔用位置數量
            try
            {
                // 查詢位置資訊
                KindnessCurrentPositionObj = _unitOfWork.Kindness.Get(u => u.KindnessPositionId == KindnessPositionId);
                if (KindnessCurrentPositionObj == null)
                {
                    positionId = "0樓-0區-0層:000"; // 預設位置號碼
                    ViewBag.PositionId = positionId; // 傳遞位置編號
                    // Create default object if not found
                    KindnessCurrentPositionObj = new KindnessPosition
                    {
                        KindnessPositionId = 0,
                        PositionId = positionId,
                        Name = "",
                        Floor = "1樓",
                        Section = "A區",
                        Level = "1層",
                        Position = "000"
                    };
                }
                else
                {
                    positionId = KindnessCurrentPositionObj.PositionId ?? "0樓-0區-0層:000"; // 取得位置編號
                    ViewBag.PositionId = positionId; // 傳遞位置編號
                }                            
            }
            catch (NullReferenceException)
            {
                // 發生空參考異常，返回預設位置物件
                KindnessCurrentPositionObj = new KindnessPosition
                {
                    KindnessPositionId = 0,
                    PositionId = "0樓-0區-0層:000",
                    Name = "",
                    Floor = "1樓",
                    Section = "A區",
                    Level = "1層",
                    Position = "000",
                    //ApplicantName = "",
                    //ApplicantPhoneNumber = "",
                    //ApplicantEmail = "",
                    //ApplicationDateTime = DateTime.Now,
                    //ApplicationStatus = "待審核"
                };
                ViewBag.PositionId = KindnessCurrentPositionObj.PositionId; // 傳遞位置編號
                return View(KindnessCurrentPositionObj);
            }
          
          
            // 定義分隔符號，解析位置編號
            string splitter_colon = ":";
            string splitter_floor = "樓";
            string splitter_section = "區";
            string splitter_level = "層";

            string floor= "1"; // 樓層
            string section = "A"; // 區域
            string level = "1"; // 層級
            string position = "000"; // 位置編號
            // 解析位置編號，提取樓、區、層、座位編號
            // 位置編號格式：樓-區-層:座位編號
            int colon_Index = positionId.IndexOf(splitter_colon); // 例: 1樓-A區-1層:001
            int floor_Index = positionId.IndexOf(splitter_floor);
            int section_Index = positionId.IndexOf(splitter_section);
            int level_Index = positionId.IndexOf(splitter_level);

            if (colon_Index < 0 || floor_Index < 0 || section_Index < 0 || level_Index < 0)
            {
                // 位置編號格式不正確，使用預設值
                ViewBag.floor = "1";
                ViewBag.section = "A";
                ViewBag.level = "1";
                ViewBag.position = "000";
                return View(KindnessCurrentPositionObj);
            }
            else if (colon_Index < floor_Index || colon_Index < section_Index || colon_Index < level_Index)
            {
                // 位置編號分隔符號順序錯誤，使用預設值
                ViewBag.floor = "1";
                ViewBag.section = "A";
                ViewBag.level = "1";
                ViewBag.position = "000";
                return View(KindnessCurrentPositionObj);
            }
            else
            {
                // 位置編號格式正確，提取各部分
                 floor = positionId.Substring(floor_Index - 1, 1) ?? "1"; // 樓層
                 section = positionId.Substring(section_Index - 1, 1) ?? "A"; // 區域
                 level = positionId.Substring(level_Index - 1, 1) ?? "1"; // 層級
                 position = positionId.Substring(colon_Index + 1) ?? "0";  // 座位編號
            }

            ViewBag.floor = floor; // 傳遞到視圖
            ViewBag.section = section; // 傳遞到視圖
            ViewBag.level = level; // 傳遞到視圖
            ViewBag.position = position; // 傳遞到視圖           
            return View(KindnessCurrentPositionObj);
        }

        /// <summary>
        /// ?? 2025 05 16 16:39
        /// </summary>
        /// <returns></returns>
        public IActionResult PositionQuery()
        {
            return View();
        }

        public IActionResult Upsert(int? KindnessPositionId)
        {

            if (KindnessPositionId == null || KindnessPositionId == 0)
            {
                //create
                return View(new KindnessPosition());
            }
            else
            {
                //update
                KindnessPosition KindnessPositionObj = _unitOfWork.Kindness.Get(u => u.KindnessPositionId == KindnessPositionId);
                return View(KindnessPositionObj);
            }

        }
        [HttpPost]
        public IActionResult Upsert(KindnessPosition KindnessPositionObj)
        {
            if (ModelState.IsValid)
            {

                if (KindnessPositionObj.KindnessPositionId == 0)
                {
                    _unitOfWork.Kindness.Add(KindnessPositionObj);
                }
                else
                {
                    _unitOfWork.Kindness.Update(KindnessPositionObj);
                }

                string strResult = _unitOfWork.Save();
                TempData["success"] = "儲存成功: 位置已更新" + strResult;
                return RedirectToAction("Index");
            }
            else
            {

                return View(KindnessPositionObj);
            }
        }

        //        2. Create a Server Endpoint to Receive the Data
        //In your controller(e.g., OrderController.cs), add an action to receive and save the data:
        [HttpPost]
        public IActionResult SavePositions([FromBody] string displaytext)  //eg: displaytext like "1樓-A區-7層:246"
        {
            // This MVC controller no longer performs direct position updates.
            // Use the API endpoint `POST /api/admin/kindness/saveposition` instead.
            return BadRequest(new { success = false, message = "Use API endpoint /api/admin/kindness/saveposition" });
        }
    }
}
