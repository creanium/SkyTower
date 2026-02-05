using SkyTower.Domain.MonitoredLocations;

namespace SkyTower.Domain.UnitTests.MonitoredLocations;

[TestFixture]
internal sealed class MonitoringPeriodTests
{
	[Test]
	public void Constructor_Succeeds_When_NullOrValid()
	{
		using (Assert.EnterMultipleScope())
		{
			MonitoringPeriod period = null!;
			Assert.DoesNotThrow(() => period = new MonitoringPeriod(null, null));

			Assert.That(period.NotBefore, Is.Null);
			Assert.That(period.NotAfter, Is.Null);
		}

		using (Assert.EnterMultipleScope())
		{
			var notAfter = DateOnly.FromDateTime(DateTime.UtcNow);

			MonitoringPeriod period = null!;
			Assert.DoesNotThrow(() => period = new MonitoringPeriod(null, notAfter));

			Assert.That(period.NotBefore, Is.Null);
			Assert.That(period.NotAfter, Is.EqualTo(notAfter));
		}

		using (Assert.EnterMultipleScope())
		{
			var notBefore = DateOnly.FromDateTime(DateTime.UtcNow);

			MonitoringPeriod period = null!;
			Assert.DoesNotThrow(() => period = new MonitoringPeriod(notBefore, null));

			Assert.That(period.NotBefore, Is.EqualTo(notBefore));
			Assert.That(period.NotAfter, Is.Null);
		}

		using (Assert.EnterMultipleScope())
		{
			var notBefore = DateOnly.FromDateTime(DateTime.UtcNow);
			var notAfter = notBefore.AddDays(1);

			MonitoringPeriod period = null!;
			Assert.DoesNotThrow(() => period = new MonitoringPeriod(notBefore, notAfter));

			Assert.That(period.NotBefore, Is.EqualTo(notBefore));
			Assert.That(period.NotAfter, Is.EqualTo(notAfter));
		}
	}

	[Test]
	public void Constructor_Should_ThrowArgumentOutOfRangeException_When_NotAfterIsBeforeOrEqualToNotBefore()
	{
		using (Assert.EnterMultipleScope())
		{
			var notBefore = DateOnly.FromDateTime(DateTime.UtcNow);

			var ex = Assert.Throws<ArgumentOutOfRangeException>(() => { _ = new MonitoringPeriod(notBefore, notBefore); });
			Assert.That(ex.ParamName, Is.EqualTo(nameof(MonitoringPeriod.NotAfter)));
			Assert.That(ex.Message, Does.Contain("NotAfter must be after NotBefore."));
		}

		using (Assert.EnterMultipleScope())
		{
			var notBefore = DateOnly.FromDateTime(DateTime.UtcNow);
			var notAfter = notBefore.AddDays(-1);

			var ex = Assert.Throws<ArgumentOutOfRangeException>(() => { _ = new MonitoringPeriod(notBefore, notAfter); });
			Assert.That(ex.ParamName, Is.EqualTo(nameof(MonitoringPeriod.NotAfter)));
			Assert.That(ex.Message, Does.Contain("NotAfter must be after NotBefore."));
		}
	}
	
	[Test]
	public void Deconstruct_Should_Return_Correct_Values()
	{
		var notBefore = DateOnly.FromDateTime(DateTime.UtcNow);
		var notAfter = notBefore.AddDays(5);

		var period = new MonitoringPeriod(notBefore, notAfter);

		var (deconstructedNotBefore, deconstructedNotAfter) = period;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(deconstructedNotBefore, Is.EqualTo(notBefore));
			Assert.That(deconstructedNotAfter, Is.EqualTo(notAfter));
		}
	}
}