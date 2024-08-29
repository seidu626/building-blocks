using BuildingBlocks.Exceptions;
using BuildingBlocks.SeedWork;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace BuildingBlocks.Persistence
{
    public abstract class ObjectContextBase<TDbContext> : DbContext where TDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMediator _mediator;
        private static int? _commandTimeoutInSeconds = new int?(30); // Assuming a default of 30 seconds.

        protected ObjectContextBase(DbContextOptions<TDbContext> options,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment,
            IMediator mediator) : base(options)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            SetupCommandTimeout();
        }

        protected ObjectContextBase(DbContextOptions<TDbContext> options) : base(options)
        {
            SetupCommandTimeout();
        }

        private void SetupCommandTimeout()
        {
            if (_commandTimeoutInSeconds.HasValue && _commandTimeoutInSeconds.Value > 0)
            {
                Database.SetCommandTimeout(_commandTimeoutInSeconds);
            }
        }

        public static string GetConnectionString()
        {
            if (!DataSettingsManager.Current.IsValid)
            {
                throw Errors.Application("A connection string could not be resolved for the parameterless constructor of the derived DbContext. Either the database is not installed, or the file 'Settings.txt' does not exist or contains invalid content.");
            }

            return DataSettingsManager.Current.ConnectionString;
        }

        public void ChangeState<TEntity>(TEntity entity, EntityState requestedState) where TEntity : Entity
        {
            EntityEntry<TEntity> entityEntry = Entry(entity);
            if (entityEntry.State != (Microsoft.EntityFrameworkCore.EntityState)requestedState)
            {
                entityEntry.State = (Microsoft.EntityFrameworkCore.EntityState)requestedState;
            }
        }
    }
}
