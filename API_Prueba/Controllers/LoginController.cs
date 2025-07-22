using ApiConsumidora.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiConsumidora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Paso 1: Obtener el token desde la API externa (login)
        /// </summary>
        [HttpPost("obtener-token")]
        public async Task<IActionResult> ObtenerToken([FromBody] LoginExternoRequest login)
        {
            string url = "https://para-ti.geniusqa.com.mx/api/v1/login/authenticate";

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(login),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync(url, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    return Ok(contenido); // el token
                }

                return StatusCode((int)response.StatusCode, "Error al obtener el token.");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error de conexión: {ex.Message}");
            }
        }

        /// <summary>
        /// Paso 2: Usar el token para autenticar sesión
        /// </summary>
        [HttpPost("autenticar-sesion")]
        public async Task<IActionResult> AutenticarSesion([FromBody] SesionRequest sesion, [FromHeader(Name = "Authorization")] string token)
        {
            string url = "https://para-ti.geniusqa.com.mx/api/v1/sesion/autenticar";

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(sesion),
                Encoding.UTF8,
                "application/json"
            );

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", token);
            request.Content = jsonContent;

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    return Ok(contenido);
                }

                return StatusCode((int)response.StatusCode, "Error desde la API externa.");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error de conexión: {ex.Message}");
            }
        }

        /// <summary>
        /// Paso 3: Obtener empleados desde la API externa usando el token
        /// </summary>
        [HttpGet("empleados")]
        public async Task<IActionResult> GetEmpleados([FromHeader(Name = "Authorization")] string token)
        {
            string url = "https://para-ti.geniusqa.com.mx/api/v1/entidades/GetListaEmpleados";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", token);

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    return Ok(contenido);
                }

                return StatusCode((int)response.StatusCode, "Error al obtener empleados.");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error de conexión: {ex.Message}");
            }
        }
    }
}
