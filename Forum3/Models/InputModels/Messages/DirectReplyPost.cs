﻿using Forum3.Interfaces.Messages;

namespace Forum3.InputModels.Messages {
	public class DirectReplyPost : IMessageInput
    {
		public int Id { get; set; }
		public string Body { get; set; }
		public string FormAction { get; } = "DirectReply";
	}
}