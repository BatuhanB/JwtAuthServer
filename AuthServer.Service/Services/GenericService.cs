using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System.Linq.Expressions;

namespace AuthServer.Service.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto dto)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();
            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            return Response<TDto>.Success(newDto,200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var entities = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync()); 
            return Response<IEnumerable<TDto>>.Success(entities,200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var entity = ObjectMapper.Mapper.Map<TDto>(await _genericRepository.GetByIdAsync(id));
            if(entity == null)
            {
                return Response<TDto>.Fail(404,"Id not found!",true);
            }
            return Response<TDto>.Success(entity,200);
        }

        public async Task<Response<NoContentDto>> Remove(int id)
        {
            var isExists = await _genericRepository.GetByIdAsync(id);
            if(isExists == null)
            {
                return Response<NoContentDto>.Fail(404,"Id not found!",true);
            }
            _genericRepository.Remove(id);
            await _unitOfWork.CommitAsync();
            return Response<NoContentDto>.Success(204);
            
        }

        public async Task<Response<NoContentDto>> Update(TDto dto,int id)
        {
            var isExists = await _genericRepository.GetByIdAsync(id);
            if(isExists == null)
            {
                return Response<NoContentDto>.Fail(404,"Id not found!",true);
            }
            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            _genericRepository.Update(updateEntity);
            await _unitOfWork.CommitAsync();
            return Response<NoContentDto>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _genericRepository.Where(predicate);
            var data = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync());
            return Response<IEnumerable<TDto>>.Success(data,200);
        }
    }
}
