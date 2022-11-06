using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System.Linq.Expressions;

namespace AuthServer.Service.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;
        private readonly IMapper _mapper;
        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Response<TDto>> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _genericRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            var addedDto = _mapper.Map<TDto>(entity); 
            return Response<TDto>.Success(addedDto,200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var entities = _mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync()); 
            return Response<IEnumerable<TDto>>.Success(entities,200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var entity = _mapper.Map<TDto>(await _genericRepository.GetByIdAsync(id));
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
            _genericRepository.Remove(isExists);
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
            var updateEntity = _mapper.Map<TEntity>(dto);
            _genericRepository.Update(updateEntity);
            await _unitOfWork.CommitAsync();
            return Response<NoContentDto>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _genericRepository.Where(predicate);
            var data = _mapper.Map<IEnumerable<TDto>>(await list.ToListAsync());
            return Response<IEnumerable<TDto>>.Success(data,200);
        }
    }
}
