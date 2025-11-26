using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/ancestral")]
    [ApiController]
    public class AncestralApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AncestralApiController> _logger;

        public AncestralApiController(IUnitOfWork unitOfWork, ILogger<AncestralApiController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? search)
        {
            IEnumerable<AncestralPosition> list;
            if (!string.IsNullOrWhiteSpace(search))
            {
                list = _unitOfWork.Ancestral.GetAll(filter: x => x.Name != null && x.Name.Contains(search)).ToList();
            }
            else
            {
                list = _unitOfWork.Ancestral.GetAll().ToList();
            }

            // Mask applicant names as before
            foreach (var item in list)
            {
                try
                {
                    if (string.IsNullOrEmpty(item.Applicant)) continue;
                    if (item.Applicant.Length > 2)
                    {
                        item.Applicant = item.Applicant.Substring(0, 1) + "*" + item.Applicant.Substring(2);
                    }
                    else if (item.Applicant.Length == 2)
                    {
                        item.Applicant = item.Applicant.Substring(0, 2) + "*";
                    }
                }
                catch
                {
                    // ignore masking errors
                    continue;
                }
            }

            return Ok(new { data = list });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var entity = _unitOfWork.Ancestral.Get(u => u.AncestralPositionId == id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AncestralPositionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = new AncestralPosition
            {
                Name = dto.Name,
                PositionId = dto.PositionId,
                Side = dto.Side,
                Section = dto.Section,
                Level = dto.Level,
                Position = dto.Position,
                Applicant = dto.Applicant,
                Relation = dto.Relation,
                Mobile_Tel = dto.Mobile_Tel,
                Note = dto.Note
            };
            _unitOfWork.Ancestral.Add(entity);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(GetById), new { id = entity.AncestralPositionId }, entity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AncestralPositionDto dto)
        {
            var entity = _unitOfWork.Ancestral.Get(u => u.AncestralPositionId == id);
            if (entity == null) return NotFound();
            entity.Name = dto.Name;
            entity.PositionId = dto.PositionId;
            entity.Side = dto.Side;
            entity.Section = dto.Section;
            entity.Level = dto.Level;
            entity.Position = dto.Position;
            entity.Applicant = dto.Applicant;
            entity.Relation = dto.Relation;
            entity.Mobile_Tel = dto.Mobile_Tel;
            entity.Note = dto.Note;

            _unitOfWork.Ancestral.Update(entity);
            _unitOfWork.Save();
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _unitOfWork.Ancestral.Get(u => u.AncestralPositionId == id);
            if (entity == null) return NotFound(new { success = false, message = "刪除失敗" });
            _unitOfWork.Ancestral.Remove(entity);
            _unitOfWork.Save();
            return Ok(new { success = true, message = "刪除成功" });
        }

        [HttpPost("import")]
        public IActionResult Import([FromBody] List<ImportRowDto> rows)
        {
            var errors = new List<string>();
            var valid = new List<AncestralPosition>();
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int rowNum = i + 2;
                if (string.IsNullOrWhiteSpace(r.Name)) errors.Add($"第{rowNum}行: 祖先姓名為必填");
                if (r.Side != "左側" && r.Side != "右側" && r.Side != "中間") errors.Add($"第{rowNum}行: 側必須為'左側'或'右側'或'中間");
                if (string.IsNullOrWhiteSpace(r.Section)) errors.Add($"第{rowNum}行: 區為必填");
                if (string.IsNullOrWhiteSpace(r.Level)) errors.Add($"第{rowNum}行: 層為必填");
                if (string.IsNullOrWhiteSpace(r.Position)) errors.Add($"第{rowNum}行: 編號為必填");
                if (string.IsNullOrWhiteSpace(r.PositionId)) errors.Add($"第{rowNum}行: 牌位為必填");
                if (_unitOfWork.Ancestral.Get(a => a.PositionId == r.PositionId) != null) errors.Add($"第{rowNum}行: 牌位編號 [{r.PositionId}] 已存在於資料庫");

                if (!errors.Any(e => e.StartsWith($"第{rowNum}行")))
                {
                    valid.Add(new AncestralPosition
                    {
                        Name = r.Name,
                        Side = r.Side,
                        Section = r.Section,
                        Level = r.Level,
                        Position = r.Position,
                        PositionId = r.PositionId,
                        Applicant = r.Applicant,
                        Relation = r.Relation,
                        Mobile_Tel = r.Mobile_Tel,
                        Note = r.Note
                    });
                }
            }

            if (errors.Count > 0) return BadRequest(new { success = false, errors });
            foreach (var e in valid) _unitOfWork.Ancestral.Add(e);
            _unitOfWork.Save();
            return Ok(new { success = true });
        }

        [HttpPost("saverange")]
        public IActionResult DeleteRange([FromBody] List<int> ids)
        {
            foreach (var id in ids)
            {
                var entity = _unitOfWork.Ancestral.Get(x => x.AncestralPositionId == id);
                if (entity != null) _unitOfWork.Ancestral.Remove(entity);
            }
            _unitOfWork.Save();
            return Ok(new { success = true });
        }

        [HttpPost("deleteall")]
        public IActionResult DeleteAll()
        {
            var all = _unitOfWork.Ancestral.GetAll().ToList();
            foreach (var entity in all) _unitOfWork.Ancestral.Remove(entity);
            _unitOfWork.Save();
            return Ok(new { success = true });
        }

        [HttpPost("saveposition")]
        public IActionResult SavePosition([FromBody] SavePositionDto dto)
        {
            try
            {
                var displaytext = dto.DisplayText;
                var colon_Index = displaytext.IndexOf(":");
                var splitter_side = "側";
                var splitter_section = "區";
                var splitter_level = "層";
                var side_Index = displaytext.IndexOf(splitter_side);
                var section_Index = displaytext.IndexOf(splitter_section);
                var level_Index = displaytext.IndexOf(splitter_level);

                var side = displaytext.Substring(side_Index - 1, 2);
                var section = displaytext.Substring(section_Index - 1, 2);
                var level = displaytext.Substring(section_Index + 2, level_Index - 1 - section_Index);
                var position = displaytext.Substring(colon_Index + 1);

                var existing = _unitOfWork.Ancestral.Get(u => u.PositionId == displaytext);
                if (existing != null) return BadRequest(new { success = false, message = "[新選取]的塔位已被使用,無法被變更!!" });

                var target = _unitOfWork.Ancestral.Get(u => u.AncestralPositionId == dto.SelectedAncestralPositionId);
                if (target == null) return NotFound(new { success = false, message = "選取的牌位不存在" });

                target.PositionId = displaytext;
                target.Side = side;
                target.Section = section;
                target.Level = level;
                target.Position = position;
                _unitOfWork.Ancestral.Update(target);
                _unitOfWork.Save();
                return Ok(new { success = true });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "SavePosition error");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
