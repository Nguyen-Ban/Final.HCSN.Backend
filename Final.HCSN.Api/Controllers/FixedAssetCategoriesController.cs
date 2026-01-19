using Final.HCSN.Core.Interface.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Final.HCSN.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixedAssetCategoriesController : ControllerBase
    {
        private readonly IFixedAssetCategoryRepo _fixedAssetCategoryRepo;
        public FixedAssetCategoriesController(IFixedAssetCategoryRepo fixedAssetCategoryRepo)
        {
            _fixedAssetCategoryRepo = fixedAssetCategoryRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFixedAssetCategorys()
        {
            var fixedAssetCategorys = await _fixedAssetCategoryRepo.GetAllAsync();
            return Ok(fixedAssetCategorys);
        }
    }
}
