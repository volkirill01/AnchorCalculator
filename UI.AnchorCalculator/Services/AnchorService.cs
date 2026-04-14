using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using DAL.AnchorCalculator;
using Microsoft.AspNetCore.Identity;
using NLog;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.Services;

public class AnchorService
{
	private readonly ApplicationDbContext m_ApplicationDbContext;
	private readonly MaterialService m_MaterialService;
	private readonly UserManager<User> m_UserManager;
	private static Logger m_Logger = LogManager.GetCurrentClassLogger();
	private readonly LoggerManager m_LoggerManager;
	private readonly IWebHostEnvironment m_AppEnvironment;

	public AnchorService(ApplicationDbContext applicationDbContext, MaterialService materialService, UserManager<User> userManager, LoggerManager loggerManager, IWebHostEnvironment appEnvironment)
	{
		m_ApplicationDbContext = applicationDbContext;
		m_MaterialService = materialService;
		m_UserManager = userManager;
		m_LoggerManager = loggerManager;
		m_AppEnvironment = appEnvironment;
	}

	public async Task<AnchorViewModel> GetAnchorViewModel()
	{
		var materials = await m_MaterialService.GetAllMaterials();
		AnchorViewModel anchorViewModel = new(){ Materials = materials.OrderBy(x => x.Name).ThenBy(x => x.Size).ToList() };
		return anchorViewModel;
	}

	public AnchorViewModel GetAnchorViewModelForDetails(Anchor anchor)
	{
		AnchorViewModel anchorViewModel = new(){ Anchor = anchor };

		if(anchor.Material != null)
			anchorViewModel.MaterialName = anchor.Material.FullName;

		if (anchor.User != null)
			anchorViewModel.UserName = anchor.User.UserName;

		return anchorViewModel;
	}

	public async Task<Anchor> GetAnchor(AnchorViewModel viewModel)
	{
		m_Logger.Debug($"viewModel.ThreadStepMillimeters: {viewModel.ThreadStepMillimeters}; culture: {Thread.CurrentThread.CurrentCulture.DisplayName}");
		List<AnchorKind> kinds = Enum.GetValues(typeof(AnchorKind)).Cast<AnchorKind>().ToList();
		float threadStepMillimeters = float.Parse(viewModel.ThreadStepMillimeters, CultureInfo.InvariantCulture);
		float diameterMillimeters = float.Parse(viewModel.DiameterMillimeters, CultureInfo.InvariantCulture);
		ThreadProductionType threadProductionType = viewModel.HasCuttingThread ? ThreadProductionType.CuttingThreadOnLathe : ThreadProductionType.RollingThreadOnMechanicMachine;

		WorkCost workCost = new();
		try
		{
			workCost = await workCost.GetWorkCost(m_AppEnvironment);
		}
		catch (Exception ex)
		{
			string exception = $"Error:{ex.Message}";
			m_Logger.Debug(exception);
			throw;
		}

		Anchor anchor = new()
		{
			MaterialId = viewModel.MaterialId,
			DiameterMillimeters = diameterMillimeters,
			ThreadDiameterMillimeters = viewModel.ThreadDiameterMillimeters,
			LengthMillimeters = viewModel.LengthMillimeters,
			ThreadLengthMillimeters = viewModel.ThreadLengthMillimeters,
			BendLengthMillimeters = viewModel.BendLengthMillimeters,
			BendRadiusMillimeters = viewModel.BendRadiusMillimeters,
			ThreadStepMillimeters = threadStepMillimeters,
			Quantity = viewModel.Quantity,
			Material = await m_MaterialService.GetMaterialById(viewModel.MaterialId),
			KindId = (int)kinds.FirstOrDefault(e => e.ToString() == viewModel.Kind),
			ThreadSecondLengthMillimeters = viewModel.ThreadSecondLengthMillimeters,
			ThreadProductionType = threadProductionType,
			ProductionThreadHours = viewModel.ProductionThreadHours,
			ProductionBendHours = viewModel.ProductionBendHours,
			ProductionBandSawHours = viewModel.ProductionBandSawHours,
			FullLengthMeters = viewModel.FullLengthMeters,
			HasCuttingThread = viewModel.HasCuttingThread,
			WithoutBindThreadDiamMaterial = viewModel.WithoutBindThreadDiamMaterial
		};
		if (viewModel.Kind == AnchorKind.DoubleBend.ToString())
			anchor.SecondLengthMillimeters = viewModel.HasVariableLength ? viewModel.SecondLengthMillimeters : viewModel.LengthMillimeters;

		return anchor;
	}

