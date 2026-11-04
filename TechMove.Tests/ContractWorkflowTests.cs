using TechMove.Core.Models;
using TechMove.Core.Services;
using Xunit;

namespace TechMove.Tests;

public class ContractWorkflowTests
{
    [Theory]
    [InlineData(ContractStatus.Expired, false)]
    [InlineData(ContractStatus.OnHold, false)]
    [InlineData(ContractStatus.Active, true)]
    [InlineData(ContractStatus.Draft, true)]
    public void ServiceRequestEligibilityDependsOnContractStatus(ContractStatus status, bool expected)
    {
        Assert.Equal(expected, ContractWorkflow.CanCreateServiceRequest(status));
    }
}