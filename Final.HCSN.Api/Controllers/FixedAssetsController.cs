using Final.HCSN.Core.DTOs;
using Final.HCSN.Core.Entities;
using Final.HCSN.Core.Interface.Repository;
using Final.HCSN.Core.Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Final.HCSN.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixedAssetsController : ControllerBase
    {
        private readonly IFixedAssetRepo _fixedAssetRepo;
        private readonly IFixedAssetService _fixedAssetService;
        public FixedAssetsController(IFixedAssetRepo fixedAssetRepo, IFixedAssetService fixedAssetService)
        {
            _fixedAssetRepo = fixedAssetRepo;
            _fixedAssetService = fixedAssetService;
        }

        // GET: api/FixedAssets/newCode
        // Lấy mã tài sản mới khi thêm mới hoặc nhân bản
        [HttpGet("newCode")]
        public async Task<IActionResult> GetNewCode()
        {
            var newCode = await _fixedAssetService.GetNewFixedAssetCodeAsync();
            return Ok(newCode);
        }

        // GET: api/FixedAssets
        // Dùng để load dữ liệu lên grid
        [HttpGet]
        public async Task<IActionResult> GetAllFixedAssets()
        {
            var fixedAssets = await _fixedAssetRepo.GetFixedAssetGridAsync();
            return Ok(fixedAssets);
        }

        // GET: api/FixedAssets/filter
        [HttpGet("filter")]
        public async Task<IActionResult> GetFixedAssetsFilter([FromQuery] FixedAssetFilterDto filterDto)
        {
            try
            {
                var result = await _fixedAssetService.GetFixedAssetsPagingAsync(filterDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/FixedAssets/{id}
        // Dùng để blind dữ liệu lên form sửa
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _fixedAssetRepo.GetFixedAssetDetailByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFixedAsset([FromBody] FixedAssetCreateUpdateDto fixedAssetDto)
        {
            var result = await _fixedAssetService.CreateFixedAssetAsync(fixedAssetDto);
            if (result > 0)
            {
                return Ok(new { message = "Tạo thành công." });
            }
            return BadRequest(new { message = "Thất bại." });
        }

        // POST: api/FixedAssets/duplicate/{id}
        // Nhân bản tài sản mã tài sản là mã lớn nhất + 1
        [HttpPost("duplicate/{id}")]
        public async Task<IActionResult> DuplicateFixedAsset(string id)
        {
            try
            {
                var result = await _fixedAssetService.DuplicateFixedAssetAsync(id);

                if (result > 0)
                {
                    return StatusCode(201, new
                    {
                        message = "Nhân bản tài sản thành công.",
                        originalId = id
                    });
                }
                else
                {
                    return BadRequest(new { message = "Nhân bản thất bại." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/FixedAssets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFixedAsset(string id, [FromBody] FixedAssetCreateUpdateDto fixedAssetDto)
        {
            try
            {
                var result = await _fixedAssetService.UpdateFixedAssetAsync(id, fixedAssetDto);
                if (result > 0) return Ok(new { message = "Cập nhật thành công." });
                return BadRequest(new { message = "Cập nhật thất bại." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // XÓA 1 TÀI SẢN
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFixedAsset(string id)
        {
            var result = await _fixedAssetRepo.DeleteAsync(id);
            if (result > 0) return Ok(new { message = "Xóa thành công." });
            return BadRequest(new { message = "Xóa thất bại." });
        }

        // XÓA NHIỀU TÀI SẢN
        // Body gửi lên: ["id1", "id2", "id3"]
        [HttpDelete("batch")]
        public async Task<IActionResult> DeleteManyFixedAssets([FromBody] List<string> ids)
        {
            var result = await _fixedAssetRepo.DeleteManyAsync(ids);
            if (result > 0) return Ok(new { message = $"Đã xóa {result} bản ghi." });
            return BadRequest(new { message = "Không xóa được bản ghi nào." });
        }
    }
}
