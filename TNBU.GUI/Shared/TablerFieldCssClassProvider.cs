using Microsoft.AspNetCore.Components.Forms;

namespace TNBU.GUI.Shared {
	public class TablerFieldCssClassProvider : FieldCssClassProvider {
		public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier) {
			if(editContext.GetValidationMessages(fieldIdentifier).Any()) {
				return "is-invalid";
			}
			return "";
		}
	}
}
