using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using System.Security.Claims;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class StoresController : GenericController<Store>
{
    private readonly IStoresUnitOfWork _storesUnitOfWork;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public StoresController(IGenericUnitOfWork<Store> unit, IStoresUnitOfWork storesUnitOfWork, IUsersUnitOfWork usersUnitOfWork) : base(unit)
    {
        _storesUnitOfWork = storesUnitOfWork;
        _usersUnitOfWork = usersUnitOfWork;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetStoresByUserAsync(string userId)
    {
        // Verificar si el usuario actual es el dueño o es administrador
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string currentUserId = currentUser.Id;
        
        // Si no es admin y no es el dueño, solo permitir ver tiendas propias
        if (!isAdmin && currentUserId != userId)
        {
            return Ok(new List<Store>()); // Devolver lista vacía en lugar de un error
        }
        
        var response = await _storesUnitOfWork.GetStoresByUserAsync(userId);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        // Verificar si el usuario es administrador
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;

        if (isAdmin)
        {
            // Administrador puede ver todas las tiendas
            return Ok(await _storesUnitOfWork.GetComboAsync());
        }
        else
        {
            // Vendedor solo puede ver sus propias tiendas
            var response = await _storesUnitOfWork.GetStoresByUserAsync(userId);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return Ok(new List<Store>());
        }
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        // Verificar si el usuario es administrador
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;
        
        if (isAdmin)
        {
            // Administrador puede ver todas las tiendas
            var response = await _storesUnitOfWork.GetAsync();
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        else
        {
            // Vendedor solo puede ver sus propias tiendas
            var response = await _storesUnitOfWork.GetStoresByUserAsync(userId);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _storesUnitOfWork.GetAsync(id);
        if (!response.WasSuccess || response.Result == null)
        {
            return NotFound(response.Message);
        }
        
        // Verificar si el usuario es administrador o dueño de la tienda
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;
        
        if (!isAdmin && response.Result.UserId != userId)
        {
            return Forbid("No tienes permiso para ver esta tienda");
        }
        
        return Ok(response.Result);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        // Verificar si el usuario es administrador
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;
        
        // Imprimir los valores para depuración
        Console.WriteLine($"Token UserId: {userId}");
        Console.WriteLine($"Es Admin: {isAdmin}");
        
        if (isAdmin)
        {
            // Administrador puede ver todas las tiendas paginadas
            var response = await _storesUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        else
        {
            // Para vendedores, solo mostrar sus propias tiendas
            // Modificamos la consulta para filtrar solo las tiendas del usuario
            Console.WriteLine($"Filtrando tiendas por UserId: {userId}");
            pagination.UserId = userId;
            var response = await _storesUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                // Imprimir IDs para depuración
                foreach (var store in response.Result)
                {
                    Console.WriteLine($"Tienda ID: {store.Id}, Tienda UserId: {store.UserId}, Comparación: {store.UserId == userId}");
                }
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        // Verificar si el usuario es administrador
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;
        if (!isAdmin)
        {
            // Para vendedores, solo contar sus propias tiendas
            pagination.UserId = userId;
        }
        
        var action = await _storesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync(StoreDTO storeDTO)
    {
        // Verificar que el usuario que crea la tienda sea el mismo que el del token o un admin
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;

        
        // Si no es admin, asegurarse que solo pueda crear tiendas para sí mismo
        if (!isAdmin && storeDTO.UserId != userId)
        {
            return Forbid("Solo puedes crear tiendas para tu propio usuario");
        }
        
        var action = await _storesUnitOfWork.AddAsync(storeDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutAsync(StoreDTO storeDTO)
    {
        // Verificar que el usuario que actualiza la tienda sea el dueño o un admin
        bool isAdmin = User.IsInRole("Admin");
         var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;
        
        // Obtener la tienda actual para verificar el dueño
        var storeResponse = await _storesUnitOfWork.GetAsync(storeDTO.Id);
        if (!storeResponse.WasSuccess || storeResponse.Result == null)
        {
            return NotFound("Tienda no encontrada");
        }
        
        // Si no es admin y no es el dueño, denegar acceso
        if (!isAdmin && storeResponse.Result.UserId != userId)
        {
            return Forbid("No tienes permiso para editar esta tienda");
        }
        
        var action = await _storesUnitOfWork.UpdateAsync(storeDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
    
    [HttpDelete("{id}")]
    public override async Task<IActionResult> DeleteAsync(int id)
    {
        // Verificar que el usuario que elimina la tienda sea el dueño o un admin
        bool isAdmin = User.IsInRole("Admin");
         var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;
        
        // Obtener la tienda actual para verificar el dueño
        var storeResponse = await _storesUnitOfWork.GetAsync(id);
        if (!storeResponse.WasSuccess || storeResponse.Result == null)
        {
            return NotFound("Tienda no encontrada");
        }
        
        // Si no es admin y no es el dueño, denegar acceso
        if (!isAdmin && storeResponse.Result.UserId != userId)
        {
            return Forbid("No tienes permiso para eliminar esta tienda");
        }
        
        var response = await base.DeleteAsync(id);
        if (response is NoContentResult)
        {
            return response;
        }
        return BadRequest();
    }
}