BulkyWeb\Areas\Admin\Controllers\KindnessController.cs
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
using System.Text.Json;
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
        private static int? SelectedPoistionId { set; get; } //??????? KindnessPositionID      
        private static string? SelectedName { set; get; } //???????Name

        private static DateTime? SystemStartTime { get; set; }
        public KindnessController(IUnitOfWork unitOfWork)
        {
            if (SystemStartTime != null && (DateTime.Now - SystemStartTime.Value).TotalMinutes > 3)
            {
                SystemStartTime = DateTime.Now;
            }
            else
            {
                SystemStartTime = DateTime.Now;
            }
            //redcord sytem start time
            ViewBag.SystemStartTime = SystemStartTime;
            _unitOfWork = unitOfWork;
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
            //record SystemStartTime
            ViewBag.SystemStartTime = SystemStartTime;
            SelectedPoistionId = KindnessPositionId; //???????ID
            var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder();
            // ?? ??? ?? ????? 2025 05 22 16:13
            ViewBag.KindnessFloor = builder.Configuration.GetSection("Kindness:Floor").Get<int>();
            ViewBag.KindnessSection = builder.Configuration.GetSection("Kindness:Section").Get<int>();
            ViewBag.KindnessLevel_3f = builder.Configuration.GetSection("Kindness:Level_3f").Get<int>();
            ViewBag.KindnessLevel_1f_2f = builder.Configuration.GetSection("Kindness:Level_1f_2f").Get<int>();
            ViewBag.KindnessPosition = builder.Configuration.GetSection("Kindness:Position").Get<int>();

            // Kindness Layout_1F
            ViewBag.KindnessLayout_1F = builder.Configuration.GetSection("Kindness:Layout_1F").Get<string>();
            // Kindness Layout_2F
            ViewBag.KindnessLayout_2F = builder.Configuration.GetSection("Kindness:Layout_2F").Get<string>();
            // Kindness Layout_3F
            ViewBag.KindnessLayout_3F = builder.Configuration.GetSection("Kindness:Layout_3F").Get<string>();

            //1 ?: r ??, c?? : f1ar, f1ac, f1br, f1bc, f1cr, f1cc, f1dr, f1dc, f1er, f1ec, f1fr, f1fc
            ViewBag.kf1ar = builder.Configuration.GetSection("Kindness:kf1ar:row").Get<int>();
            ViewBag.kf1ac = builder.Configuration.GetSection("Kindness:kf1ac:col").Get<int>();

            ViewBag.kf1br = builder.Configuration.GetSection("Kindness:kf1br:row").Get<int>();
            ViewBag.kf1bc = builder.Configuration.GetSection("Kindness:kf1bc:col").Get<int>();

            ViewBag.kf1cr = builder.Configuration.GetSection("Kindness:kf1cr:row").Get<int>();
            ViewBag.kf1cc = builder.Configuration.GetSection("Kindness:kf1cc:col").Get<int>();

            ViewBag.kf1dr = builder.Configuration.GetSection("Kindness:kf1dr:row").Get<int>();
            ViewBag.kf1dc = builder.Configuration.GetSection("Kindness:kf1dc:col").Get<int>();

            ViewBag.kf1er = builder.Configuration.GetSection("Kindness:kf1er:row").Get<int>();
            ViewBag.kf1ec = builder.Configuration.GetSection("Kindness:kf1ec:col").Get<int>();

            ViewBag.kf1fr = builder.Configuration.GetSection("Kindness:kf1fr:row").Get<int>();
            ViewBag.kf1fc = builder.Configuration.GetSection("Kindness:kf1fc:col").Get<int>();

            //2 ?:  r ??, c?? : f2ar, f2ac, f2br, f2bc, f2cr, f2cc, f2dr, f2dc, f2er, f2ec, f2fr, f2fc
            ViewBag.kf2ar = builder.Configuration.GetSection("Kindness:kf2ar:row").Get<int>();
            ViewBag.kf2ac = builder.Configuration.GetSection("Kindness:kf2ac:col").Get<int>();

            ViewBag.kf2br = builder.Configuration.GetSection("Kindness:kf2br:row").Get<int>();
            ViewBag.kf2bc = builder.Configuration.GetSection("Kindness:kf2bc:col").Get<int>();

            ViewBag.kf2cr = builder.Configuration.GetSection("Kindness:kf2cr:row").Get<int>();
            ViewBag.kf2cc = builder.Configuration.GetSection("Kindness:kf2cc:col").Get<int>();

            ViewBag.kf2dr = builder.Configuration.GetSection("Kindness:kf2dr:row").Get<int>();
            ViewBag.kf2dc = builder.Configuration.GetSection("Kindness:kf2dc:col").Get<int>();

            ViewBag.kf2er = builder.Configuration.GetSection("Kindness:kf2er:row").Get<int>();
            ViewBag.kf2ec = builder.Configuration.GetSection("Kindness:kf2ec:col").Get<int>();

            ViewBag.kf2fr = builder.Configuration.GetSection("Kindness:kf2fr:row").Get<int>();
            ViewBag.kf2fc = builder.Configuration.GetSection("Kindness:kf2fc:col").Get<int>();

            //3 ?:  r ??, c?? : f3ar, f3ac, f3br, f3bc, f3cr, f3cc, f3dr, f3dc, f3er, f3ec, f3fr, f3fc
            ViewBag.kf3ar = builder.Configuration.GetSection("Kindness:kf3ar:row").Get<int>();
            ViewBag.kf3ac = builder.Configuration.GetSection("Kindness:kf3ac:col").Get<int>();

            ViewBag.kf3br = builder.Configuration.GetSection("Kindness:kf3br:row").Get<int>();
            ViewBag.kf3bc = builder.Configuration.GetSection("Kindness:kf3bc:col").Get<int>();

            ViewBag.kf3cr = builder.Configuration.GetSection("Kindness:kf3cr:row").Get<int>();
            ViewBag.kf3cc = builder.Configuration.GetSection("Kindness:kf3cc:col").Get<int>();

            ViewBag.kf3dr = builder.Configuration.GetSection("Kindness:kf3dr:row").Get<int>();
            ViewBag.kf3dc = builder.Configuration.GetSection("Kindness:kf3dc:col").Get<int>();

            ViewBag.kf3er = builder.Configuration.GetSection("Kindness:kf3er:row").Get<int>();
            ViewBag.kf3ec = builder.Configuration.GetSection("Kindness:kf3ec:col").Get<int>();

            ViewBag.kf3fr = builder.Configuration.GetSection("Kindness:kf3fr:row").Get<int>();
            ViewBag.kf3fc = builder.Configuration.GetSection("Kindness:kf3fc:col").Get<int>();

            //???:??—????:
            //1 ?  ?? 4???(????: row1,row2,....row4)
            //1 ?  1?   kf1a.row1
            ViewBag.kf1arow1 = builder.Configuration.GetSection("kf1a:row1").Get<string>();
            //1 ?  2?   kf1a.row2
            ViewBag.kf1arow2 = builder.Configuration.GetSection("kf1a:row2").Get<string>();
            //1 ?  3?   kf1a.row3
            ViewBag.kf1arow3 = builder.Configuration.GetSection("kf1a:row3").Get<string>();
            //1 ?  4?   kf1a.row4
            ViewBag.kf1arow4 = builder.Configuration.GetSection("kf1a:row4").Get<string>();

            //1 ? ?? 4???(????: row1,row2,....row4)
            //1 ?  1?   kf1b.row1
            ViewBag.kf1brow1 = builder.Configuration.GetSection("kf1b:row1").Get<string>();
            //1 ?  2?   kf1b.row2
            ViewBag.kf1brow2 = builder.Configuration.GetSection("kf1b:row2").Get<string>();
            //1 ?  3?   kf1b.row3
            ViewBag.kf1brow3 = builder.Configuration.GetSection("kf1b:row3").Get<string>();
            //1 ?  4?   kf1b.row4
            ViewBag.kf1brow4 = builder.Configuration.GetSection("kf1b:row4").Get<string>();

            //1 ? ?? 4???(????: row1,row2,....row4)
            //1 ?  1?   kf1c.row1
            ViewBag.kf1crow1 = builder.Configuration.GetSection("kf1c:row1").Get<string>();
            //1 ?  2?   kf1c.row2
            ViewBag.kf1crow2 = builder.Configuration.GetSection("kf1c:row2").Get<string>();
            //1 ?  3?   kf1c.row3
            ViewBag.kf1crow3 = builder.Configuration.GetSection("kf1c:row3").Get<string>();
            //1 ?  4?   kf1c.row4
            ViewBag.kf1crow4 = builder.Configuration.GetSection("kf1c:row4").Get<string>();

            //1 ? ?? 4???(????: row1,row2,....row4)
            //1 ?  1?   kf1d.row1
            ViewBag.kf1drow1 = builder.Configuration.GetSection("kf1d:row1").Get<string>();
            //1 ?  2?   kf1d.row2
            ViewBag.kf1drow2 = builder.Configuration.GetSection("kf1d:row2").Get<string>();
            //1 ?  3?   kf1d.row3
            ViewBag.kf1drow3 = builder.Configuration.GetSection("kf1d:row3").Get<string>();
            //1 ?  4?   kf1d.row4
            ViewBag.kf1drow4 = builder.Configuration.GetSection("kf1d:row4").Get<string>();

            //1 ? ?? 4???(????: row1,row2,....row4)
            //1 ?  1?   kf1e.row1
            ViewBag.kf1erow1 = builder.Configuration.GetSection("kf1e:row1").Get<string>();
            //1 ?  2?   kf1e.row2
            ViewBag.kf1erow2 = builder.Configuration.GetSection("kf1e:row2").Get<string>();
            //1 ?  3?   kf1e.row3
            ViewBag.kf1erow3 = builder.Configuration.GetSection("kf1e:row3").Get<string>();
            //1 ?  4?   kf1e.row4
            ViewBag.kf1erow4 = builder.Configuration.GetSection("kf1e:row4").Get<string>();


            //1 ? ?? 4???(????: row1,row2,....row4)
            //1 ?  1?   kf1f.row1
            ViewBag.kf1frow1 = builder.Configuration.GetSection("kf1f:row1").Get<string>();
            //1 ?  2?   kf1f.row2
            ViewBag.kf1frow2 = builder.Configuration.GetSection("kf1f:row2").Get<string>();
            //1 ?  3?   kf1f.row3
            ViewBag.kf1frow3 = builder.Configuration.GetSection("kf1f:row3").Get<string>();
            //1 ?  4?   kf1f.row4
            ViewBag.kf1frow4 = builder.Configuration.GetSection("kf1f:row4").Get<string>();

            // 2 ? ?? 4???(????: row1,row2,....row4)
            // 1?   kf2a.row1
            ViewBag.kf2arow1 = builder.Configuration.GetSection("kf2a:row1").Get<string>();
            // 2?   kf2a.row2
            ViewBag.kf2arow2 = builder.Configuration.GetSection("kf2a:row2").Get<string>();
            // 3?   kf2a.row3
            ViewBag.kf2arow3 = builder.Configuration.GetSection("kf2a:row3").Get<string>();
            // 4?   kf2a.row4
            ViewBag.kf2arow4 = builder.Configuration.GetSection("kf2a:row4").Get<string>();

            // 2 ? ?? 4???(????: row1,row2,....row4)
            // 1?   kf2b.row1
            ViewBag.kf2brow1 = builder.Configuration.GetSection("kf2b:row1").Get<string>();
            // 2?   kf2b.row2
            ViewBag.kf2brow2 = builder.Configuration.GetSection("kf2b:row2").Get<string>();
            // 3?   kf2b.row3
            ViewBag.kf2brow3 = builder.Configuration.GetSection("kf2b:row3").Get<string>();
            // 4?   kf2b.row4
            ViewBag.kf2brow4 = builder.Configuration.GetSection("kf2b:row4").Get<string>();

            //??? 2 ? ?? 4???(????: row1,row2,....row4)
            // 1?   kf2c.row1
            ViewBag.kf2crow1 = builder.Configuration.GetSection("kf2c:row1").Get<string>();
            // 2?   kf2c.row2
            ViewBag.kf2crow2 = builder.Configuration.GetSection("kf2c:row2").Get<string>();
            // 3?   kf2c.row3
            ViewBag.kf2crow3 = builder.Configuration.GetSection("kf2c:row3").Get<string>();
            // 4?   kf2c.row4
            ViewBag.kf2crow4 = builder.Configuration.GetSection("kf2c:row4").Get<string>();

            //??? 2 ? ?? 4???(????: row1,row2,....row4)
            // 1?   kf2d.row1
            ViewBag.kf2drow1 = builder.Configuration.GetSection("kf2d:row1").Get<string>();
            // 2?   kf2d.row2
            ViewBag.kf2drow2 = builder.Configuration.GetSection("kf2d:row2").Get<string>();
            // 3?   kf2d.row3
            ViewBag.kf2drow3 = builder.Configuration.GetSection("kf2d:row3").Get<string>();
            // 4?   kf2d.row4
            ViewBag.kf2drow4 = builder.Configuration.GetSection("kf2d:row4").Get<string>();

            //??? 2 ? ?? 4???(????: row1,row2,....row4)
            // 1?   kf2e.row1
            ViewBag.kf2erow1 = builder.Configuration.GetSection("kf2e:row1").Get<string>();
            // 2?   kf2e.row2
            ViewBag.kf2erow2 = builder.Configuration.GetSection("kf2e:row2").Get<string>();
            // 3?   kf2e.row3
            ViewBag.kf2erow3 = builder.Configuration.GetSection("kf2e:row3").Get<string>();
            // 4?   kf2e.row4
            ViewBag.kf2erow4 = builder.Configuration.GetSection("kf2e:row4").Get<string>();


            //??? 2 ? ?? 4???(????: row1,row2,....row4)
            // 1?   kf2f.row1
            ViewBag.kf2frow1 = builder.Configuration.GetSection("kf2f:row1").Get<string>();
            // 2?   kf2f.row2
            ViewBag.kf2frow2 = builder.Configuration.GetSection("kf2f:row2").Get<string>();
            // 3?   kf2f.row3
            ViewBag.kf2frow3 = builder.Configuration.GetSection("kf2f:row3").Get<string>();
            // 4?   kf2f.row4
            ViewBag.kf2frow4 = builder.Configuration.GetSection("kf2f:row4").Get<string>();

            //??? 3 ? ?? 7???(????: row1,row2,....row7)
            // 1?   kf3a.row1
            ViewBag.kf3arow1 = builder.Configuration.GetSection("kf3a:row1").Get<string>();
            // 2?   kf3a.row2
            ViewBag.kf3arow2 = builder.Configuration.GetSection("kf3a:row2").Get<string>();
            // 3?   kf3a.row3
            ViewBag.kf3arow3 = builder.Configuration.GetSection("kf3a:row3").Get<string>();
            // 4?   kf3a.row4
            ViewBag.kf3arow4 = builder.Configuration.GetSection("kf3a:row4").Get<string>();
            // 5?   kf3a.row5
            ViewBag.kf3arow5 = builder.Configuration.GetSection("kf3a:row5").Get<string>();
            // 6?   kf3a.row6
            ViewBag.kf3arow6 = builder.Configuration.GetSection("kf3a:row6").Get<string>();
            // 7?   kf3a.row7
            ViewBag.kf3arow7 = builder.Configuration.GetSection("kf3a:row7").Get<string>();

            //??? 3 ? ?? 7???(????: row1,row2,....row7)
            // 1?   kf3b.row1
            ViewBag.kf3brow1 = builder.Configuration.GetSection("kf3b:row1").Get<string>();
            // 2?   kf3b.row2
            ViewBag.kf3brow2 = builder.Configuration.GetSection("kf3b:row2").Get<string>();
            // 3?   kf3b.row3
            ViewBag.kf3brow3 = builder.Configuration.GetSection("kf3b:row3").Get<string>();
            // 4?   kf3b.row4
            ViewBag.kf3brow4 = builder.Configuration.GetSection("kf3b:row4").Get<string>();
            // 5?   kf3b.row5
            ViewBag.kf3brow5 = builder.Configuration.GetSection("kf3b:row5").Get<string>();
            // 6?   kf3b.row6
            ViewBag.kf3brow6 = builder.Configuration.GetSection("kf3b:row6").Get<string>();
            // 7?   kf3b.row7
            ViewBag.kf3brow7 = builder.Configuration.GetSection("kf3b:row7").Get<string>();

            //??? 3 ? ?? 7???(????: row1,row2,....row7)
            // 1?   kf3c.row1
            ViewBag.kf3crow1 = builder.Configuration.GetSection("kf3c:row1").Get<string>();
            // 2?   kf3c.row2
            ViewBag.kf3crow2 = builder.Configuration.GetSection("kf3c:row2").Get<string>();
            // 3?   kf3c.row3
            ViewBag.kf3crow3 = builder.Configuration.GetSection("kf3c:row3").Get<string>();
            // 4?   kf3c.row4
            ViewBag.kf3crow4 = builder.Configuration.GetSection("kf3c:row4").Get<string>();
            // 5?   kf3c.row5
            ViewBag.kf3crow5 = builder.Configuration.GetSection("kf3c:row5").Get<string>();
            // 6?   kf3c.row6
            ViewBag.kf3crow6 = builder.Configuration.GetSection("kf3c:row6").Get<string>();
            // 7?   kf3c.row7
            ViewBag.kf3crow7 = builder.Configuration.GetSection("kf3c:row7").Get<string>();

            //??? 3 ? ?? 7???(????: row1,row2,....row7)
            // 1?   kf3d.row1
            ViewBag.kf3drow1 = builder.Configuration.GetSection("kf3d:row1").Get<string>();
            // 2?   kf3d.row2
            ViewBag.kf3drow2 = builder.Configuration.GetSection("kf3d:row2").Get<string>();
            // 3?   kf3d.row3
            ViewBag.kf3drow3 = builder.Configuration.GetSection("kf3d:row3").Get<string>();
            // 4?   kf3d.row4
            ViewBag.kf3drow4 = builder.Configuration.GetSection("kf3d:row4").Get<string>();
            // 5?   kf3d.row5
            ViewBag.kf3drow5 = builder.Configuration.GetSection("kf3d:row5").Get<string>();
            // 6?   kf3d.row6
            ViewBag.kf3drow6 = builder.Configuration.GetSection("kf3d:row6").Get<string>();
            // 7?   kf3drow7
            ViewBag.kf3drow7 = builder.Configuration.GetSection("kf3d:row7").Get<string>();

            //??? 3 ? ?? 7???(????: row1,row2,....row7)
            // 1?   kf3e.row1
            ViewBag.kf3erow1 = builder.Configuration.GetSection("kf3e:row1").Get<string>();
            // 2?   kf3e.row2
            ViewBag.kf3erow2 = builder.Configuration.GetSection("kf3e:row2").Get<string>();
            // 3?   kf3e.row3
            ViewBag.kf3erow3 = builder.Configuration.GetSection("kf3e:row3").Get<string>();
            // 4?   kf3e.row4
            ViewBag.kf3erow4 = builder.Configuration.GetSection("kf3e:row4").Get<string>();
            // 5?   kf3e.row5
            ViewBag.kf3erow5 = builder.Configuration.GetSection("kf3e:row5").Get<string>();
            // 6?   kf3e.row6
            ViewBag.kf3erow6 = builder.Configuration.GetSection("kf3e:row6").Get<string>();
            // 7?   kf3erow7
            ViewBag.kf3erow7 = builder.Configuration.GetSection("kf3e:row7").Get<string>();

            //??? 3 ? ?? 7???(????: row1,row2,....row7)
            // 1?   kf3f.row1
            ViewBag.kf3frow1 = builder.Configuration.GetSection("kf3f:row1").Get<string>();
            // 2?   kf3f.row2
            ViewBag.kf3frow2 = builder.Configuration.GetSection("kf3f:row2").Get<string>();
            // 3?   kf3f.row3
            ViewBag.kf3frow3 = builder.Configuration.GetSection("kf3f:row3").Get<string>();
            // 4?   kf3f.row4
            ViewBag.kf3frow4 = builder.Configuration.GetSection("kf3f:row4").Get<string>();
            // 5?   kf3f.row5
            ViewBag.kf3frow5 = builder.Configuration.GetSection("kf3f:row5").Get<string>();
            // 6?   kf3f.row6
            ViewBag.kf3frow6 = builder.Configuration.GetSection("kf3f:row6").Get<string>();
            // 7?   kf3f.row7
            ViewBag.kf3frow7 = builder.Configuration.GetSection("kf3f:row7").Get<string>();

            // ????: @ViewBag.PublishDate
            ViewBag.PublishDate = builder.Configuration.GetValue<string>("PublishDate");

            //??????
            ViewBag.connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Auto logout settings
            float? AUTO_LOGOUT_MINUTE = builder.Configuration.GetSection("Logout_Duration:AUTO_LOGOUT_MINUTE").Get<float>();
            int? WARNING_BEFORE_LOGOUT_SECOND = builder.Configuration.GetSection("Logout_Duration:WARNING_BEFORE_LOGOUT_SECOND").Get<int>();

            // If AUTO_LOGOUT_MINUTE or WARNING_BEFORE_LOGOUT_SECOND is not set, use default values
            if (AUTO_LOGOUT_MINUTE == null || WARNING_BEFORE_LOGOUT_SECOND == null)
            {
                //Default values if not set in configuration
                AUTO_LOGOUT_MINUTE = (float?)0.5; // 30 minutes
                WARNING_BEFORE_LOGOUT_SECOND = 10; // 10 seconds                                                   
            }
            // Set ViewBag for use in the view
            ViewBag.AUTO_LOGOUT_MINUTE = AUTO_LOGOUT_MINUTE;
            ViewBag.WARNING_BEFORE_LOGOUT_SECOND = WARNING_BEFORE_LOGOUT_SECOND;


            // Work_Duration is used for the duration of work in the system
            float? Work_Duration = builder.Configuration.GetSection("Work_Duration").Get<float>();
            float? Import_Duration = builder.Configuration.GetSection("Import_Duration").Get<float>();
            int? WORK_WARNING_SECONDS = builder.Configuration.GetSection("WORK_WARNING_SECONDS").Get<int>();

            // If Work_Duration is not set, use default value
            if (Work_Duration == null)
            {
                // Default value if not set in configuration
                Work_Duration = (float?)1.0; // 1 mins          
            }
            ViewBag.Work_Duration = Work_Duration;
            // If Work_Duration is not set, use default value
            if (Import_Duration == null)
            {
                // Default value if not set in configuration
                Import_Duration = (float?)3.0; // 1 mins
            }
            ViewBag.Import_Duration = Import_Duration;

            // Set ViewBag for WORK_WARNING_SECONDS for use in the view
            if (WORK_WARNING_SECONDS == null)
            {
                // Default value if not set in configuration
                WORK_WARNING_SECONDS = 60; // 60 seconds          
            }
            ViewBag.WORK_WARNING_SECONDS = WORK_WARNING_SECONDS; // Default value will be null if not set  

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
                //???????????????
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

                // ?????? server payload????????
                ViewBag.ServerPayloadJson = JsonSerializer.Serialize(BuildKindnessServerPayload());
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
            //?? positionId ?????????
            //?? positionId ????????
            int colon_Index = positionId.IndexOf(splitter_colon); //1?-??-7?:246
            int floor_Index = positionId.IndexOf(splitter_floor);
            int section_Index = positionId.IndexOf(splitter_section);
            int level_Index = positionId.IndexOf(splitter_level);

            if (colon_Index < 0 || floor_Index < 0 || section_Index < 0 || level_Index < 0)
            {
                //?????????????????
                ViewBag.floor = "1";
                ViewBag.section = "?";
                ViewBag.level = "1";
                ViewBag.position = "000";

                // ?????? server payload????????
                ViewBag.ServerPayloadJson = JsonSerializer.Serialize(BuildKindnessServerPayload());
                return View(KindnessCurrentPositionObj);
            }
            else if (colon_Index < floor_Index || colon_Index < section_Index || colon_Index < level_Index)
            {
                //??????????????????
                ViewBag.floor = "1";
                ViewBag.section = "?";
                ViewBag.level = "1";
                ViewBag.position = "000";

                // ?????? server payload????????
                ViewBag.ServerPayloadJson = JsonSerializer.Serialize(BuildKindnessServerPayload());
                return View(KindnessCurrentPositionObj);
            }
            else
            {
                //????????????????
                 floor = positionId.Substring(floor_Index - 1, 1) ?? "1"; //?
                 section = positionId.Substring(section_Index - 1, 1) ?? "1";//?
                 level = positionId.Substring(level_Index - 1, 1) ?? "1"; //?
                 position = positionId.Substring(colon_Index + 1) ?? "0";  //?
            }

            ViewBag.floor = floor; //?????-? 
            ViewBag.section = section; //?????-? 
            ViewBag.level = level; //?????-? 
            ViewBag.position = position; //?????-?           

            // ?????? server payload????????
            ViewBag.ServerPayloadJson = JsonSerializer.Serialize(BuildKindnessServerPayload());

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


        #region API CALLS

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
                KindnessPosition KindnessPositionObj = _unitOfWork.Kindness.Get(u => u.KindnessPositionId==SelectedPoistionId);
                //Update PositionId
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
        //2	Adjust the parameter type to match your data structure(e.g., List<int>, List<PositionModel>, etc.).
        //        
        //3. Save to Database
        //Inside the controller action, use your Entity Framework context or other data access code to save the received data.
        //        ---
        //Summary:
        //•	Use JavaScript to send localStorage data to the server via AJAX.
        //•	Create a controller action to receive and save the data.
        //•	Save the data to your database using your data access layer.
        //Gotcha:
        //•	Make sure your endpoint URL matches your route.
        //•	Ensure the data format sent from JS matches what your controller expects.
        //Let me know if you need a full working example for your specific data structure!
        //* 2025 08 27 01:12 ??????  
        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IEnumerable<KindnessPosition> objKindnessPositionList;
            if (!string.IsNullOrWhiteSpace(search))
            {
                objKindnessPositionList = _unitOfWork.Kindness.GetAll(
                    filter: x => x.Name != null && x.Name.Contains(search)
                ).ToList();
            }
            else
            {
                objKindnessPositionList = _unitOfWork.Kindness.GetAll().ToList();
            }

            foreach(var item in objKindnessPositionList)
            {

                try
                {
                    //2025 07 16 15:20 new encrpted_name,applicant
                    if (item.Applicant == null)
                    {
                        continue; //????
                    }
                    else if (item.Applicant.Length < 2)
                    {
                        continue; //?????????
                    }
                    else if (item.Applicant.Length > 2)
                    {
                        //string encrpted_name = item.Name.Substring(0, 1) + "*" + item.Name.Substring(2, item.Name.Length - 2) ?? "";
                        string encrpted_applicant = item.Applicant.Substring(0, 1) + "*" + item.Applicant.Substring(2, item.Applicant.Length - 2) ?? ""; //???????
                        //item.Name = encrpted_name; //????                   
                        item.Applicant = encrpted_applicant; //???????
                    }
                    else if (item.Applicant.Length == 2)
                    {
                        //string encrpted_name = item.Name.Substring(0, 2) + "*"; //????
                        string encrpted_applicant = item.Applicant.Substring(0, 2) + "*"; //???????
                        //item.Name = encrpted_name; //????                   
                        item.Applicant = encrpted_applicant; //???????
                    }
                    else
                    {
                        return Json(new { success = false, message = "?????????!!" });
                    }
                }
                catch (Exception ex)
                {
                    continue; //????[??]??????
                }

            }
            return Json(new { data = objKindnessPositionList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var KindnessToBeDeleted = _unitOfWork.Kindness.Get(u => u.KindnessPositionId == id);
            if (KindnessToBeDeleted == null)
            {
                return Json(new { success = false, message = "????" });
            }

            _unitOfWork.Kindness.Remove(KindnessToBeDeleted);
            string strResult = _unitOfWork.Save();

            return Json(new { success = true, message = "????" });
        }


        /// <summary>
        /// ???? 2025.07.07 22:12
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteRange([FromBody] List<int> ids)
        {
            foreach (var id in ids)
            {
                var entity = _unitOfWork.Kindness.Get(x => x.KindnessPositionId == id);
                if (entity != null)
                    _unitOfWork.Kindness.Remove(entity);
            }
            _unitOfWork.Save();
            return Json(new { success = true });
        }

        /// <summary>
        /// ???? 2025.07.07 22:12
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteAll()
        {
            var all = _unitOfWork.Kindness.GetAll().ToList();
            foreach (var entity in all)
                _unitOfWork.Kindness.Remove(entity);
            _unitOfWork.Save();
            return Json(new { success = true });
        }


        #endregion

        /// <summary>
        /// ?????????? payload??? View ???? JSON?
        /// </summary>
        /// <returns>?????????????????</returns>
        private object BuildKindnessServerPayload()
        {
            return new
            {
                KindnessFloor = ViewBag.KindnessFloor,
                KindnessSection = ViewBag.KindnessSection,
                KindnessLevel_3f = ViewBag.KindnessLevel_3f,
                KindnessLevel_1f_2f = ViewBag.KindnessLevel_1f_2f,
                KindnessPosition = ViewBag.KindnessPosition,
                KindnessLayout_1F = ViewBag.KindnessLayout_1F,
                KindnessLayout_2F = ViewBag.KindnessLayout_2F,
                KindnessLayout_3F = ViewBag.KindnessLayout_3F,

                kf1ar = ViewBag.kf1ar,
                kf1ac = ViewBag.kf1ac,
                kf1br = ViewBag.kf1br,
                kf1bc = ViewBag.kf1bc,
                kf1cr = ViewBag.kf1cr,
                kf1cc = ViewBag.kf1cc,
                kf1dr = ViewBag.kf1dr,
                kf1dc = ViewBag.kf1dc,
                kf1er = ViewBag.kf1er,
                kf1ec = ViewBag.kf1ec,
                kf1fr = ViewBag.kf1fr,
                kf1fc = ViewBag.kf1fc,

                kf2ar = ViewBag.kf2ar,
                kf2ac = ViewBag.kf2ac,
                kf2br = ViewBag.kf2br,
                kf2bc = ViewBag.kf2bc,
                kf2cr = ViewBag.kf2cr,
                kf2cc = ViewBag.kf2cc,
                kf2dr = ViewBag.kf2dr,
                kf2dc = ViewBag.kf2dc,
                kf2er = ViewBag.kf2er,
                kf2ec = ViewBag.kf2ec,
                kf2fr = ViewBag.kf2fr,
                kf2fc = ViewBag.kf2fc,

                kf3ar = ViewBag.kf3ar,
                kf3ac = ViewBag.kf3ac,
                kf3br = ViewBag.kf3br,
                kf3bc = ViewBag.kf3bc,
                kf3cr = ViewBag.kf3cr,
                kf3cc = ViewBag.kf3cc,
                kf3dr = ViewBag.kf3dr,
                kf3dc = ViewBag.kf3dc,
                kf3er = ViewBag.kf3er,
                kf3ec = ViewBag.kf3ec,
                kf3fr = ViewBag.kf3fr,
                kf3fc = ViewBag.kf3fc,

                // ? row ??
                kf1arow1 = ViewBag.kf1arow1,
                kf1arow2 = ViewBag.kf1arow2,
                kf1arow3 = ViewBag.kf1arow3,
                kf1arow4 = ViewBag.kf1arow4,

                kf1brow1 = ViewBag.kf1brow1,
                kf1brow2 = ViewBag.kf1brow2,
                kf1brow3 = ViewBag.kf1brow3,
                kf1brow4 = ViewBag.kf1brow4,

                kf1crow1 = ViewBag.kf1crow1,
                kf1crow2 = ViewBag.kf1crow2,
                kf1crow3 = ViewBag.kf1crow3,
                kf1crow4 = ViewBag.kf1crow4,

                kf1drow1 = ViewBag.kf1drow1,
                kf1drow2 = ViewBag.kf1drow2,
                kf1drow3 = ViewBag.kf1drow3,
                kf1drow4 = ViewBag.kf1drow4,

                kf1erow1 = ViewBag.kf1erow1,
                kf1erow2 = ViewBag.kf1erow2,
                kf1erow3 = ViewBag.kf1erow3,
                kf1erow4 = ViewBag.kf1erow4,

                kf1frow1 = ViewBag.kf1frow1,
                kf1frow2 = ViewBag.kf1frow2,
                kf1frow3 = ViewBag.kf1frow3,
                kf1frow4 = ViewBag.kf1frow4,

                // 2F rows...
                kf2arow1 = ViewBag.kf2arow1,
                kf2arow2 = ViewBag.kf2arow2,
                kf2arow3 = ViewBag.kf2arow3,
                kf2arow4 = ViewBag.kf2arow4,
                kf2brow1 = ViewBag.kf2brow1,
                kf2brow2 = ViewBag.kf2brow2,
                kf2brow3 = ViewBag.kf2brow3,
                kf2brow4 = ViewBag.kf2brow4,
                kf2crow1 = ViewBag.kf2crow1,
                kf2crow2 = ViewBag.kf2crow2,
                kf2crow3 = ViewBag.kf2crow3,
                kf2crow4 = ViewBag.kf2crow4,
                kf2drow1 = ViewBag.kf2drow1,
                kf2drow2 = ViewBag.kf2drow2,
                kf2drow3 = ViewBag.kf2drow3,
                kf2drow4 = ViewBag.kf2drow4,
                kf2erow1 = ViewBag.kf2erow1,
                kf2erow2 = ViewBag.kf2erow2,
                kf2erow3 = ViewBag.kf2erow3,
                kf2erow4 = ViewBag.kf2erow4,
                kf2frow1 = ViewBag.kf2frow1,
                kf2frow2 = ViewBag.kf2frow2,
                kf2frow3 = ViewBag.kf2frow3,
                kf2frow4 = ViewBag.kf2frow4,

                // 3F rows...
                kf3arow1 = ViewBag.kf3arow1,
                kf3arow2 = ViewBag.kf3arow2,
                kf3arow3 = ViewBag.kf3arow3,
                kf3arow4 = ViewBag.kf3arow4,
                kf3arow5 = ViewBag.kf3arow5,
                kf3arow6 = ViewBag.kf3arow6,
                kf3arow7 = ViewBag.kf3arow7,

                kf3brow1 = ViewBag.kf3brow1,
                kf3brow2 = ViewBag.kf3brow2,
                kf3brow3 = ViewBag.kf3brow3,
                kf3brow4 = ViewBag.kf3brow4,
                kf3brow5 = ViewBag.kf3brow5,
                kf3brow6 = ViewBag.kf3brow6,
                kf3brow7 = ViewBag.kf3brow7,

                kf3crow1 = ViewBag.kf3crow1,
                kf3crow2 = ViewBag.kf3crow2,
                kf3crow3 = ViewBag.kf3crow3,
                kf3crow4 = ViewBag.kf3crow4,
                kf3crow5 = ViewBag.kf3crow5,
                kf3crow6 = ViewBag.kf3crow6,
                kf3crow7 = ViewBag.kf3crow7,

                kf3drow1 = ViewBag.kf3drow1,
                kf3drow2 = ViewBag.kf3drow2,
                kf3drow3 = ViewBag.kf3drow3,
                kf3drow4 = ViewBag.kf3drow4,
                kf3drow5 = ViewBag.kf3drow5,
                kf3drow6 = ViewBag.kf3drow6,
                kf3drow7 = ViewBag.kf3drow7,

                kf3erow1 = ViewBag.kf3erow1,
                kf3erow2 = ViewBag.kf3erow2,
                kf3erow3 = ViewBag.kf3erow3,
                kf3erow4 = ViewBag.kf3erow4,
                kf3erow5 = ViewBag.kf3erow5,
                kf3erow6 = ViewBag.kf3erow6,
                kf3erow7 = ViewBag.kf3erow7,

                kf3frow1 = ViewBag.kf3frow1,
                kf3frow2 = ViewBag.kf3frow2,
                kf3frow3 = ViewBag.kf3frow3,
                kf3frow4 = ViewBag.kf3frow4,
                kf3frow5 = ViewBag.kf3frow5,
                kf3frow6 = ViewBag.kf3frow6,
                kf3frow7 = ViewBag.kf3frow7,

                // ??
                ListPositionId = ViewBag.ListPositionId,
                OccupiedCount = ViewBag.OccupiedCount,
                PositionId = ViewBag.PositionId,
                floor = ViewBag.floor,
                section = ViewBag.section,
                level = ViewBag.level,
                position = ViewBag.position,
                PublishDate = ViewBag.PublishDate,
                AUTO_LOGOUT_MINUTE = ViewBag.AUTO_LOGOUT_MINUTE,
                WARNING_BEFORE_LOGOUT_SECOND = ViewBag.WARNING_BEFORE_LOGOUT_SECOND,
                Work_Duration = ViewBag.Work_Duration,
                WORK_WARNING_SECONDS = ViewBag.WORK_WARNING_SECONDS
            };
        }
    }
}