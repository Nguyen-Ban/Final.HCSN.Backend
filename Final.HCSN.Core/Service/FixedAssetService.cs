using Final.HCSN.Core.DTOs;
using Final.HCSN.Core.Entities;
using Final.HCSN.Core.Interface.Repository;
using Final.HCSN.Core.Interface.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.HCSN.Core.Service
{
    public class FixedAssetService : IFixedAssetService
    {
        private readonly IFixedAssetRepo _fixedAssetRepo;
        private readonly IFixedAssetCategoryRepo _fixedAssetCategoryRepo;
        public FixedAssetService(IFixedAssetRepo fixedAssetRepo, IFixedAssetCategoryRepo fixedAssetCategoryRepo)
        {
            _fixedAssetRepo = fixedAssetRepo;
            _fixedAssetCategoryRepo = fixedAssetCategoryRepo;
        }

        private FixedAssetEntity MapDtoToEntity(FixedAssetCreateUpdateDto dto) => new FixedAssetEntity
        {
            fixed_asset_code = dto.fixed_asset_code,
            fixed_asset_name = dto.fixed_asset_name,
            department_id = Guid.Parse(dto.department_id),
            fixed_asset_category_id = Guid.Parse(dto.fixed_asset_category_id),
            purchase_date = dto.purchase_date,
            quantity = dto.quantity,
            cost = dto.cost,
            depreciation_rate = dto.depreciation_rate
        };
        public async Task<int> CreateFixedAssetAsync(FixedAssetCreateUpdateDto fixedAssetDto)
        {
            if (await _fixedAssetRepo.IsCodeExistsAsync(fixedAssetDto.fixed_asset_code))
            {
                throw new Exception("Mã tài sản đã tồn tại trong hệ thống.");
            }

            // Xử lý Tỷ lệ hao mòn (Depreciation Rate)
            // Nếu người dùng không nhập (null hoặc 0) -> Lấy từ Category
            if (fixedAssetDto.depreciation_rate == 0)
            {
                var category = await _fixedAssetCategoryRepo.GetByIdAsync(fixedAssetDto.fixed_asset_category_id);
                if (category != null)
                {
                    fixedAssetDto.depreciation_rate = category.depreciation_rate;
                }
                else
                {
                    // Trường hợp lỗi không tìm thấy loại tài sản (tùy business xử lý)
                    throw new Exception("Không tìm thấy Loại tài sản để lấy tỷ lệ hao mòn.");
                }
            }

            var entity = MapDtoToEntity(fixedAssetDto);

            entity.depreciation_value_year = (fixedAssetDto.cost * fixedAssetDto.depreciation_rate) / 100;

            // Mặc định năm theo dõi/sử dụng từ ngày mua
            entity.production_year = fixedAssetDto.purchase_date.Year;
            entity.tracked_year = fixedAssetDto.purchase_date.Year;

            return await _fixedAssetRepo.CreateAsync(entity);

        }

        public async Task<int> UpdateFixedAssetAsync(string id, FixedAssetCreateUpdateDto fixedAssetDto)
        {
            // 1. Kiểm tra tài sản có tồn tại không
            var oldEntity = await _fixedAssetRepo.GetByIdAsync(id);
            if (oldEntity == null)
            {
                throw new Exception("Tài sản không tồn tại.");
            }

            // 2. Kiểm tra trùng mã (Nếu người dùng sửa mã code khác với mã cũ)
            if (fixedAssetDto.fixed_asset_code != oldEntity.fixed_asset_code)
            {
                if (await _fixedAssetRepo.IsCodeExistsAsync(fixedAssetDto.fixed_asset_code))
                {
                    throw new Exception("Mã tài sản đã tồn tại.");
                }
            }

            // 3. Tính toán lại các giá trị (Hao mòn, v.v...)
            // (Logic tương tự Create)
            if (fixedAssetDto.depreciation_rate == 0)
            {
                var category = await _fixedAssetCategoryRepo.GetByIdAsync(fixedAssetDto.fixed_asset_category_id);
                if (category != null) fixedAssetDto.depreciation_rate = category.depreciation_rate;
            }

            // 4. Map sang Entity
            var updateEntity = MapDtoToEntity(fixedAssetDto);
            updateEntity.fixed_asset_id = Guid.Parse(id); // Giữ nguyên ID cũ

            // Cập nhật lại các trường tính toán
            updateEntity.depreciation_value_year = (fixedAssetDto.cost * fixedAssetDto.depreciation_rate) / 100;
            updateEntity.production_year = fixedAssetDto.production_year;
            updateEntity.tracked_year = fixedAssetDto.tracked_year;

            return await _fixedAssetRepo.UpdateAsync(updateEntity);
        }

        public async Task<PagingResult<FixedAssetDto>> GetFixedAssetsPagingAsync(FixedAssetFilterDto filter)
        {
            return await _fixedAssetRepo.GetFixedAssetsByFilterAsync(filter);
        }

        public async Task<string> GetNewFixedAssetCodeAsync()
        {
            return await _fixedAssetRepo.GetNewFixedAssetCodeAsync();
        }

        public async Task<int> DuplicateFixedAssetAsync(string sourceId)
        {
            var sourceAsset = await _fixedAssetRepo.GetByIdAsync(sourceId);
            if (sourceAsset == null)
            {
                throw new Exception("Tài sản nguồn không tồn tại.");
            }

            var newCode = await _fixedAssetRepo.GetNewFixedAssetCodeAsync();

            var newAsset = new FixedAssetEntity
            {
                fixed_asset_id = Guid.NewGuid(),
                fixed_asset_code = newCode,
                fixed_asset_name = sourceAsset.fixed_asset_name,
                department_id = sourceAsset.department_id,
                fixed_asset_category_id = sourceAsset.fixed_asset_category_id,
                purchase_date = sourceAsset.purchase_date,
                quantity = sourceAsset.quantity,
                cost = sourceAsset.cost,
                depreciation_rate = sourceAsset.depreciation_rate,
                depreciation_value_year = sourceAsset.depreciation_value_year,
                production_year = sourceAsset.production_year,
                tracked_year = sourceAsset.tracked_year
            };

            return await _fixedAssetRepo.CreateAsync(newAsset);
        }
    }
}
