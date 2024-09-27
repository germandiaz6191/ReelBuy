using Microsoft.AspNetCore.Components;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Frontend.Pages.Countries;

public partial class CountriesIndex
{
	[Inject] private IRepository Repository { get; set; } = null!;

	private List<Country>? Countries { get; set; }

	protected override async Task OnInitializedAsync()
	{
		var responseHppt = await Repository.GetAsync<List<Country>>("api/countries");
		Countries = responseHppt.Response;
	}
}