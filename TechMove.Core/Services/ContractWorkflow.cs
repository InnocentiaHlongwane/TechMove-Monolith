using TechMove.Core.Models;

namespace TechMove.Core.Services;

/// <summary>
/// Contains workflow logic for contract state transitions and validations.
/// </summary>
public static class ContractWorkflow
{
    /// <summary>
    /// Determines if a service request can be created for a contract with the given status.
    /// </summary>
    public static bool CanCreateServiceRequest(ContractStatus status)
    {
        return status is ContractStatus.Active or ContractStatus.Draft;
    }

    /// <summary>
    /// Determines if a contract can transition to the specified status.
    /// </summary>
    public static bool CanTransitionTo(ContractStatus currentStatus, ContractStatus targetStatus)
    {
        return (currentStatus, targetStatus) switch
        {
            (ContractStatus.Draft, ContractStatus.Active) => true,
            (ContractStatus.Active, ContractStatus.OnHold) => true,
            (ContractStatus.OnHold, ContractStatus.Active) => true,
            (ContractStatus.Active, ContractStatus.Expired) => true,
            _ => false
        };
    }
}
