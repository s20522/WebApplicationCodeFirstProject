using Microsoft.AspNetCore.Mvc;
using WebApplicationCodeFirstProject.DTO;
using WebApplicationCodeFirstProject.Services;

namespace WebApplicationCodeFirstProject.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionControllers(IPrescriptionService prescriptionService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreatePrescription([FromBody] PrescriptionCreateDto prescriptionData)
    {
        var result = await prescriptionService.CreatePrescriptionAsync(prescriptionData);
        return result;
    }

    [HttpGet("{idPatient}")]
    public async Task<IActionResult> GetPatientDetails(int idPatient)
    {
        var result = await prescriptionService.GetPatientDetailsAsync(idPatient);
        return result;
    }
}