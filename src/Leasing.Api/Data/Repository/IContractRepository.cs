using Leasing.Api.Domain;

namespace Leasing.Api.Data.Repository;

public interface IContractRepository : IRepository<(int facilityCode, int equipmentCode), Contract>, IExistable<Contract>
{
}