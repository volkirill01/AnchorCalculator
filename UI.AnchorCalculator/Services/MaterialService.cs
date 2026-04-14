using Core.AnchorCalculator.Entities;
using DAL.AnchorCalculator;
using Microsoft.EntityFrameworkCore;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.Services;

public class MaterialService
{
	private readonly ApplicationDbContext m_ApplicationDbContext;
	private readonly IWebHostEnvironment m_Environment;
	private readonly LoggerManager m_Logger;

	public MaterialService(ApplicationDbContext applicationDbContext, IWebHostEnvironment environment, LoggerManager logger)
	{
		m_ApplicationDbContext = applicationDbContext;
		m_Environment = environment;
		m_Logger = logger;
	}

	public async Task AddMaterial(MaterialViewModel viewModel)
	{
		Material material = new()
		{
			Name = viewModel.Name,
			Size = viewModel.Size,
			TypeId = viewModel.TypeId,
			PricePerMeter = viewModel.PricePerMeter,
			DateUpdate = DateTime.UtcNow,
			ThreadRollingHours = viewModel.ThreadRollingHours,
			ThreadCuttingHours = viewModel.ThreadCuttingHours,
			BandSawHours = viewModel.BandSawHours,
			BandSawBladeCount = viewModel.BandSawBladeCount,
			PlashkaCount = viewModel.PlashkaCount,
			CutterCount = viewModel.CutterCount
		};
		await m_ApplicationDbContext.Materials.AddAsync(material);
		await m_ApplicationDbContext.SaveChangesAsync();
	}

	public async Task EditMaterial(MaterialViewModelForEdit viewModel)
	{
		Material material = m_ApplicationDbContext.Materials.Find(viewModel.Id);
		material.TypeId = viewModel.TypeId;
		material.Name = viewModel.Name;
		material.Size = viewModel.Size;
		material.PricePerMeter = viewModel.PricePerMeter;
		material.DateUpdate = DateTime.UtcNow;
		material.ThreadRollingHours = viewModel.TheradRollingHours;
		material.ThreadCuttingHours = viewModel.ThreadCuttingHours;
		material.BandSawHours = viewModel.BandSawHours;
		material.BandSawBladeCount = viewModel.BandSawBladeCount;
		material.PlashkaCount = viewModel.PlashkaCount;
		material.CutterCount = viewModel.CutterCount;

		m_ApplicationDbContext.Materials.Update(material);
		await m_ApplicationDbContext.SaveChangesAsync();
	}

	public MaterialViewModel GetMaterialViewModel()
	{
		MaterialViewModel materialViewModel = new()
		{
			Types = Enum.GetValues(typeof(Core.AnchorCalculator.Entities.Enums.Type)),
			Names = new string[]{ "Арматура", "Круг", "Катанка" }
		};
		return materialViewModel;
	}

	public async Task<MaterialsAndWorkPriceViewModel> GetMaterialsAndWorkPriceViewModel()
	{
		WorkPrice workPrice = new();
		MaterialsAndWorkPriceViewModel materialsAndWorkPriceViewModel = new()
		{
			Materials = await m_ApplicationDbContext.Materials.OrderBy(x => x.Name).ThenBy(x => x.Size).ToListAsync(),
		};

		try
		{
			materialsAndWorkPriceViewModel.WorkPrice = await workPrice.GetWorkPrice(m_Environment);
		}
		catch (Exception ex)
		{
			string exception = $"Error:{ex.Message}";
			m_Logger.LogDebug(exception);
			throw;
		}
		return materialsAndWorkPriceViewModel;
	}

	public async Task<List<Material>> GetAllMaterials() => await m_ApplicationDbContext.Materials.ToListAsync();

	public async Task<Material> GetMaterialById(int id) => await m_ApplicationDbContext.Materials.FindAsync(id);

	public MaterialViewModelForEdit GetMaterialViewModelForEdit(int id)
	{
		Material material = m_ApplicationDbContext.Materials.Find(id);
		MaterialViewModel materialViewModel = GetMaterialViewModel();
		MaterialViewModelForEdit viewModelForEdit = new()
		{
			Id = material.Id,
			Name = material.Name,
			Size = material.Size,
			TypeId = material.TypeId,
			PricePerMeter = material.PricePerMeter,
			Types = materialViewModel.Types,
			Names = materialViewModel.Names.Where(e => e != material.Name).ToArray(),
			Type = material.Type,
			TheradRollingHours = material.ThreadRollingHours,
			ThreadCuttingHours = material.ThreadCuttingHours,
			BandSawHours = material.BandSawHours,
			BandSawBladeCount = material.BandSawBladeCount,
			PlashkaCount = material.PlashkaCount,
			CutterCount = material.CutterCount,
		};
		return viewModelForEdit;
	}

	public async Task DeleteById(int id)
	{
		Material material = m_ApplicationDbContext.Materials.Find(id);
		m_ApplicationDbContext.Remove(material);
		await m_ApplicationDbContext.SaveChangesAsync();
	}

	public async Task EditWorkPrice(MaterialsAndWorkPriceViewModel materialsAndWorkPriceViewModel)
	{
		WorkPrice workPrice = new()
		{
			ExchangeDollar = materialsAndWorkPriceViewModel.WorkPrice.ExchangeDollar,
			PnrRollingThreadDollars = materialsAndWorkPriceViewModel.WorkPrice.PnrRollingThreadDollars,
			PnrBendingAnchorDollars = materialsAndWorkPriceViewModel.WorkPrice.PnrBendingAnchorDollars,
			PnrBandSawDollars = materialsAndWorkPriceViewModel.WorkPrice.PnrBandSawDollars,
			EffectiveLengthMillimeters = materialsAndWorkPriceViewModel.WorkPrice.EffectiveLengthMillimeters,
			BandSawPriceDollars = materialsAndWorkPriceViewModel.WorkPrice.BandSawPriceDollars,
			ThreadRollingSettingHours = materialsAndWorkPriceViewModel.WorkPrice.ThreadRollingSettingHours,
			BendHours = materialsAndWorkPriceViewModel.WorkPrice.BendHours,
			BendSettingHours = materialsAndWorkPriceViewModel.WorkPrice.BendSettingHours,
			MarkupPercent = materialsAndWorkPriceViewModel.WorkPrice.MarkupPercent,
			AdditionalMarkupPercent_DiameterMoreThan30 = materialsAndWorkPriceViewModel.WorkPrice.AdditionalMarkupPercent_DiameterMoreThan30,
			PnrMetalworkingAreaDollars = materialsAndWorkPriceViewModel.WorkPrice.PnrMetalworkingAreaDollars,
			PlashkaPriceDollars = materialsAndWorkPriceViewModel.WorkPrice.PlashkaPriceDollars,
			CutterPriceDollars = materialsAndWorkPriceViewModel.WorkPrice.CutterPriceDollars
		};
		await workPrice.AddWorkPrice(workPrice, m_Environment);
	}
}
