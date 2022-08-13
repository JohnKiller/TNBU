using Microsoft.AspNetCore.Components;
using TNBU.GUI.Services;
using static TNBU.GUI.Services.MessageService;

namespace TNBU.GUI.Components {
	public partial class MessageModal {
		[Inject] public MessageService MessageService { get; set; } = null!;
		private string Title { get; set; } = "";
		private string Message { get; set; } = "";

		private MessageLevel Level = MessageLevel.Info;
		private MessageButton Buttons = MessageButton.Close;
		private MessageButton? PrimaryButton;

		private TaskCompletionSource<MessageButton>? Result;

		private Modal? Ref { get; set; }

		private string Color {
			get {
				return Level switch {
					MessageLevel.Info => "info",
					MessageLevel.Success => "success",
					MessageLevel.Warning => "warning",
					MessageLevel.Danger => "danger",
					_ => "",
				};
			}
		}
		private string Icon {
			get {
				return Level switch {
					MessageLevel.Info => "info-circle",
					MessageLevel.Success => "circle-check",
					MessageLevel.Warning => "alert-octagon",
					MessageLevel.Danger => "alert-triangle",
					_ => "",
				};
			}
		}

		protected override void OnInitialized() {
			MessageService.OnShow = ShowMessage;
		}

		public Task<MessageButton> ShowMessage(MessageLevel level, string title, string message, MessageButton buttons, MessageButton? primaryButton = null) {
			Level = level;
			Title = title;
			Message = message;
			Buttons = buttons;
			PrimaryButton = primaryButton;

			if(Result != null) {
				Result.SetCanceled();
			}
			Result = new();

			Ref!.Open();

			return Result.Task;
		}

		public void OnClick(MessageButton button) {
			Ref!.Close();
			var res = Result!;
			Result = null;
			res.SetResult(button);
		}
	}
}
