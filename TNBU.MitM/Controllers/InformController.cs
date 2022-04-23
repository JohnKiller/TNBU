using Microsoft.AspNetCore.Mvc;
using TNBU.Core.Models;
using TNBU.MitM.Services;

namespace TNBU.MitM.Controllers;

public class InformController : Controller {
	private readonly ILogger<InformController> logger;
	private readonly DeviceManagerService deviceManagerService;

	public InformController(ILogger<InformController> _logger, DeviceManagerService _dms) {
		logger = _logger;
		deviceManagerService = _dms;
	}

	[HttpPost("/inform")]
	public async Task<IActionResult> Index() {
		logger.LogInformation("Received Inform request from {ip}", Request.HttpContext.Connection.RemoteIpAddress?.ToString());
		using var data = new MemoryStream();
		await Request.Body.CopyToAsync(data);
		data.Position = 0;
		var req = InformPacket.Decode(data);
		var resp = await deviceManagerService.GotInform(req);
		return File(resp, "application/x-binary");
	}
}
