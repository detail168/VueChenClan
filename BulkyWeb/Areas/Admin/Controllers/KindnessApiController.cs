using System;
using System.Collections.Generic;
using System.Linq;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/admin/kindness")]
    public class KindnessApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public KindnessApiController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        /// <summary>
        /// GET /api/admin/kindness
        /// Search and return all kindness positions with encrypted applicant names
        /// </summary>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? search)
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

            // Encrypt applicant names for privacy
            foreach (var item in objKindnessPositionList)
            {
                try
                {
                    if (item.Applicant == null)
                    {
                        continue;
                    }
                    else if (item.Applicant.Length < 2)
                    {
                        continue;
                    }
                    else if (item.Applicant.Length > 2)
                    {
                        string encrpted_applicant = item.Applicant.Substring(0, 1) + "*" + item.Applicant.Substring(2, item.Applicant.Length - 2) ?? "";
                        item.Applicant = encrpted_applicant;
                    }
                    else if (item.Applicant.Length == 2)
                    {
                        string encrpted_applicant = item.Applicant.Substring(0, 2) + "*";
                        item.Applicant = encrpted_applicant;
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "加密申請人姓名失敗!!" });
                    }
                }
                catch
                {
                    continue;
                }
            }

            return Ok(new { data = objKindnessPositionList });
        }

        /// <summary>
        /// DELETE /api/admin/kindness/{id}
        /// Delete a single kindness position by ID
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int? id)
        {
            var KindnessToBeDeleted = _unitOfWork.Kindness.Get(u => u.KindnessPositionId == id);
            if (KindnessToBeDeleted == null)
            {
                return BadRequest(new { success = false, message = "刪除失敗" });
            }

            _unitOfWork.Kindness.Remove(KindnessToBeDeleted);
            string strResult = _unitOfWork.Save();

            return Ok(new { success = true, message = "刪除成功" });
        }

        /// <summary>
        /// POST /api/admin/kindness/deleterange
        /// Delete multiple kindness positions by list of IDs
        /// </summary>
        [HttpPost("deleterange")]
        public IActionResult DeleteRange([FromBody] List<int> ids)
        {
            foreach (var id in ids)
            {
                var entity = _unitOfWork.Kindness.Get(x => x.KindnessPositionId == id);
                if (entity != null)
                    _unitOfWork.Kindness.Remove(entity);
            }
            _unitOfWork.Save();
            return Ok(new { success = true });
        }

        /// <summary>
        /// POST /api/admin/kindness/deleteall
        /// Delete all kindness positions
        /// </summary>
        [HttpPost("deleteall")]
        public IActionResult DeleteAll()
        {
            var all = _unitOfWork.Kindness.GetAll().ToList();
            foreach (var entity in all)
                _unitOfWork.Kindness.Remove(entity);
            _unitOfWork.Save();
            return Ok(new { success = true });
        }
    }
}
