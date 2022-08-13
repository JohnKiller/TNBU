using System.ComponentModel;

namespace TNBU.GUI.Services {
	public class MessageService {
		public Func<MessageLevel, string, string, MessageButton, MessageButton?, Task<MessageButton>> OnShow = null!;

		public Task<MessageButton> ShowInfo(string title, string message, MessageButton buttons = MessageButton.Close, MessageButton? primaryButton = null) {
			return OnShow.Invoke(MessageLevel.Info, title, message, buttons, primaryButton);
		}
		public Task<MessageButton> ShowSuccess(string title, string message, MessageButton buttons = MessageButton.Close, MessageButton? primaryButton = null) {
			return OnShow.Invoke(MessageLevel.Success, title, message, buttons, primaryButton);
		}
		public Task<MessageButton> ShowWarning(string title, string message, MessageButton buttons = MessageButton.Close, MessageButton? primaryButton = null) {
			return OnShow.Invoke(MessageLevel.Warning, title, message, buttons, primaryButton);
		}
		public Task<MessageButton> ShowDanger(string title, string message, MessageButton buttons = MessageButton.Close, MessageButton? primaryButton = null) {
			return OnShow.Invoke(MessageLevel.Danger, title, message, buttons, primaryButton);
		}

		public enum MessageLevel {
			Info,
			Success,
			Warning,
			Danger
		}

		[Flags]
		public enum MessageButton {
			[Description("Close")] Close = 1,
			[Description("Ok")] Ok = 2,
			[Description("No")] No = 4,
			[Description("Yes")] Yes = 8,
			YesNo = Yes | No,
		}
	}
}