	public async Task<Anchor> GetAnchorById(int id)
	{
		Anchor anchor = await m_ApplicationDbContext.Anchors.FindAsync(id);

		if (!String.IsNullOrEmpty(anchor.MaterialJson))
			anchor.Material = JsonSerializer.Deserialize<Material>(anchor.MaterialJson);

		if (!String.IsNullOrEmpty(anchor.UserJson))
			anchor.User = JsonSerializer.Deserialize<User>(anchor.UserJson);

		return anchor;
	}

	public IQueryable<Anchor> GetAll()
	{
		IQueryable<Anchor> anchors = m_ApplicationDbContext.Anchors.OrderBy(x => x.Id);
		foreach (Anchor? item in anchors)
		{
			if (!String.IsNullOrEmpty(item.MaterialJson))
				item.Material = JsonSerializer.Deserialize<Material>(item.MaterialJson);

			if (!String.IsNullOrEmpty(item.UserJson))
				item.User = JsonSerializer.Deserialize<User>(item.UserJson);
		}
		return anchors;
	}

	public async Task<List<Anchor>> GetListAnchorFromPage(string ids)
	{
		List<Anchor> anchors = new();
		if (!String.IsNullOrEmpty(ids))
		{
			string[] strIds = ids.Split(new char[] { ',' });
			int[] intIds = Array.ConvertAll(strIds, e => int.Parse(e));

			foreach (var item in intIds)
				anchors.Add(await GetAnchorById(item));
		}
		else
			anchors = GetAll().ToList();

		return anchors;
	}

	public void Filter(ref IQueryable<Anchor> anchors, string? SelectedMaterial, string SelectedUserName, DateTime DateTimeFrom, DateTime DateTimeTill, double PriceFrom, double PriceTill)
	{
		if (!String.IsNullOrEmpty(SelectedMaterial))
			anchors = anchors.Where(e => e.MaterialJson.Contains(SelectedMaterial));

		if (!String.IsNullOrEmpty(SelectedUserName))
			anchors = anchors.Where(e => e.User.UserName.Contains(SelectedUserName));

		if (DateTimeFrom > DateTime.MinValue && DateTimeFrom < DateTime.MaxValue && DateTimeTill <= DateTime.MinValue)
			anchors = anchors.Where(e => e.DateCreate >= DateTimeFrom);

		if (DateTimeTill > DateTime.MinValue && DateTimeTill < DateTime.MaxValue && DateTimeFrom <= DateTime.MinValue)
			anchors = anchors.Where(e => e.DateCreate <= DateTimeTill);

		if (DateTimeFrom > DateTime.MinValue && DateTimeFrom < DateTime.MaxValue && DateTimeTill > DateTime.MinValue && DateTimeTill < DateTime.MaxValue)
			anchors = anchors.Where(e => e.DateCreate >= DateTimeFrom && e.DateCreate <= DateTimeTill);

		if (PriceFrom > 0 && PriceFrom < Double.PositiveInfinity && PriceTill == 0)
			anchors = anchors.Where(e => e.Price >= PriceFrom);

		if (PriceTill > 0 && PriceTill < Double.PositiveInfinity && PriceFrom == 0)
			anchors = anchors.Where(e => e.Price <= PriceTill);

		if (PriceFrom > 0 && PriceFrom < Double.PositiveInfinity && PriceTill > 0 && PriceTill < Double.PositiveInfinity)
			anchors = anchors.Where(e => e.Price >= PriceFrom && e.Price <= PriceTill);
	}

	public PagingData Pagination(ref IQueryable<Anchor> anchors, int pageSize, int page)
	{
		var countAllAnchors = anchors.Count();
		anchors = anchors.Skip((page - 1) * pageSize).Take(pageSize);

		PagingData pagingData = new(page, countAllAnchors, pageSize);
		return pagingData;
	}

