

//namespace FinShark.EF.Repositories
//{
//	public class StockRepository : BaseRepository<Stock>, IStockRepository
//	{
//		private readonly ApplicationDBcontext _context;
//		public StockRepository(ApplicationDBcontext context) : base(context)
//		{
//			_context = context;
			
//		}

//		public async Task<List<Stock>> Pagenate(StockQueryObject query, params string[] includes)
//		{
//			int skips = (int)((query.PageNumber - 1) * query.PageSize);

//			IQueryable<Stock> stocks = _context.Stocks;

//			foreach (var include in includes)
//			{
//				stocks = stocks.Include(include);
//			}

//			stocks = stocks.Skip(skips).Take((int)query.PageSize);

//			return await stocks.ToListAsync();
//		}

//		public async Task<List<Stock>> QueryStocksAsync(StockQueryObject query, params string[] includes)
//		{
//			int skips = ((int)query.PageNumber - 1) * (int)query.PageSize;
//			IQueryable<Stock> stocks = _context.Stocks;

//			// Filtering
//			stocks = stocks
//		       .WhereIf(!string.IsNullOrEmpty(query.Industry), s => s.Industry.Contains(query.Industry))
//		       .WhereIf(!string.IsNullOrEmpty(query.Symbol), s => s.Symbol.Contains(query.Symbol))
//		       .WhereIf(!string.IsNullOrEmpty(query.CompanyName), s => s.CompanyName.Contains(query.CompanyName));

//			// Sorting
//			if (!string.IsNullOrWhiteSpace(query.SortBy))
//			{
//				bool asec = query.IsAsecending ?? true;
//				stocks = query.SortBy.ToLower() switch
//				{
//					"id" => asec ? stocks.OrderBy(s => s.Id) : stocks.OrderByDescending(s => s.Id), // Fixed
//					"symbol" => asec ? stocks.OrderBy(s => s.Symbol) : stocks.OrderByDescending(s => s.Symbol),
//					"companyname" => asec ? stocks.OrderBy(s => s.CompanyName) : stocks.OrderByDescending(s => s.CompanyName),
//					"purchase" => asec ? stocks.OrderBy(s => s.Purchase) : stocks.OrderByDescending(s => s.Purchase),
//					"lastdiv" => asec ? stocks.OrderBy(s => s.LastDiv) : stocks.OrderByDescending(s => s.LastDiv),
//					"industry" => asec ? stocks.OrderBy(s => s.Industry) : stocks.OrderByDescending(s => s.Industry),
//					"marketcap" => asec ? stocks.OrderBy(s => s.MarketCap) : stocks.OrderByDescending(s => s.MarketCap),
//					_ => stocks
//				};
//			}

//			// Includes
//			if (includes != null && includes.Any()) // Fixed
//			{
//				foreach (var i in includes)
//				{
//					stocks = stocks.Include(i);
//				}
//			}

//			// Pagination
//			stocks = stocks.Skip(skips).Take((int)query.PageSize);

//			return await stocks.ToListAsync();
//		}

//		public async Task<List<Stock>> SortStock(StockQueryObject query, params string[] includes)
//		{
//			IQueryable<Stock> Stocks = _context.Set<Stock>();
	
//			if (!string.IsNullOrWhiteSpace(query.SortBy))
//			{
//				bool asec = query.IsAsecending ?? true;
//				Stocks = query.SortBy.ToLower() switch
//				{
//					"id" => asec ? Stocks.OrderBy(s => s.Id) : _context.Stocks.OrderByDescending(s => s.Id),
//					"symbol" => asec ? Stocks.OrderBy(s => s.Symbol) : Stocks.OrderByDescending(s => s.Symbol),
//					"companyname" => asec ? Stocks.OrderBy(s => s.CompanyName) : Stocks.OrderByDescending(s => s.CompanyName),
//					"purchase" => asec ? Stocks.OrderBy(s => s.Purchase) : Stocks.OrderByDescending(s => s.Purchase),
//					"lastdiv" => asec ? Stocks.OrderBy(s => s.LastDiv) : Stocks.OrderByDescending(s => s.LastDiv),
//					"industry" => asec ? Stocks.OrderBy(s => s.Industry) : Stocks.OrderByDescending(s => s.Industry),
//					"marketcap" => asec ? Stocks.OrderBy(s => s.MarketCap) : Stocks.OrderByDescending(s => s.MarketCap),
//					_ => Stocks

//				};
//			}
//				if (includes != null && includes.Any())
//				{
//					foreach (var i in includes)
//					{
//						Stocks = Stocks.Include(i);
//					}
//				}
//		return await Stocks.ToListAsync();
//		}

//		public async Task<List<Stock>> StockFilter(StockQueryObject query)
//		{
//			 IQueryable<Stock> stocks = _context.Stocks;
//				 stocks = stocks
//				.WhereIf(!string.IsNullOrEmpty(query.Industry), s => s.Industry.Contains(query.Industry))
//				.WhereIf(!string.IsNullOrEmpty(query.Symbol), s => s.Symbol.Contains(query.Symbol))
//				.WhereIf(!string.IsNullOrEmpty(query.CompanyName), s => s.CompanyName.Contains(query.CompanyName));
			     
//			var result = await stocks.ToListAsync();
//			return result.Any() ? result : null;
//		}
//		public async Task<Stock> UpdateStock(int id, Stock _updatedStockDto)
//		{
//			var oldstock = await _context.Stocks.FindAsync(id);
//			if (oldstock != null)
//			{
//				oldstock.Symbol = _updatedStockDto.Symbol;
//				oldstock.CompanyName = _updatedStockDto.CompanyName;
//				oldstock.Purchase = _updatedStockDto.Purchase;
//				oldstock.MarketCap = _updatedStockDto.MarketCap;
//				oldstock.Industry = _updatedStockDto.Industry;
//				oldstock.LastDiv = _updatedStockDto.LastDiv;
//				await _context.SaveChangesAsync();
//				return oldstock;
//			}
//			else return null;
//		}
		
//	} 
//}

