using System.Text.Json;

namespace UI.AnchorCalculator.Extensions;

public class WorkPrice
{
	public double ExchangeDollar { get; set; } // Conversion from $ to som
	public double PnrRollingThreadDollars { get; set; }
	public double PnrBendingAnchorDollars { get; set; }
	public double PnrBandSawDollars { get; set; }
	public double EffectiveLengthMillimeters { get; set; }
	public double BandSawPriceDollars { get; set; }
	public double ThreadRollingSettingHours { get; set; }
	public double BendHours { get; set; }
	public double BendSettingHours { get; set; }
	public double MarkupPercent { get; set; }
	public double AdditionalMarkupPercent_DiameterMoreThan30 { get; set; }
	public double PnrMetalworkingAreaDollars { get; set; }
	public double PlashkaPriceDollars { get; set; }
	public double CutterPriceDollars { get; set; }

	public async Task AddWorkPrice(WorkPrice workPrice, IWebHostEnvironment appEnvironment)
	{
		string path = Path.Combine(appEnvironment.WebRootPath, "jsonsDataSeed", "workprice.json");
		string json = JsonSerializer.Serialize<WorkPrice>(workPrice);

		using StreamWriter writer = new(path, false);
		await writer.WriteAsync(json);
	}

	public async Task<WorkPrice> GetWorkPrice(IWebHostEnvironment appEnvironment)
	{
		string path = Path.Combine(appEnvironment.WebRootPath, "jsonsDataSeed", "workprice.json");

		using FileStream fs = new(path, FileMode.OpenOrCreate);
		return await JsonSerializer.DeserializeAsync<WorkPrice>(fs);
	}
}
