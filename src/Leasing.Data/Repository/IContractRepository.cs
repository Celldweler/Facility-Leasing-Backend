using Leasing.Domain.Models;

namespace Leasing.Data.Repository;

public interface IContractRepository :
    IRepository<(int facilityCode, int equipmentCode), Contract>,
    IExistable<Contract>
{
}