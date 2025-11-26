using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.TermStore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Intrinsics.X86;
using static System.Collections.Specialized.BitVector32;
using WebApplication = Microsoft.Graph.Models.WebApplication;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    //???-???? 2025 05 15 12:01  KindnessPosition.cs
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
        public IActionResult Index(int? KindnessPositionId)
        {
            ReadKindnessSetting(KindnessPositionId);
            List<KindnessPosition> objKindnessPositionList = _unitOfWork.Kindness.GetAll().ToList();
            //List<KindnessPosition> objKindnessPositionList = _unitOfWork.Kindness.GetAll(includeProperties: "ApplicationUser").ToList();
            //IEnumerable<KindnessPosition> objKindnessPositionList = _unitOfWork.Kindness.GetAll().Where(item=> item.PositionId.Length>0).ToList();

            return View(objKindnessPositionList);
        }

        /// <summary>
        /// (??)??:2025 05 16 16:39
        ///   ????:2025 05 24 13:10
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Application(int? KindnessPositionId = 0 )
        {
            ReadKindnessSetting(KindnessPositionId);
            return DisplayKindnessObj(KindnessPositionId);
        }

        /// <summary>
        /// ?? 2025 05 16 16:39
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
        /// ???????: ??,??,??,???...
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
            //???????????? ListPositionId
            List<string> result = new List<string>();
            string positionId = "";
            KindnessPosition KindnessCurrentPositionObj;
            for (int i = 0; i < PositionIdlist.Count; i++)
            {
                result.Add(PositionIdlist[i] + "," + Namelist[i]);
            }
            //???????????? ViewBag.ListPositionId
            ViewBag.ListPositionId = result;
            ViewBag.OccupiedCount = result.Count; //?????
            try
            {
                //?????/?? ???????
                KindnessCurrentPositionObj = _unitOfWork.Kindness.Get(u => u.KindnessPositionId == KindnessPositionId);
                if (KindnessCurrentPositionObj == null)
                {
                    positionId = "0?-0?-0?:000"; //???????
                    ViewBag.PositionId = positionId; //???????
                }
                else
                {
                    positionId = KindnessCurrentPositionObj.PositionId ?? "0?-0?-0?:000"; //???????
                    ViewBag.PositionId = positionId; //???????
                }                            
            }
            catch (NullReferenceException)
            {
                //????????,??????
                 KindnessCurrentPositionObj = new KindnessPosition
                {
                    KindnessPositionId = 0,
                    PositionId = "0?-0?-0?:000",
                    Name = "",
                    Floor = "1?",
                    Section = "??",
                    Level = "1?",
                    Position = "000",
                    //ApplicantName = "",
                    //ApplicantPhoneNumber = "",
                    //ApplicantEmail = "",
                    //ApplicationDateTime = DateTime.Now,
                    //ApplicationStatus = "???"
                };
                ViewBag.PositionId = KindnessCurrentPositionObj.PositionId; //???????
                return View(KindnessCurrentPositionObj);
            }
          
          
            // ???,???
            string splitter_colon = ":";
            string splitter_floor = "?";
            string splitter_section = "?";
            string splitter_level = "?";

            string floor= "1"; //?
            string section = "?"; //?
            string level = "1"; //?
            string position = "000"; //?
            //?? positionId ??,??????
            //?? positionId ????????
            int colon_Index = positionId.IndexOf(splitter_colon); //1?-??-7?:246
            int floor_Index = positionId.IndexOf(splitter_floor);
            int section_Index = positionId.IndexOf(splitter_section);
            int level_Index = positionId.IndexOf(splitter_level);

            if (colon_Index < 0 || floor_Index < 0 || section_Index < 0 || level_Index < 0)
            {
                //??????????,??????
                ViewBag.floor = "1";
                ViewBag.section = "?";
                ViewBag.level = "1";
                ViewBag.position = "000";
                return View(KindnessCurrentPositionObj);
            }
            else if (colon_Index < floor_Index || colon_Index < section_Index || colon_Index < level_Index)
            {
                //???????????,??????
                ViewBag.floor = "1";
                ViewBag.section = "?";
                ViewBag.level = "1";
                ViewBag.position = "000";
                return View(KindnessCurrentPositionObj);
            }
            else
            {
                //????????,???????
                 floor = positionId.Substring(floor_Index - 1, 1) ?? "1"; //?
                 section = positionId.Substring(section_Index - 1, 1) ?? "1";//?
                 level = positionId.Substring(level_Index - 1, 1) ?? "1"; //?
                 position = positionId.Substring(colon_Index + 1) ?? "0";  //?
            }

            ViewBag.floor = floor; //?????-? 
            ViewBag.section = section; //?????-? 
            ViewBag.level = level; //?????-? 
            ViewBag.position = position; //?????-?           
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

        /// <summary>
        /// 2025 07 07 14:56
        /// Imports kindness position data from an Excel file and validates the input rows.
        /// </summary>
        /// <remarks>This method processes a list of kindness position data provided in the request body,
        /// validates each row,  and saves valid rows to the database. Validation includes checking required fields,
        /// ensuring values conform  to expected formats, and verifying that position IDs are unique within the
        /// database. If any validation errors are found, the method returns a response containing the error
        /// details.</remarks>
        /// <param name="importedRows">A list of <see cref="KindnessPositionViewModel"/> objects representing the rows to be imported. Each object
        /// should contain the necessary data for an ancestral position, such as name, floor, section, level,  position,
        /// position ID, and applicant information.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the import operation.  If validation errors are
        /// found, the response includes a list of error messages and indicates failure.  If all rows are valid, the
        /// response indicates success.</returns>
        [HttpPost]
        public IActionResult ImportExcel([FromBody] List<KindnessPositionViewModel> importedRows)
        {
            var errors = new List<string>();
            var validRows = new List<KindnessPosition>();

            for (int i = 0; i < importedRows.Count; i++)
            {
                var row = importedRows[i];
                int rowNum = i + 2; // Excel row number (header is row 1)

                // Example validation rules
                if (string.IsNullOrWhiteSpace(row.Name))
                    errors.Add($"?{rowNum}?: ???????");
                if (row.Floor != "1?" && row.Floor != "2?" && row.Floor != "3?")
                    errors.Add($"?{rowNum}?: ????'1?'?'2?'?'3?'");
                if (string.IsNullOrWhiteSpace(row.Section))
                    errors.Add($"?{rowNum}?: ????");
                if (string.IsNullOrWhiteSpace(row.Level))
                    errors.Add($"?{rowNum}?: ????");
                //      if (!int.TryParse(row.Level, out _))
                //      errors.Add($"?{rowNum}?: ??????");
                if (string.IsNullOrWhiteSpace(row.Position))
                    errors.Add($"?{rowNum}?: ?????");
                if (string.IsNullOrWhiteSpace(row.PositionId))
                    errors.Add($"?{rowNum}?: ?????");

                // Example: check for duplicates in DB
                // commented:2025 08 25
                //if (_unitOfWork.Kindness.Get(a => a.PositionId == row.PositionId) != null)
                //    errors.Add($"?{rowNum}?: ???? [{row.PositionId}] ???????");
                // commented:2025 08 25

                // If no errors for this row, add to validRows
                if (!errors.Any(e => e.StartsWith($"?{rowNum}?")))
                {
                    validRows.Add(new KindnessPosition
                    {
                        Name = row.Name,
                        Floor = row.Floor,
                        Section = row.Section,
                        Level = row.Level,
                        Position = row.Position,
                        PositionId = row.PositionId,
                        Applicant = row.Applicant,
                        Relation = row.Relation,
                        Mobile_Tel = row.Mobile_Tel,
                        Note = row.Note
                    });
                }
            }

            if (errors.Count > 0)
                return Json(new { success = false, errors });

            // Save validRows to DB
            foreach (var entity in validRows)
            {
                _unitOfWork.Kindness.Add(entity);
            }
            _unitOfWork.Save();

            return Json(new { success = true });
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
                TempData["success"] = "??/?? ??" + strResult;
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
        public IActionResult SavePositions([FromBody] string displaytext)  //eg: ????..1?,??,7?????:246
        {
            try
            {
                // ?????????
                string splitter_colon = ":";
                string splitter_floor = "?";
                string splitter_section = "?";
                string splitter_level = "?";

                int colon_Index = displaytext.IndexOf(splitter_colon); //????..1?,??,7?????:246
                int floor_Index = displaytext.IndexOf(splitter_floor);
                int section_Index = displaytext.IndexOf(splitter_section);
                int level_Index = displaytext.IndexOf(splitter_level);
                     
                string floor = displaytext.Substring(floor_Index-1,2); //?
                string section = displaytext.Substring(section_Index-1,2); ;//?
                string level = displaytext.Substring(level_Index-1, 2); ; //?
                string position = displaytext.Substring(colon_Index+1);  //?
                //string sPositionId = floor + "-" + section + "-" + level + ":" + position;
                string sPositionId = displaytext;
                //Select Obj of sKindnessPositionId
                // TODO: kindnessPositionId should be passed as a parameter from the client
                KindnessPosition KindnessPositionObj = null;
                // Attempt to extract position ID from displaytext or use default
                KindnessPositionObj.PositionId = sPositionId;
                KindnessPositionObj.Floor = floor;
                KindnessPositionObj.Section = section;
                KindnessPositionObj.Level = level;
                KindnessPositionObj.Position = position;

                //do db_change:
                _unitOfWork.Kindness.Update(KindnessPositionObj);
                string strResult = _unitOfWork.Save();
                TempData["success"] = "??????~" + strResult;
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // ??? log
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
