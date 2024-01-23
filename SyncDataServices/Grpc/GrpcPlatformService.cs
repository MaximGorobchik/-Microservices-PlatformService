using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        // Конструктор, який отримує репозиторій та мапер
        public GrpcPlatformService(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        // Метод для отримання всіх платформ
        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse(); // Створення відповіді
            var platforms = _repository.GetAllPlatforms(); // Отримання всіх платформ з репозиторію

            // Перетворення та додавання платформ до відповіді
            foreach (var plat in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));
            }
            // Повернення відповіді в форматі завданого типу (Task<PlatformResponse>)
            return Task.FromResult(response);
        }
    }
}