	public async Task<AnchorsViewModel> GetAnchorsViewModel(IQueryable<Anchor> anchors, string? SelectedMaterial
		, string SelectedUserName, DateTime DateTimeFrom, DateTime DateTimeTill, double PriceFrom, double PriceTill, PagingData pagingData)
	{
		AnchorsViewModel anchorsViewModel = new()
		{
			Anchors = anchors.ToList(),
			FilterView = new FilterViewModelAnchors(await m_MaterialService.GetAllMaterials(), SelectedMaterial, SelectedUserName, DateTimeFrom, DateTimeTill, PriceFrom, PriceTill),
			PageViewModelAnchors = new PageViewModelAnchors(pagingData.Count, pagingData.Page, pagingData.PageSize)
		};
		return anchorsViewModel;
	}

	public async Task<int> AddAnchor(AnchorViewModel viewModel, string userId)
	{
		var options = new JsonSerializerOptions
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};
		User user = await m_UserManager.FindByIdAsync(userId);
		Material material = await m_MaterialService.GetMaterialById(viewModel.MaterialId);
		string materialJson = JsonSerializer.Serialize<Material>(material, options);
		string userJson = JsonSerializer.Serialize<User>(user, options);

		try
		{
			Anchor anchor = new()
			{
				LengthMillimeters = viewModel.LengthMillimeters,
				DiameterMillimeters = float.Parse(viewModel.DiameterMillimeters, CultureInfo.InvariantCulture),
				WeightKg = double.Parse(viewModel.WeightKg, CultureInfo.InvariantCulture),
				Price = double.Parse(viewModel.Price, CultureInfo.InvariantCulture),
				BendLengthMillimeters = viewModel.BendLengthMillimeters,
				BendRadiusMillimeters = viewModel.BendRadiusMillimeters,
				ThreadLengthMillimeters = viewModel.ThreadLengthMillimeters,
				ThreadSecondLengthMillimeters = viewModel.ThreadSecondLengthMillimeters,
				ThreadDiameterMillimeters = viewModel.ThreadDiameterMillimeters,
				ThreadStepMillimeters = float.Parse(viewModel.ThreadStepMillimeters, CultureInfo.InvariantCulture),
				TotalCost = double.Parse(viewModel.TotalCost, CultureInfo.InvariantCulture),
				Quantity = viewModel.Quantity,
				DateCreate = DateTime.Now,
				SvgElement = viewModel.SvgElement,
				BatchWeightKg = double.Parse(viewModel.BatchWeightKg, CultureInfo.InvariantCulture),
				BilletLengthMillimeters = double.Parse(viewModel.BilletLengthMillimeters, CultureInfo.InvariantCulture),
				MaterialId = viewModel.MaterialId,
				Sebes = double.Parse(viewModel.Sebes, CultureInfo.InvariantCulture),
				BatchSebes = double.Parse(viewModel.BatchSebes, CultureInfo.InvariantCulture),
				UserId = userId,
				UserJson = userJson,
				MaterialJson = materialJson,
				KindId = int.Parse(viewModel.Kind),
				BatchPriceMaterial = viewModel.BatchPriceMaterial,
				BatchPriceProductionThread = viewModel.BatchPriceProductionThread,
				BatchPriceProductionBend = viewModel.BatchPriceProductionBend,
				BatchPriceProductionBandSaw = viewModel.BatchPriceProductionBandSaw,
				RollerPathLengthMillimeters = viewModel.RollerPathLengthMillimeters,
				RollerPathLengthMillimetersBeforeEnd = viewModel.RollerPathLengthMillimetersBeforeEnd,
				ThreadProductionTypeId = viewModel.ThreadProductionTypeId,
				ProductionThreadHours = viewModel.ProductionThreadHours,
				ProductionBendHours = viewModel.ProductionBendHours,
				ProductionBandSawHours = viewModel.ProductionBandSawHours,
				FullLengthMeters = viewModel.FullLengthMeters,
				WithoutBindThreadDiamMaterial = viewModel.WithoutBindThreadDiamMaterial
			};
			await m_ApplicationDbContext.Anchors.AddAsync(anchor);
			await m_ApplicationDbContext.SaveChangesAsync();
			return anchor.Id;
		}
		catch (Exception ex)
		{
			string exception = $"Error:{ex.Message}";
			m_LoggerManager.LogDebug(exception);
			throw;
		}
	}

	public async Task DeleteById(int id)
	{
		Anchor anchor = m_ApplicationDbContext.Anchors.Find(id);
		m_ApplicationDbContext.Remove(anchor);
		await m_ApplicationDbContext.SaveChangesAsync();
	}
}
