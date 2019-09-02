﻿using System.ComponentModel.DataAnnotations;

namespace Forum.Data.Models {
	public class Smiley {
		public int Id { get; set; }
		public int SortOrder { get; set; }

		[Required]
		public string Code { get; set; }

		[Required]
		public string FileName { get; set; }

		[Required]
		public string Path { get; set; }

		public string Thought { get; set; }
	}
}