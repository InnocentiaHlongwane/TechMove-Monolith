using TechMove.Web.Models;
namespace TechMove.Web.Services;
public static class ContractWorkflow { public static bool CanCreateServiceRequest(ContractStatus status) => status is not ContractStatus.Expired and not ContractStatus.OnHold; }