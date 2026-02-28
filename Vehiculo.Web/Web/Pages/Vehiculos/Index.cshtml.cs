using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguracion _configuracion;
        public IList<VehiculoResponse> vehiculos { get; set; } = default!; 

        public IndexModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task OnGet()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerTodos");
            var cliente = new HttpClient();
            var solcitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solcitud);
            respuesta.EnsureSuccessStatusCode();
           var resultado= await respuesta.Content.ReadAsStringAsync();
           var opciones = new JsonSerializerOptions
           {
               PropertyNameCaseInsensitive = true
           };
            vehiculos = JsonSerializer.Deserialize<List<VehiculoResponse>>(resultado, opciones);
        }
    }
}
