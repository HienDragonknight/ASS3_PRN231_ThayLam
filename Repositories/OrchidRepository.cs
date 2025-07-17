using Azure.Core;
using BusinessObjects.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrchidRepository
    {
        private readonly OrchildDAO _orchildDAO;

        public OrchidRepository(OrchildDAO orchildDAO)
        {
            _orchildDAO = orchildDAO;
        }

        public async Task<Orchid> CreateOrchidAsync(OrchidCreateDto dto)
        {
            var orchid = new Orchid
            {
               IsNatural = dto.IsNatural,
                OrchidDescription = dto.OrchidDescription,
                OrchidName = dto.OrchidName,
                OrchidUrl = dto.OrchidUrl,
                Price = dto.Price,
                CategoryId = dto.CategoryId
            };
            return await _orchildDAO.CreateOrchidAsync(orchid);
        }

        public async Task<bool> DeleteOrchidAsync(int orchidId)
        {
            using var context = new MyNewStoreDbContext();

            var orchid = await context.Orchids.FindAsync(orchidId);

            if (orchid == null) return false;

            // kiểm tra xem có order nào liên quan không
            var hasOrders = context.OrderDetails.Any(od => od.OrchidId == orchidId);
            if (hasOrders)
            {
                // trả false hoặc throw exception tùy mục đích
                throw new InvalidOperationException("Cannot delete: Orchid is referenced in order details.");
            }

            context.Orchids.Remove(orchid);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Orchid?> GetOrchidAsync(int orchid)
        {
            var o = await _orchildDAO.GetOrchidWithVirusesAsync(orchid);
            if (o == null)
                return null;

            return o;
           
        }
        public async Task<Orchid?> UpdateOrchidAsync(int id, OrchidUpdateDto dto)

        {
            var orchid = await _orchildDAO.GetByIdAsync(id); // hoặc context.Orchids.FindAsync(id);
            if (orchid == null)
                return null;

            // Chỉ cập nhật nếu có giá trị
            if (dto.IsNatural.HasValue)
                orchid.IsNatural = dto.IsNatural.Value;

            if (!string.IsNullOrEmpty(dto.OrchidDescription))
                orchid.OrchidDescription = dto.OrchidDescription;

            if (!string.IsNullOrEmpty(dto.OrchidName))
                orchid.OrchidName = dto.OrchidName;

            if (!string.IsNullOrEmpty(dto.OrchidUrl))
                orchid.OrchidUrl = dto.OrchidUrl;

            if (dto.Price.HasValue)
                orchid.Price = dto.Price.Value;

            if (dto.CategoryId.HasValue)
                orchid.CategoryId = dto.CategoryId.Value;

            return await _orchildDAO.UpdateOrchidAsync(orchid);
        }

        public async Task<List<Orchid>> GetAllOrchidsAsync()
        {
            var orchids = await _orchildDAO.GetAllOrchidsWithVirusesAsync();

            return orchids.ToList();
        }


        public async Task<bool> OrchidExistsAsync(int orchidId)
        {
            return await _orchildDAO.OrchidExistsAsync(orchidId);
        }


    }
}
