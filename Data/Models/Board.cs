﻿namespace Forum.Data.Models {
	public class Board {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int DisplayOrder { get; set; }
		public int CategoryId { get; set; }
	}
}