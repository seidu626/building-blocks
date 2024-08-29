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
    public sealed class FastPager<T> where T : Entity, new()
    {
        private readonly IQueryable<T> _query;
        private readonly int _pageSize;
        private int? _maxId;
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
            _maxId = null;
            _currentPage = null;
        }

        public int? MaxId => _maxId;

        public int? CurrentPage => _currentPage;

        public bool ReadNextPage(out IList<T> page)
        {
            return ReadNextPage(x => x, x => x.Id, out page);
        }

        public bool ReadNextPage<TShape>(
            Expression<Func<T, TShape>> shaper,
            Func<TShape, int> idSelector,
            out IList<TShape> page)
        {
            Guard.AgainstNull(shaper, nameof(shaper));
            page = null;

            if (!_maxId.HasValue)
            {
                _maxId = int.MaxValue;
                _currentPage = 0;
            }

            if (_maxId.Value <= 1)
                return false;

            page = _query
                .Where(x => x.Id < _maxId.Value)
                .OrderByDescending(x => x.Id)
                .Take(_pageSize)
                .Select(shaper)
                .ToList();

            if (page.Count == 0)
            {
                _maxId = -1;
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
            Func<TShape, int> idSelector)
        {
            Guard.AgainstNull(shaper, nameof(shaper));

            if (!_maxId.HasValue)
            {
                _maxId = int.MaxValue;
                _currentPage = 0;
            }

            if (_maxId.Value <= 1)
                return null;

            var listAsync = await _query
                .Where(x => x.Id < _maxId.Value)
                .OrderByDescending(x => x.Id)
                .Take(_pageSize)
                .Select(shaper)
                .ToListAsync();

            if (listAsync.Count == 0)
            {
                _maxId = -1;
                return null;
            }

            _currentPage = (_currentPage.HasValue ? _currentPage.Value + 1 : 1);
            _maxId = idSelector(listAsync.Last());

            return listAsync;
        }
    }
}
