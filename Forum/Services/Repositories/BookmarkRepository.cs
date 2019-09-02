﻿using Forum.Contracts;
using Forum.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services.Repositories {
	using DataModels = Data.Models;

	public class BookmarkRepository : IRepository<DataModels.Bookmark> {
		public async Task<List<DataModels.Bookmark>> Records() {
			if (_Records is null) {
				var records = await DbContext.Bookmarks.Where(r => r.UserId == UserContext.ApplicationUser.Id).ToListAsync();
				_Records = records.OrderByDescending(item => item.Id).ToList();
			}

			return _Records;
		}
		List<DataModels.Bookmark> _Records;

		ApplicationDbContext DbContext { get; }
		UserContext UserContext { get; }

		public BookmarkRepository(
			ApplicationDbContext dbContext,
			UserContext userContext
		) {
			DbContext = dbContext;
			UserContext = userContext;
		}

		public async Task<bool> IsBookmarked(int topicId) {
			var records = await Records();
			return records.Any(r => r.TopicId == topicId);
		}
	}
}