using Bogus;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleA2A.Plugins;

public class PolicyInfoTools
{
    [KernelFunction]
    public static PolicyCustomer GetPolicyInfo(string policyId)
    {
        var policyCustomerFaker = new Faker<PolicyCustomer>()
            .RuleFor(e => e.PolicyId, policyId)
            .RuleFor(e => e.CustomerId, f => f.Random.Guid().ToString())
            .RuleFor(e => e.CustomerName, f => f.Person.FullName)
            .RuleFor(e => e.CustomerAddress, f => f.Address.FullAddress())
            .Generate(1);
        return policyCustomerFaker.First();
        
    }
}
