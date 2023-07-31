// using Microsoft.AspNetCore.Mvc;
// using esp32_client.Models;
// using esp32_client.Services;
// using Newtonsoft.Json;
// using esp32_client.Builder;
// using AutoMapper;

// namespace esp32_client.Controllers;

// [CustomAuthenticationFilter]
// public class PatternController : BaseController
// {
//     // private readonly IPatternService _patternService;
//     private readonly IUserAccountService _userAccountService;
//     private readonly IMapper _mapper;

//     public PatternController(LinqToDb linq2db, IUserAccountService userAccountService, IMapper mapper)
//     {
//         _linq2db = linq2db;
//         // _patternService = patternService;
//         _userAccountService = userAccountService;
//         _mapper = mapper;
//     }

//     public async Task<IActionResult> Index()
//     {

//         await Task.CompletedTask;
//         return View(new PatternIndexPageModel());
//     }

//     [HttpPost]
//     public async Task<IActionResult> Add(PatternIndexPageModel model)
//     {
//         return await HandleActionAsync(async () =>
//         {
//             // Handle if there is no file name
//             model.PatternCreateModel.FileName = string.IsNullOrEmpty(model.PatternCreateModel.FileName) ? model.PatternCreateModel.File.FileName : model.PatternCreateModel.FileName;

//             // await _patternService.Create(model.PatternCreateModel);
//         }, RedirectToAction("Index"));
//     }


//     [HttpPost]
//     public async Task<IActionResult> Delete(PatternIndexPageModel model)
//     {
//         return await HandleActionAsync(async () =>
//         {
//             var listId = model.ListDeletePatternById.Where(s => s.IsSelected == true).Select(s => s.Id).ToList();
//             // await _patternService.Delete(listId);
//         }, RedirectToAction("Index"));
//     }


//     [HttpGet]
//     [ProducesResponseType(StatusCodes.Status201Created)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public async Task<IActionResult> Download(int id)
//     {
//         // var pattern = await _patternService.GetById(id);
//         if (pattern is null) return Ok();

//         var format = pattern.FileName.Split('.').LastOrDefault() ?? "";

//         return File(Convert.FromBase64String(pattern.FileData), Utils.Utils.GetContentType(format), fileDownloadName: pattern.FileName);
//     }

//     public async Task<IActionResult> Update(int id)
//     {
//         var pattern = await _patternService.GetById(id);
//         var productUpdateModel = _mapper.Map<PatternUpdateModel>(pattern);
//         return View(productUpdateModel);
//     }

//     [HttpPost]
//     public async Task<IActionResult> Update(PatternUpdateModel model)
//     {
//         return await HandleActionAsync(async () =>
//         {
//             var product = await _patternService.Update(model);
//         }, RedirectToAction("Index"));
//     }

// }
