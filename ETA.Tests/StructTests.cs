using NUnit.Framework;
using System;
using ETA.Shared;
using System.Threading.Tasks;
using System.Threading;
using Ploeh.AutoFixture;
using Moq;
using Should;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace ETA.Tests
{
	/// <summary>
	/// Tests the "struct" objects.
	/// </summary>
	[TestFixture]
	public class StructTests
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			// Use AutoFixture to generate values.
			this.fixture = new Fixture();

			// Create mocks for our interfaces.
			this.etaWebApiMock = new Mock<IEtaWebApi>();
			this.loggerMock = new Mock<ILogger>();

			// Register mocks as implementations for the interfaces in DI container.
			SimpleIoc.Default.Register<IEtaWebApi>(() => this.etaWebApiMock.Object);
			SimpleIoc.Default.Register<ILogger>(() => this.loggerMock.Object);
			SimpleIoc.Default.Register<EtaManager>();

			// Create EtaManager through DI container to inject interface implementations.
			this.etaManager = SimpleIoc.Default.GetInstance<EtaManager>();

			this.etaManager.Config = new EtaConfig("192.168.178.35", 8080);
		}

		EtaManager etaManager;
		IFixture fixture;
		Mock<IEtaWebApi> etaWebApiMock;
		Mock<ILogger> loggerMock;

		public IEtaWebApi WebApi => this.etaWebApiMock.Object;

		[Test]
		public void EtaError_constructor_should_set_properties()
		{
			var msg = this.fixture.Create<string>();
			var desc = this.fixture.Create<string>();
			var time = this.fixture.Create<DateTime>();

			var testError = new EtaError(msg, desc, time);

			testError.Message.ShouldEqual(msg);
			testError.Reason.ShouldEqual(desc);
			testError.OccurredAt.ShouldEqual(time);
		}

		[Test]
		public void EtaError_equality_should_work()
		{
			var msg = this.fixture.Create<string>();
			var desc = this.fixture.Create<string>();
			var time = this.fixture.Create<DateTime>();

			var testError = new EtaError(msg, desc, time);
			var compareError = new EtaError(msg, desc, time);

			testError.ShouldEqual(compareError);
		}

		[Test]
		public void EtaError_equality_should_fail()
		{
			var testError = this.fixture.Create<EtaError>();
			var compareError = this.fixture.Create<EtaError>();

			testError.ShouldNotEqual(compareError);
		}

		[Test]
		public void NumericUnit_constructor_should_set_properties()
		{
			var value = this.fixture.Create<double>();
			var unit = this.fixture.Create<string>();

			var testUnit = new NumericUnit(value, unit);

			testUnit.Value.ShouldEqual(value);
			testUnit.Unit.ShouldEqual(unit);
		}

		[Test]
		public void NumericUnit_equality_should_work()
		{
			var value = this.fixture.Create<double>();
			var unit = this.fixture.Create<string>();

			var testUnit = new NumericUnit(value, unit);
			var compareUnit = new NumericUnit(value, unit);

			testUnit.ShouldEqual(compareUnit);
		}

		[Test]
		public void NumericUnit_equality_should_fail()
		{
			var testUnit = this.fixture.Create<NumericUnit>();
			var compareUnit = this.fixture.Create<NumericUnit>();

			testUnit.ShouldNotEqual(compareUnit);
		}

		[Test]
		public void NumericUnit_implicit_cast_to_double_should_be_correct()
		{
			var testUnit = this.fixture.Create<NumericUnit>();

			double compare = testUnit;

			testUnit.Value.ShouldEqual(compare);
		}

		[Test]
		public void NumericUnit_explicit_cast_to_double_should_be_correct()
		{
			var testUnit = this.fixture.Create<NumericUnit>();

			var compare = (double)testUnit;

			testUnit.Value.ShouldEqual(compare);
		}

	}
}

