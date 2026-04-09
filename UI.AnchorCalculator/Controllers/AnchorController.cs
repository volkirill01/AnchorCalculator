using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.Services;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.Controllers;

public class AnchorController : Controller
{
	private readonly AnchorService m_AnchorService;
	private readonly SvgMakingService m_SvgService;
	private readonly CalculateService m_CalculateService;
	private readonly UserManager<User> m_UserManager;

	public AnchorController(AnchorService anchorService, SvgMakingService svgService, CalculateService calculateService, UserManager<User> userManager)
	{
		m_AnchorService = anchorService;
		m_SvgService = svgService;
		m_CalculateService = calculateService;
		m_UserManager = userManager;
	}

	[HttpGet] // AnchorController
	public async Task<ActionResult> Index()
	{
		AnchorViewModel viewModel = await m_AnchorService.GetAnchorViewModel();
		return View(viewModel);
	}

	[HttpGet] // AnchorController
	public async Task<ActionResult> Anchors(string? SelectedMaterial, string SelectedUserName, DateTime DateTimeFrom, DateTime DateTimeTill, double PriceFrom, double PriceTill, int PageSize = 6, int page = 1)
	{
		IQueryable<Anchor> anchors = m_AnchorService.GetAll();
		m_AnchorService.Filter(ref anchors, SelectedMaterial, SelectedUserName, DateTimeFrom, DateTimeTill, PriceFrom, PriceTill);
		PagingData pagingData = m_AnchorService.Pagination(ref anchors, PageSize, page);
		AnchorsViewModel anchorsViewModel = await m_AnchorService.GetAnchorsViewModel(anchors, SelectedMaterial, SelectedUserName, DateTimeFrom, DateTimeTill, PriceFrom, PriceTill, pagingData);
		return View(anchorsViewModel);
	}

	[HttpGet] // AnchorController
	public async Task<JsonResult> GetListAnchorJsonResult(string ids)
	{
		List<Anchor> anchors = await m_AnchorService.GetListAnchorFromPage(ids);
		var anchorsSvg = anchors.Select(e => e.SvgElement).ToList();
		var anchorsId = anchors.Select(e => e.Id).ToList();
		if (anchorsSvg.Count>0)
			return Json(new { success = true, anchorsSvg, idMin = anchorsId[0], idMax = anchorsId[^1]});
		else
			return Json(new { success = false });
	}

	[HttpGet] // AnchorController
	public async Task<JsonResult> GetAnchorJsonResult(int id)
	{
		Anchor anchor = await m_AnchorService.GetAnchorById(id);
		if (anchor != null)
			return Json(new { success = true, anchor });
		else
			return Json(new { success = false });
	}

