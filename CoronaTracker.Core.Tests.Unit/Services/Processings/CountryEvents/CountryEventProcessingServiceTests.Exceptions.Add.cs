﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System.Threading.Tasks;
using CoronaTracker.Core.Models.CountryEvents;
using CoronaTracker.Core.Models.Processings.CountryEvents;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xeptions;
using Xunit;

namespace CoronaTracker.Core.Tests.Unit.Services.Processings.CountryEvents
{
    public partial class CountryEventProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var someCountryEvent = CreateRandomCountryEvent();

            var expectedCountryEventProcessingDependencyException =
                new CountryEventProcessingDependencyException(
                    dependencyException.InnerException as Xeption);

            this.countryEventServiceMock.Setup(service =>
                service.AddCountryEventAsync(someCountryEvent))
                    .Throws(dependencyException);

            // when
            ValueTask<CountryEvent> addcountryEventTask =
                this.countryEventProcessingService.AddCountryEventAsync(someCountryEvent);
                
            // then
            await Assert.ThrowsAsync<CountryEventProcessingDependencyException>(() =>
                addcountryEventTask.AsTask());

            this.countryEventServiceMock.Verify(service =>
                service.AddCountryEventAsync(someCountryEvent),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCountryEventProcessingDependencyException))), 
                        Times.Once);

            this.countryEventServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        } 
    }
}
