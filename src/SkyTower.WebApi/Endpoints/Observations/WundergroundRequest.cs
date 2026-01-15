using FastEndpoints;
using JetBrains.Annotations;

namespace SkyTower.WebApi.Endpoints.Observations;

[UsedImplicitly]
internal sealed class WundergroundRequest
{
	/// <summary>
	/// action [action=updateraw] -- always supply this parameter to indicate you are making a weather observation upload
	/// </summary>
	[QueryParam, BindFrom("action")]
	public string Action { get; set; }

	/// <summary>
	/// ID [ID as registered by wunderground.com]
	/// </summary>
	[QueryParam, BindFrom("ID")]
	public string Id { get; set; }

	/// <summary>
	/// PASSWORD [Station Key registered with this PWS ID, case sensitive]
	/// </summary>
	[QueryParam, BindFrom("PASSWORD")]
	public string Password { get; set; }

	/// <summary>
	/// dateutc - [YYYY-MM-DD HH:MM:SS (mysql format)] In Universal Coordinated Time (UTC) Not local time
	/// </summary>
	[QueryParam, BindFrom("dateutc")]
	public string DateUtc { get; set; }

	/// <summary>
	/// winddir - [0-360 instantaneous wind direction]
	/// </summary>
	[QueryParam, BindFrom("winddir")]
	public int WindDirectionDegrees { get; set; }

	/// <summary>
	/// windspeedmph - [mph instantaneous wind speed]
	/// </summary>
	[QueryParam, BindFrom("windspeedmph")]
	public double WindSpeedMph { get; set; }

	/// <summary>
	/// windgustmph - [mph current wind gust, using software specific time period]
	/// </summary>
	[QueryParam, BindFrom("windgustmph")]
	public double WindGustMph { get; set; }

	/// <summary>
	/// windgustdir - [0-360 using software specific time period]
	/// </summary>
	[QueryParam, BindFrom("windgustdir")]
	public int WindGustDirectionDegrees { get; set; }
	
	/// <summary>
	/// windspdmph_avg2m  - [mph 2 minute average wind speed mph]
	/// </summary>
	[QueryParam, BindFrom("windspdmph_avg2m")]
	public double WindSpeedTwoMinAvg { get; set; }

	/// <summary>
	/// winddir_avg2m - [0-360 2 minute average wind direction]
	/// </summary>
	[QueryParam, BindFrom("winddir_avg2m")]
	public int WindDirectionTwoMinAvgDegrees { get; set; }

	/// <summary>
	/// windgustmph_10m - [mph past 10 minutes wind gust mph ]
	/// </summary>
	[QueryParam, BindFrom("windgustmph_10m")]
	public double WindGustTenMinMph { get; set; }

	/// <summary>
	/// windgustdir_10m - [0-360 past 10 minutes wind gust direction]
	/// </summary>
	[QueryParam, BindFrom("windgustdir_10m")]
	public int WindGustTenMinDirectionDegrees { get; set; }

	/// <summary>
	/// humidity - [% outdoor humidity 0-100%]
	/// </summary>
	[QueryParam, BindFrom("humidity")]
	public int HumidityPercent { get; set; }

	/// <summary>
	/// dewptf- [F outdoor dewpoint F]
	/// </summary>
	[QueryParam, BindFrom("dewptf")]
	public double DewPointF { get; set; }

	/// <summary>
	/// tempf - [F outdoor temperature]
	/// </summary>
	/// <remarks>
	/// * for extra outdoor sensors use temp2f, temp3f, and so on
	/// </remarks>
	[QueryParam, BindFrom("tempf")]
	public double TemperatureF { get; set; }
	
	/// <summary>
	/// rainin - [rain inches over the past hour)] -- the accumulated rainfall in the past 60 min
	/// </summary>
	[QueryParam, BindFrom("rainin")]
	public double? RainInchesPastHour { get; set; }

	/// <summary>
	/// dailyrainin - [rain inches so far today in local time]
	/// </summary>
	[QueryParam, BindFrom("dailyrainin")]
	public double? DailyRainInches { get; set; }

	/// <summary>
	/// baromin - [barometric pressure inches]
	/// </summary>
	[QueryParam, BindFrom("baromin")]
	public double? BarometricPressureInches { get; set; }

	/// <summary>
	/// weather - [text] -- metar style (+RA)
	/// </summary>
	[QueryParam, BindFrom("weather")]
	public string? MetarWeather { get; set; }

	/// <summary>
	/// clouds - [text] -- SKC, FEW, SCT, BKN, OVC
	/// </summary>
	[QueryParam, BindFrom("clouds")]
	public string? CloudCover { get; set; }

	/// <summary>
	/// soiltempf - [F soil temperature]
	/// </summary>
	/// <remarks>
	/// * for sensors 2,3,4 use soiltemp2f, soiltemp3f, and soiltemp4f
	/// </remarks>
	[QueryParam, BindFrom("soiltempf")]
	public double SoilTemperatureF { get; set; }
	
	/// <summary>
	/// soilmoisture - [%]
	/// </summary>
	/// <remarks>
	/// * for sensors 2,3,4 use soilmoisture2, soilmoisture3, and soilmoisture4
	/// </remarks>
	[QueryParam, BindFrom("soilmoisture")]
	public int SoilMoisturePercent { get; set; }

	/// <summary>
	/// leafwetness  - [%]
	/// </summary>
	/// <remarks>
	/// + for sensor 2 use leafwetness2
	/// </remarks>
	[QueryParam, BindFrom("leafwetness")]
	public int LeafWetnessPercent { get; set; }

	/// <summary>
	/// solarradiation - [W/m^2]
	/// </summary>
	[QueryParam, BindFrom("solarradiation")]
	public double SolarRadiation { get; set; }

	/// <summary>
	/// UV - [index]
	/// </summary>
	[QueryParam, BindFrom("UV")]
	public double UvIndex { get; set; }

	/// <summary>
	/// visibility - [nm visibility]
	/// </summary>
	[QueryParam, BindFrom("visibility")]
	public double VisibilityNauticalMiles { get; set; }

	/// <summary>
	/// indoortempf - [F indoor temperature F]
	/// </summary>
	[QueryParam, BindFrom("indoortempf")]
	public double IndoorTemperatureF { get; set; }

	/// <summary>
	/// indoorhumidity - [% indoor humidity 0-100]
	/// </summary>
	[QueryParam, BindFrom("indoorhumidity")]
	public int IndoorHumidityPercent { get; set; }
}