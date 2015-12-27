using NUnit.Framework;
using System;
using EtaShared;
using System.Threading.Tasks;
using System.Threading;
using Ploeh.AutoFixture;
using Moq;
using Should;

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
			try
			{
				// Use AutoFixture to generate values.
				this.fixture = new Fixture();

				// Create mocks for our interfaces.
				this.etaWebApiMock = new Mock<IEtaWebApi>();
				this.loggerMock = new Mock<ILogger>();
				this.storageMock = new Mock<IStorage>();
				this.supplyDataCreatorMock = new Mock<Func<ISupplyData>>();

				this.etaManager = new EtaManager(this.etaWebApiMock.Object, this.loggerMock.Object, storageMock.Object, supplyDataCreatorMock.Object);

				this.etaManager.Config = new EtaConfig("http://192.168.178.35:8080");
			}
			catch (Exception ex)
			{
				// Exception in Setup methods are not reported by Nunit and silently swallowed. Can be annoying. So let's log this.
				Console.WriteLine(ex);

			}
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
		}

		EtaManager etaManager;
		IFixture fixture;
		Mock<IEtaWebApi> etaWebApiMock;
		Mock<ILogger> loggerMock;
		Mock<IStorage> storageMock;
		Mock<Func<ISupplyData>> supplyDataCreatorMock;

		public IEtaWebApi WebApi => this.etaWebApiMock.Object;

		[Test]
		public void EtaError_constructor_should_set_properties()
		{
			var msg = this.fixture.Create<string>();
			var desc = this.fixture.Create<string>();
			var time = this.fixture.Create<DateTime>();
			var errorType = this.fixture.Create<EtaError.ERROR_TYPE>();

			var testError = new EtaError(msg, desc, time, errorType);

			testError.Message.ShouldEqual(msg);
			testError.Reason.ShouldEqual(desc);
			testError.OccurredAt.ShouldEqual(time);
			testError.ErrorType.ShouldEqual(errorType);
		}

		[Test]
		public void EtaError_equality_should_work()
		{
			var msg = this.fixture.Create<string>();
			var desc = this.fixture.Create<string>();
			var time = this.fixture.Create<DateTime>();
			var errorType = this.fixture.Create<EtaError.ERROR_TYPE>();

			var testError = new EtaError(msg, desc, time, errorType);
			var compareError = new EtaError(msg, desc, time, errorType);

			testError.ShouldEqual(compareError);
		}

		[Test]
		public void EtaError_equality_should_fail()
		{
			var testError = this.fixture.Create<EtaError>();
			var compareError = this.fixture.Create<EtaError>();

			// Chances are low that fixture will create two identical objects :-)
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

