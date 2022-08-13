using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace TNBU.GUI.Shared {
	public partial class NavMenuItem : ComponentBase, IDisposable {
		[Parameter] public RenderFragment? ListNavMenuItem { get; set; }
		[Parameter] public string Link { get; set; } = "";
		[Parameter] public string Icon { get; set; } = "";
		[Parameter] public string Name { get; set; } = "";
		[CascadingParameter(Name = "IsSubItem")] public bool IsSubItem { get; set; }

		private bool Expanded = false;
		private bool IsActive { get; set; }
		private string? hrefAbsolute;

		private void ToggleExpander() {
			Expanded = !Expanded;
		}

		[Inject] private NavigationManager NavigationManager { get; set; } = default!;

		protected override void OnInitialized() {
			NavigationManager.LocationChanged += OnLocationChanged;
		}

		protected override void OnParametersSet() {
			hrefAbsolute = Link == null ? null : NavigationManager.ToAbsoluteUri(Link).AbsoluteUri;
			IsActive = ShouldMatch(NavigationManager.Uri);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			NavigationManager.LocationChanged -= OnLocationChanged;
		}

		private void OnLocationChanged(object? sender, LocationChangedEventArgs args) {
			var shouldBeActiveNow = ShouldMatch(args.Location);
			if(shouldBeActiveNow != IsActive || Expanded) {
				Expanded = false;
				IsActive = shouldBeActiveNow;
				StateHasChanged();
			}
		}

		private bool ShouldMatch(string currentUriAbsolute) {
			if(hrefAbsolute == null) {
				return false;
			}

			if(string.Equals(currentUriAbsolute, hrefAbsolute, StringComparison.OrdinalIgnoreCase)) {
				return true;
			}

			if(Link == "/") {
				return false;
			}

			if(currentUriAbsolute.Length == hrefAbsolute.Length - 1) {
				if(hrefAbsolute[hrefAbsolute.Length - 1] == '/'
					&& hrefAbsolute.StartsWith(currentUriAbsolute, StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}

			var prefixLength = hrefAbsolute.Length;
			if(currentUriAbsolute.Length > prefixLength) {
				return currentUriAbsolute.StartsWith(hrefAbsolute, StringComparison.OrdinalIgnoreCase)
					&& (
						prefixLength == 0
						|| !char.IsLetterOrDigit(hrefAbsolute[prefixLength - 1])
						|| !char.IsLetterOrDigit(currentUriAbsolute[prefixLength])
					);
			}

			return false;
		}
	}
}
