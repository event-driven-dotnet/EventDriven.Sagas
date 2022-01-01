using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Repositories;

namespace EventDriven.Sagas.Tests.Fakes;

public class FakeSagaConfigRepository : ISagaConfigRepository
{
    public Task<SagaConfiguration> GetSagaConfigurationAsync(Guid id)
    {
        var steps = new Dictionary<int, SagaStep>
            {
                {   1,
                    new SagaStep
                    {
                        Sequence = 1,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStatePending",
                                Payload = "Pending",
                                ExpectedResult = "Pending"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateInitial",
                                Payload = "Initial",
                                ExpectedResult = "Initial"
                            }
                        }
                    }
                },
                {   2,
                    new SagaStep
                    {
                        Sequence = 2,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReserveCredit",
                                Payload = "Reserved",
                                ExpectedResult = "Reserved"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReleaseCredit",
                                Payload = "Available",
                                ExpectedResult = "Available"
                            }
                        }
                    }
                },
                {   3,
                    new SagaStep
                    {
                        Sequence = 3,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReserveInventory",
                                Payload = "Reserved",
                                ExpectedResult = "Reserved"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReleaseInventory",
                                Payload = "Available",
                                ExpectedResult = "Available"
                            }
                        }
                    }
                },
                {   4,
                    new SagaStep
                    {
                        Sequence = 4,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateCreated",
                                Payload = "Created",
                                ExpectedResult = "Created"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateInitial",
                                Payload = "Initial",
                                ExpectedResult = "Initial"
                            }
                        }
                    }
                },
            };
        var config = new SagaConfiguration();
        config.Steps = steps;
        return Task.FromResult(config);
    }

    public Task<SagaConfiguration> AddSagaConfigurationAsync(SagaConfiguration entity)
    {
        throw new NotImplementedException();
    }

    public Task<SagaConfiguration> UpdateSagaConfigurationAsync(SagaConfiguration entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> RemoveSagaConfigurationAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}