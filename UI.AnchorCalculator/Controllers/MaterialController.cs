using Core.AnchorCalculator.Entities;
using Microsoft.AspNetCore.Mvc;
using UI.AnchorCalculator.Services;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.Controllers;

public class MaterialController : Controller
{
	private readonly MaterialService m_MaterialService;

	public MaterialController(MaterialService materialService)
	{
		m_MaterialService = materialService;
	}

	[HttpGet] // MaterialController
	public async Task<ActionResult> Index()
	{
		MaterialsAndWorkPriceViewModel materialsAndPrice = await m_MaterialService.GetMaterialsAndWorkPriceViewModel();
		return View(materialsAndPrice);
	}

	[HttpGet] // MaterialController/Details/5
	public ActionResult Details(int id) => View();

	[HttpGet] // MaterialController/Add
	public ActionResult Add()
	{
		MaterialViewModel materialViewModel = m_MaterialService.GetMaterialViewModel();
		return View(materialViewModel);
	}

	[HttpPost] // MaterialController/Add
	public async Task<ActionResult> Add(MaterialViewModel viewModel)
	{
		if (!ModelState.IsValid)
			return View(viewModel);

		await m_MaterialService.AddMaterial(viewModel);
		return RedirectToAction("Index");
	}

	[HttpGet] // MaterialController/Edit/5
	public ActionResult Edit(int id)
	{
		MaterialViewModelForEdit modelForEdit = m_MaterialService.GetMaterialViewModelForEdit(id);
		return View(modelForEdit);
	}

	[HttpPost] // MaterialController/Edit
	[ValidateAntiForgeryToken]
	public async Task<ActionResult> Edit(MaterialViewModelForEdit modelForEdit)
	{
		if (!ModelState.IsValid)
			return View(modelForEdit);

		await m_MaterialService.EditMaterial(modelForEdit);
		return RedirectToAction(nameof(Index));
	}

	[HttpPost] // MaterialController/Edit
	public async Task<JsonResult> EditWorkPrice(MaterialsAndWorkPriceViewModel materialsAndPrice)
	{
		if (!ModelState.IsValid)
			return Json(new { success = false });

		await m_MaterialService.EditWorkPrice(materialsAndPrice);
		return Json(new { success = true });
	}

	[HttpPost] // MaterialController/Delete/5
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			await m_MaterialService.DeleteById(id);
			return Json(new { success = true });
		}
		catch
		{
			return Json(new { success = false });
		}
	}

	[HttpGet]
	public async Task<JsonResult> GetMaterialJsonResult(int id)
	{
		Material material = new();
		if (id > 0)
			material = await m_MaterialService.GetMaterialById(id);
		else
			material.Size = 0;

		return Json(new { success = true, materialJS = material });
	}
}
