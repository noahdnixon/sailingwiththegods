using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CityDetailsViewModel : CityViewModel
{
	public readonly ICollectionModel<CrewManagementMemberViewModel> Crew;
	public readonly ICollectionModel<CargoInventoryViewModel> Buy;
	public readonly ICollectionModel<CargoInventoryViewModel> Sell;

	class PriceInfo
	{
		public Resource Resource;
		public int Price;
		public int AvgPrice;
	}

	IEnumerable<PriceInfo> PriceInfos => City.cargo
				.Select(resource => new PriceInfo {
					Resource = resource,
					Price = Session.Trade.GetPriceOfResource(resource.name, City),
					AvgPrice = Session.Trade.GetAvgPriceOfResource(resource.name)
				})
				.ToArray();

	public CityDetailsViewModel(Settlement city, Action<CityViewModel> onClick) : base(city, onClick) {

		Crew = ValueModel.Wrap(new ObservableCollection<CrewManagementMemberViewModel>(
			Session.Network.CrewMembersWithNetwork(city, true)
				.OrderBy(c => Session.Network.GetCrewMemberNetwork(c).Count())
				.Take(5)
				.Select(crew => new CrewManagementMemberViewModel(crew, OnCrewClicked, OnCrewCityClicked))
		));

		Buy = ValueModel.Wrap(new ObservableCollection<CargoInventoryViewModel>(
			PriceInfos
				.OrderBy(o => o.Price - o.AvgPrice)
				.Take(5)
				.Select(o => new CargoInventoryViewModel(o.Resource))
		));

		Sell = ValueModel.Wrap(new ObservableCollection<CargoInventoryViewModel>(
			PriceInfos
				.OrderByDescending(o => o.Price - o.AvgPrice)
				.Take(5)
				.Select(o => new CargoInventoryViewModel(o.Resource))
		));

	}

	void OnCrewClicked(CrewManagementMemberViewModel crew) {

		// hide a previous details view if one was already showing so they don't stack on top of eachother and confuse the user
		Globals.UI.Hide<CrewDetailsScreen>();
		Globals.UI.Show<CrewDetailsScreen, CrewManagementMemberViewModel>(crew);

	}

	// TODO: Yikes. I copied this from DashboardViewModel
	public void OnCrewCityClicked(CityViewModel city) {
		Debug.Log("City clicked: " + city.PortName);

		if (Globals.UI.IsShown<CityView>()) {
			Globals.UI.Hide<CityView>();
		}

		var beacon = Globals.GameVars.crewBeacon;
		if (city.City != beacon.Target) {
			beacon.Target = city.City;
			Globals.Session.ActivateNavigatorBeacon(Globals.GameVars.crewBeacon, city.City.theGameObject.transform.position);
			Globals.Session.RotateCameraTowards(city.City.theGameObject.transform.position);
			Globals.UI.Show<CityView, CityViewModel>(new CityDetailsViewModel(city.City, null));
		}
		else {
			beacon.IsBeaconActive = false;
		}
	}
}

public class CityViewModel : Model
{
	protected GameVars GameVars => Globals.GameVars;
	protected GameSession Session => Globals.Session;
	protected Notifications Notifications => Globals.Notifications;
	protected MainState MainState => Globals.MainState;

	public Settlement City { get; private set; }

	public string PortName => City.name;
	public string RegionName => City.Region.Name;
	public string PortDescription => City.description;

	public float Distance => Vector3.Distance(City.theGameObject.transform.position, Session.playerShip.transform.position);

	private Action<CityViewModel> _OnClick;
	public Action<CityViewModel> OnClick { get => _OnClick; set { _OnClick = value; Notify(); } }

	public Sprite PortIcon => City.PortIcon();
	public Sprite PortCoin => City.PortCoinIcon();

	public string PortPopulationRank {
		get {
			int pop = City.population;
			if (pop >= 0 && pop < 25) return "Population: Village";
			else if (pop >= 25 && pop < 50) return "Population: Town";
			else if (pop >= 50 && pop < 75) return "Population: City";
			else if (pop >= 75 && pop <= 100) return "Population: Metropolis";
			else return "";
		}

	}

	public CityViewModel(Settlement city, Action<CityViewModel> onClick) {
		City = city;
		OnClick = onClick;
	}

	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_Button_TryToLeavePort() {
		//If you aren't low on supplies or you've already been warned
		if (!Globals.Session.playerShipVariables.CheckIfShipLeftPortStarvingOrThirsty()) {
			//if (Session.Trade.CheckIfPlayerCanAffordToPayPortTaxes()) {
			//MGV.controlsLocked = false;
			//Start Our time passage
			Session.playerShipVariables.PassTime(.25f, true);
			Session.justLeftPort = true;
			//Session.playerShipVariables.ship.currency -= GameVars.currentPortTax;

			//Add a new route to the player journey log as a port exit
			Session.playerShipVariables.journey.AddRoute(new PlayerRoute(new Vector3(Session.playerShip.transform.position.x, Session.playerShip.transform.position.y, Session.playerShip.transform.position.z), Vector3.zero, Session.currentSettlement.settlementID, Session.currentSettlement.name, true, Session.playerShipVariables.ship.totalNumOfDaysTraveled), Session.playerShipVariables, Session.CaptainsLog);
			//We should also update the ghost trail with this route otherwise itp roduce an empty 0,0,0 position later
			Session.playerShipVariables.UpdatePlayerGhostRouteLineRenderer(MainState.IS_NOT_NEW_GAME);

			//Turn off the coin image texture
			MainState.menuControlsLock = false;

			Session.showSettlementGUI = false;
			MainState.runningMainGameGUI = true;

			GameVars.MasterGUISystem.ClearViewModels();

			Globals.UI.Hide<PortScreen>();
			Globals.UI.Hide<TownScreen>();
			Globals.UI.Show<Dashboard, DashboardViewModel>(new DashboardViewModel());

			//}
			//else {//Debug.Log ("Not Enough Drachma to Leave the Port!");
			//	Notifications.ShowANotificationMessage("Not Enough Drachma to pay the port tax and leave!");
			//}
		}

	}
}
