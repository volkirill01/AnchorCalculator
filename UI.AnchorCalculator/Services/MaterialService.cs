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

	public async Task<MaterialsAndWorkCostViewModel> GetMaterialsAndWorkCostViewModel()
	{
		WorkCost workCost = new();
		MaterialsAndWorkCostViewModel materialsAndWorkCostViewModel = new()
		{
			Materials = await m_ApplicationDbContext.Materials.OrderBy(x => x.Name).ThenBy(x => x.Size).ToListAsync(),
		};

		try
		{
			materialsAndWorkCostViewModel.WorkCost = await workCost.GetWorkCost(m_Environment);
		}
		catch (Exception ex)
		{
			string exception = $"Error:{ex.Message}";
			m_Logger.LogDebug(exception);
			throw;
		}
		return materialsAndWorkCostViewModel;
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

	public async Task EditWorkCost(MaterialsAndWorkCostViewModel materialsAndWorkCostViewModel)
	{
		WorkCost workCost = new()
		{
			ExchangeDollar = materialsAndWorkCostViewModel.WorkCost.ExchangeDollar,
			PnrRollingThreadDollars = materialsAndWorkCostViewModel.WorkCost.PnrRollingThreadDollars,
			PnrBendingAnchorDollars = materialsAndWorkCostViewModel.WorkCost.PnrBendingAnchorDollars,
			PnrBandSawDollars = materialsAndWorkCostViewModel.WorkCost.PnrBandSawDollars,
			EffectiveLengthMillimeters = materialsAndWorkCostViewModel.WorkCost.EffectiveLengthMillimeters,
			BandSawPriceDollars = materialsAndWorkCostViewModel.WorkCost.BandSawPriceDollars,
			ThreadRollingSettingHours = materialsAndWorkCostViewModel.WorkCost.ThreadRollingSettingHours,
			BendHours = materialsAndWorkCostViewModel.WorkCost.BendHours,
			BendSettingHours = materialsAndWorkCostViewModel.WorkCost.BendSettingHours,
			MarkupPercent = materialsAndWorkCostViewModel.WorkCost.MarkupPercent,
			AdditionalMarkupPercent_DiameterMoreThan30 = materialsAndWorkCostViewModel.WorkCost.AdditionalMarkupPercent_DiameterMoreThan30,
			PnrMetalworkingAreaDollars = materialsAndWorkCostViewModel.WorkCost.PnrMetalworkingAreaDollars,
			PlashkaPriceDollars = materialsAndWorkCostViewModel.WorkCost.PlashkaPriceDollars,
			CutterPriceDollars = materialsAndWorkCostViewModel.WorkCost.CutterPriceDollars
		};
		await workCost.AddWorkCost(workCost, m_Environment);
	}
}
