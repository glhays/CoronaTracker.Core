﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CoronaTracker.Core.Models.CountryEvents;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Azure.ServiceBus;
using Moq;
using Xunit;

namespace CoronaTracker.Core.Tests.Unit.Services.Foundations.CountryEvents
{
    public partial class CountryEventServiceTests
    {
        [Fact]
        public async Task ShouldAddCountryEventAsync()
        {
            // given
            CountryEvent randomCountryEvent = CreateRandomCountryEvent();
            CountryEvent inputCountryEvent = randomCountryEvent;
            CountryEvent expectedCountryEvent = inputCountryEvent.DeepClone();
            
            string serializedCountryEvent =
                JsonSerializer.Serialize(expectedCountryEvent);

            var expectedCountryEventMessage = new Message
            {
                Body = Encoding.UTF8.GetBytes(serializedCountryEvent)
            };

            this.queueBrokerMock.Setup(broker => broker
                .EnqueueCountryMessageAsync(expectedCountryEventMessage));

            // when
            var actualCountryEvent =
                await this.countryEventService.AddCountryEventAsync(inputCountryEvent);

            // then
            actualCountryEvent.Should().BeEquivalentTo(expectedCountryEvent);

            this.queueBrokerMock.Verify(broker =>
                broker.EnqueueCountryMessageAsync(
                    It.Is(SameMessageAs(
                        expectedCountryEventMessage))),
                            Times.Once);

            this.queueBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}