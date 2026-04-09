using BuildingBlocks.Exceptions;
using BuildingBlocks.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Persistence.Utilities
{
    public sealed class FastPager<T, TId> where T : Entity<TId>, new()
    {
        private readonly IQueryable<T> _query;
        private readonly int _pageSize;
        private TId? _maxId;
        private int? _currentPage;

        public FastPager(IQueryable<T> query, int pageSize = 1000)
        {
            Guard.AgainstNull(query, nameof(query));
            Guard.AgainstNegative(pageSize, nameof(pageSize), "Argument '{0}' cannot be a negative value. Value: '{1}'.");
            _query = query;
            _pageSize = pageSize;
        }

        public void Reset()
        {
            _maxId = default;
            _currentPage = null;
        }

        public TId? MaxId => _maxId;

        public int? CurrentPage => _currentPage;

        public bool ReadNextPage(out IList<T> page)
        {
            return ReadNextPage(x => x, x => x.Id, out page);
        }

        public bool ReadNextPage<TShape>(
            Expression<Func<T, TShape>> shaper,
            Func<TShape, TId> idSelector,
            out IList<TShape> page)
        {
            Guard.AgainstNull(shaper, nameof(shaper));
            page = null;

            if (_maxId == null)
            {
                _maxId = default;
                _currentPage = 0;
            }

            if (_maxId.Equals(default(TId)))
                return false;

            page = _query
                .Where(x => !x.Id.Equals(_maxId))
                .OrderByDescending(x => x.Id)
                .Take(_pageSize)
                .Select(shaper)
                .ToList();

            if (page.Count == 0)
            {
                _maxId = default;
                page = null;
                return false;
            }

            _currentPage = (_currentPage.HasValue ? _currentPage.Value + 1 : 1);
            _maxId = idSelector(page.Last());

            return true;
        }

        public async Task<IList<T>> ReadNextPageAsync()
        {
            return await ReadNextPageAsync(x => x, x => x.Id);
        }

        public async Task<IList<TShape>> ReadNextPageAsync<TShape>(
            Expression<Func<T, TShape>> shaper,
            Func<TShape, TId> idSelector)
        {
            Guard.AgainstNull(shaper, nameof(shaper));

            if (_maxId == null)
            {
                _maxId = default;
                _currentPage = 0;
            }

            if (_maxId.Equals(default(TId)))
                return null;

            var listAsync = await _query
                .Where(x => !x.Id.Equals(_maxId))
                .OrderByDescending(x => x.Id)
                .Take(_pageSize)
                .Select(shaper)
                .ToListAsync();

            if (listAsync.Count == 0)
            {
                _maxId = default;
                return null;
            }

            _currentPage = (_currentPage.HasValue ? _currentPage.Value + 1 : 1);
            _maxId = idSelector(listAsync.Last());

            return listAsync;
        }
    }
}