	[HttpPost] // AnchorController
	[AllowAnonymous]
	public async Task<JsonResult> GetAnchorJsonResult(AnchorViewModel viewModel)
	{
		double minBendLength = 60 + viewModel.BendRadiusMillimeters;
		if (double.TryParse(viewModel.DiameterMillimeters, out double diameterParse))
			minBendLength += diameterParse;

		if (!viewModel.HasThread)
		{
			ModelState.Remove(nameof(viewModel.ThreadDiameterMillimeters));
			ModelState.Remove(nameof(viewModel.ThreadStepMillimeters));
			ModelState.Remove(nameof(viewModel.ThreadLengthMillimeters));
		}
		else
		{
			if (viewModel.ThreadDiameterMillimeters > diameterParse)
				ModelState.AddModelError(nameof(viewModel.ThreadDiameterMillimeters), "Диаметр резьбы должен быть меньше или равен диаметру анкера");

			if (viewModel.ThreadDiameterMillimeters < 6)
				ModelState.AddModelError(nameof(viewModel.ThreadDiameterMillimeters), "Диаметр резьбы не может быть меньше 6");
		}

		if (!viewModel.HasSecondThread)
			ModelState.Remove(nameof(viewModel.ThreadSecondLengthMillimeters));

		if (viewModel.Kind == AnchorKind.DoubleBend.ToString())
		{
			minBendLength += viewModel.BendRadiusMillimeters;
			minBendLength += diameterParse;
		}

		if (!(viewModel.Kind == AnchorKind.Straight.ToString()))
		{
			if (viewModel.BendLengthMillimeters < minBendLength)
				ModelState.AddModelError(nameof(viewModel.BendLengthMillimeters), $"Длина загиба должна быть от {minBendLength}");

			if (viewModel.BendRadiusMillimeters < diameterParse || viewModel.BendRadiusMillimeters == 0)
				ModelState.AddModelError(nameof(viewModel.BendRadiusMillimeters), "Радиус гиба не может быть меньше диаметра анкера или равен 0");
		}
		else
			ModelState.Remove(nameof(viewModel.BendRadiusMillimeters));

		if (!viewModel.HasVariableLength)
		{
			ModelState.Remove(nameof(viewModel.SecondLengthMillimeters));

			if (viewModel.Kind == AnchorKind.DoubleBend.ToString())
				viewModel.SecondLengthMillimeters = viewModel.LengthMillimeters;
		}

		if (!ModelState.IsValid)
			return Json(new
			{
				success = false,
				errorMessageThreadDiam = ModelState["ThreadDiameterMillimeters"]?.Errors.FirstOrDefault()?.ErrorMessage,
				errorMessageBendLen = ModelState["BendLengthMillimeters"]?.Errors.FirstOrDefault()?.ErrorMessage,
				errorMessageThreadLen = ModelState["ThreadLengthMillimeters"]?.Errors.FirstOrDefault()?.ErrorMessage,
				errorMessageThreadSecondLen = ModelState["ThreadSecondLengthMillimeters"]?.Errors.FirstOrDefault()?.ErrorMessage,
				errorMessageLen = ModelState["LengthMillimeters"]?.Errors.FirstOrDefault()?.ErrorMessage,
				errorMessageRad = ModelState["BendRadiusMillimeters"]?.Errors.FirstOrDefault()?.ErrorMessage,
				errorMessageLenSecond = ModelState["SecondLengthMillimeters"]?.Errors.FirstOrDefault()?.ErrorMessage
			});

		Anchor Anchor = await m_AnchorService.GetAnchor(viewModel);
		if (Anchor.Kind == AnchorKind.Straight || Anchor.BendRadiusMillimeters == 0)
		{
			Anchor.BendRadiusMillimeters = 0;
			m_SvgService.GetSvgStraightAnchor(Anchor);
		}

		if (Anchor.Kind == AnchorKind.SingleBend)
			m_SvgService.GetSvgBendAnchor(Anchor);

		if (Anchor.Kind == AnchorKind.DoubleBend)
			m_SvgService.GetSvgBendDoubleAnchor(Anchor);

		await m_CalculateService.Calculate(Anchor);

		return Json(new { success = true, anchorJS = Anchor, isAuthen = User.Identity.IsAuthenticated });
	}

	[HttpGet] // AnchorController/Details/5
	public async Task<ActionResult> Details(int id)
	{
		if (id == 0)
			return NoContent();

		Anchor anchor = await m_AnchorService.GetAnchorById(id);
		AnchorViewModel viewModel = m_AnchorService.GetAnchorViewModelForDetails(anchor);
		return View(viewModel);
	}

	[HttpGet] // AnchorController/Create
	public ActionResult Create() => View();

	[HttpPost] // AnchorController/Add
	public async Task<ActionResult> Add(AnchorViewModel viewModel)
	{
		if (viewModel.ThreadLengthMillimeters == 0)
			ModelState.Remove(nameof(viewModel.ThreadLengthMillimeters));

		if (viewModel.ThreadSecondLengthMillimeters == 0)
			ModelState.Remove(nameof(viewModel.ThreadSecondLengthMillimeters));

		if (viewModel.Kind != AnchorKind.DoubleBend.ToString())
			ModelState.Remove(nameof(viewModel.SecondLengthMillimeters));

		if (!ModelState.IsValid)
			return Json(new { success = false });

		User user = await CurrentUser.Get(m_UserManager, User.Identity.Name);
		int id = await m_AnchorService.AddAnchor(viewModel, user.Id);
		return Json(new { success = true, id });
	}

	[HttpGet] // AnchorController/Edit/5
	public ActionResult Edit(int id) => View();

	[HttpPost] // AnchorController/Edit/5
	[ValidateAntiForgeryToken]
	public ActionResult Edit(int id, IFormCollection collection)
	{
		try
		{
			return RedirectToAction(nameof(Index));
		}
		catch
		{
			return View();
		}
	}

	[HttpPost] // AnchorController/Delete/5
	public ActionResult Delete(int id) => View();

	[HttpPost] // AnchorController/Delete/5
	public async Task<ActionResult> Delete(int id, IFormCollection collection)
	{
		try
		{
			await m_AnchorService.DeleteById(id);
			return Ok();
		}
		catch
		{
			return View();
		}
	}
}
