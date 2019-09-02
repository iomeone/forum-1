﻿using Microsoft.AspNetCore.Identity;
using System;

namespace Forum.Data.Models {
	public class ApplicationRole : IdentityRole {
		public string Description { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedById { get; set; }
		public DateTime ModifiedDate { get; set; }
		public string ModifiedById { get; set; }
	}
}