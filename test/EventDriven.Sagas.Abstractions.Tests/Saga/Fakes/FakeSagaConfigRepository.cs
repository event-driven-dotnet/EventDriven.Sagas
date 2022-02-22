using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public class FakeSagaConfigRepository : ISagaConfigRepository
{
    public Task<SagaConfiguration?> GetAsync(Guid id)
    {
        var steps = new List<SagaStep>
            {
                new SagaStep
                {
                    Sequence = 1,
                    Action = new SagaAction
                    {
                        Command = new FakeCommand
                        {
                            Name = "SetStatePending",
                            Result = "Pending",
                            ExpectedResult = "Pending"
                        },
                        ReverseOnFailure = true
                    },
                    CompensatingAction = new SagaAction
                    {
                        Command = new FakeCommand
                        {
                            Name = "SetStateInitial",
                            Result = "Initial",
                            ExpectedResult = "Initial"
                        }
                    }
                },
                new SagaStep
                {
                    Sequence = 2,
                    Action = new SagaAction
                    {
                        Command = new FakeCommand
                        {
                            Name = "ReserveCredit",
                            Result = "Reserved",
                            ExpectedResult = "Reserved"
                        },
                        ReverseOnFailure = true
                    },
                    CompensatingAction = new SagaAction
                    {
                        Command = new FakeCommand
                        {
                            Name = "ReleaseCredit",
                            Result = "Available",
                            ExpectedResult = "Available"
                        }
                    }
                },
                new SagaStep
                {
                    Sequence = 3,
                    Action = new SagaAction
                    {
                        Command = new FakeCommand
                        {
                            Name = "ReserveInventory",
                            Result = "Reserved",
                            ExpectedResult = "Reserved"
                        },
                        ReverseOnFailure = true
                    },
                    CompensatingAction = new SagaAction
                    {
                        Command = new FakeCommand
                        {
                            Name = "ReleaseInventory",
                            Result = "Available",
                            ExpectedResult = "Available"
                        }
                    }
                },
                new SagaStep
                {
                    Sequence = 4,
                    Action = new SagaAction
                    {
                        Command = new FakeCommand
                        {
                            Name = "SetStateCreated",
                            Result = "Created",
                            ExpectedResult = "Created"
                        },
                        ReverseOnFailure = true
                    },
                    CompensatingAction = new SagaAction
                    {
                        Command = new FakeCommand
                        {
                            Name = "SetStateInitial",
                            Result = "Initial",
                            ExpectedResult = "Initial"
                        }
                    }
                }
            };
        var config = new SagaConfiguration { Steps = steps };
        return Task.FromResult<SagaConfiguration?>(config);
    }

    public Task<SagaConfiguration?> AddAsync(SagaConfiguration entity)
    {
        throw new NotImplementedException();
    }

    public Task<SagaConfiguration?> UpdateAsync(SagaConfiguration entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> RemoveAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}