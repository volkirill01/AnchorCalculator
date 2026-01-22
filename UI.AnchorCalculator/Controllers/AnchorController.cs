using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using GrapeCity.Documents.Svg;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.Services;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.Controllers
{
    public class AnchorController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly AnchorService _AService;
        private readonly SvgMakingService _SvgService;
        private readonly CalculateService _CService;
        private readonly UserManager<User> _userManager;

        public AnchorController(IWebHostEnvironment appEnvironment, AnchorService aService, SvgMakingService svgService, CalculateService cService, UserManager<User> userManager)
        {
            _appEnvironment = appEnvironment;
            _AService = aService;
            _SvgService = svgService;
            _CService = cService;
            _userManager = userManager;
        }

        // GET: AnchorController
        public async Task<ActionResult> Index()
        {
            AnchorViewModel viewModel = await _AService.GetAnchorViewModel();
            return View(viewModel);
        }

        // GET: AnchorController
        public async Task<ActionResult> Anchors(string? SelectedMaterial, string SelectedUserName
            , DateTime DateTimeFrom, DateTime DateTimeTill, double PriceFrom, double PriceTill, int PageSize = 6, int page = 1)
        {
            IQueryable<Anchor> anchors = _AService.GetAll(); 
            _AService.Filter(ref anchors, SelectedMaterial, SelectedUserName, DateTimeFrom, DateTimeTill, PriceFrom, PriceTill); // filter
            PagingData pagingData = _AService.Pagination(ref anchors, PageSize, page);
            AnchorsViewModel anchorsViewModel = await _AService.GetAnchorsViewModel(anchors, SelectedMaterial, SelectedUserName, DateTimeFrom
                , DateTimeTill, PriceFrom, PriceTill, pagingData);
            return View(anchorsViewModel);
        }

        // GET: AnchorController
        public async Task<JsonResult> GetListAnchorJsonResult(string ids)
        {
            List<Anchor> anchors = await _AService.GetListAnchorFromPage(ids);
            var anchorsSvg = anchors.Select(e => e.SvgElement).ToList();
            var anchorsId = anchors.Select(e => e.Id).ToList();
            if (anchorsSvg.Count>0)
                return Json(new { success = true, anchorsSvg, idMin = anchorsId[0], idMax = anchorsId[^1]});
            else
                return Json(new { success = false });
        }

        // GET: AnchorController
        [HttpGet]
        public async Task<JsonResult> GetAnchorJsonResult(int id)
        {
            Anchor anchor = await _AService.GetAnchorById(id);
            if (anchor != null)
                return Json(new { success = true, anchor });
            else
                return Json(new { success = false });
        }

        // GET: AnchorController
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetAnchorJsonResult(AnchorViewModel viewModel)
        {
            double minBendLength = 60 + viewModel.BendRadius;
            if (double.TryParse(viewModel.Diameter, out double diameterParse))
                minBendLength += diameterParse;
            if (!viewModel.HasThread)
            {
                ModelState.Remove(nameof(viewModel.ThreadDiameter));
                ModelState.Remove(nameof(viewModel.ThreadStep));
                ModelState.Remove(nameof(viewModel.ThreadLength));
            }
            else
            {
                if (viewModel.ThreadDiameter > diameterParse)
                    ModelState.AddModelError(nameof(viewModel.ThreadDiameter), "Диаметр резьбы должен быть меньше или равен диаметру анкера");
                if (viewModel.ThreadDiameter < 6)
                    ModelState.AddModelError(nameof(viewModel.ThreadDiameter), "Диаметр резьбы не может быть меньше 6");
            }   
            if (!viewModel.HasThreadSecond)
                ModelState.Remove(nameof(viewModel.ThreadLengthSecond));
            if (viewModel.Kind == Kind.BendDouble.ToString())
            {                       
                minBendLength += viewModel.BendRadius;
                minBendLength += diameterParse;
            }
            if (!(viewModel.Kind == Kind.Straight.ToString()))
            { 
                if(viewModel.BendLength < minBendLength)
                    ModelState.AddModelError(nameof(viewModel.BendLength), $"Длина загиба должна быть от {minBendLength}");
                if (viewModel.BendRadius < diameterParse || viewModel.BendRadius == 0)
                    ModelState.AddModelError(nameof(viewModel.BendRadius), "Радиус гиба не может быть меньше диаметра анкера или равен 0");
            }
            else
                ModelState.Remove(nameof(viewModel.BendRadius));
            if (!viewModel.HasVariableLength) {
                ModelState.Remove(nameof(viewModel.LengthSecond));
                if (viewModel.Kind == Kind.BendDouble.ToString())
                    viewModel.LengthSecond = viewModel.Length;
            }
            if (ModelState.IsValid)
            {
                Anchor Anchor = await _AService.GetAnchor(viewModel);
                if (Anchor.Kind == Kind.Straight || Anchor.BendRadius == 0)
                {
                    Anchor.BendRadius = 0;
                    _SvgService.GetSvgStraightAnchor(Anchor);
                }
                if (Anchor.Kind == Kind.Bend)
                    _SvgService.GetSvgBendAnchor(Anchor);
                if (Anchor.Kind == Kind.BendDouble)
                    _SvgService.GetSvgBendDoubleAnchor(Anchor);
                await _CService.Calculate(Anchor);
                if (User.Identity.IsAuthenticated)
                    return Json(new { success = true, anchorJS = Anchor, isAuthen = true });
                else
                    return Json(new { success = true, anchorJS = Anchor, isAuthen = false });
            }
            else
            {
                return Json(new { success = false
                    , errorMessageThreadDiam = ModelState["ThreadDiameter"]?.Errors.FirstOrDefault()?.ErrorMessage
                    , errorMessageBendLen = ModelState["BendLength"]?.Errors.FirstOrDefault()?.ErrorMessage
                    , errorMessageThreadLen = ModelState["ThreadLength"]?.Errors.FirstOrDefault()?.ErrorMessage
                    , errorMessageThreadSecondLen = ModelState["ThreadLengthSecond"]?.Errors.FirstOrDefault()?.ErrorMessage
                    , errorMessageLen = ModelState["Length"]?.Errors.FirstOrDefault()?.ErrorMessage
                    , errorMessageRad = ModelState["BendRadius"]?.Errors.FirstOrDefault()?.ErrorMessage                    
                    , errorMessageLenSecond = ModelState["LengthSecond"]?.Errors.FirstOrDefault()?.ErrorMessage
                });
            }    
        }

        // GET: AnchorController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id != 0)
            {
                Anchor anchor = await _AService.GetAnchorById(id);
                AnchorViewModel viewModel = _AService.GetAnchorViewModelForDetails(anchor);
                return View(viewModel);
            }
            else
                return NoContent();
        }

        // GET: AnchorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AnchorController/Add
        [HttpPost]
        public async Task<ActionResult> Add(AnchorViewModel viewModel)
        {
            if (viewModel.ThreadLength == 0)
                ModelState.Remove(nameof(viewModel.ThreadLength));
            if (viewModel.ThreadLengthSecond == 0)
                ModelState.Remove(nameof(viewModel.ThreadLengthSecond));
            if (viewModel.Kind != Kind.BendDouble.ToString())
                ModelState.Remove(nameof(viewModel.LengthSecond));
            if (ModelState.IsValid)
            {
                User user = await CurrentUser.Get(_userManager, User.Identity.Name);
                int id = await _AService.AddAnchor(viewModel, user.Id);   
                    return Json(new { success = true, id });
            }
            else
                return Json(new { success = false });           
        }

        // GET: AnchorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AnchorController/Edit/5
        [HttpPost]
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

        // GET: AnchorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AnchorController/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await _AService.DeleteById(id);
                return Ok();
            }
            catch
            {
                return View();
            }
        }
    }
}
