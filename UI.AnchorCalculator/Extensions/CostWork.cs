using System.Text.Json;

namespace UI.AnchorCalculator.Extensions;

public class WorkCost
{
	public double ExchangeDollar { get; set; } // TODO: What? Som / $
	public double PnrRollingThreadDollars { get; set; }
	public double PnrBendingAnchorDollars { get; set; }
	public double PnrBandSawDollars { get; set; }
	public double EffectiveLengthMillimeters { get; set; }
	public double BandSawPriceDollars { get; set; }
	public double SetThreadRollingHours { get; set; }
	public double BendHours { get; set; }
	public double SetBendHours { get; set; }
	public double MarginPercent { get; set; }
	public double MarginFBPercent { get; set; } // Margin for anchors from diameter 30mm and more // TODO: Rename to more self explanatory name and remove comment
	public double AreaLockSmithDollars { get; set; }
	public double PlashkaPriceDollars { get; set; }
	public double CutterPriceDollars { get; set; }

	public async Task AddWorkCost(WorkCost workCost, IWebHostEnvironment appEnvironment)
	{
		string path = Path.Combine(appEnvironment.WebRootPath, "jsonsDataSeed", "costwork.json");
		string json = JsonSerializer.Serialize<WorkCost>(workCost);

		using StreamWriter writer = new(path, false);
		await writer.WriteAsync(json);
	}

	public async Task<WorkCost> GetWorkCost(IWebHostEnvironment appEnvironment)
	{
		string path = Path.Combine(appEnvironment.WebRootPath, "jsonsDataSeed", "costwork.json");
		WorkCost? workCost = new();

		using (FileStream fs = new(path, FileMode.OpenOrCreate))
		{
			workCost = await JsonSerializer.DeserializeAsync<WorkCost>(fs);
		}
		return workCost;
	}
}
