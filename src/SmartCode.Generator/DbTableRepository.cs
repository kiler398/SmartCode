﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Generator.Entity;
using SmartSql.Abstractions;

namespace SmartCode.Db
{
    public class DbTableRepository : DbRepository
    {
        private ILogger<DbTableRepository> _logger;
        public String Scope => "Database";

        public DbTableRepository(
            DataSource dataSource
            , ILoggerFactory loggerFactory) : base(dataSource, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DbTableRepository>();
        }
        public IEnumerable<Table> QueryTable()
        {
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName}, QueryTable Start! ----");
            IEnumerable<Table> tables;
            try
            {
                SqlMapper.BeginSession();
                tables = SqlMapper.Query<Table>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "QueryTable",
                    Request = new { DBName = DbName }
                });
                foreach (var table in tables)
                {
                    table.Columns = SqlMapper.Query<Column>(new RequestContext
                    {
                        Scope = Scope,
                        SqlId = "QueryColumn",
                        Request = new { DBName = DbName, TableId = table.Id, TableName = table.Name }
                    });
                }
            }
            finally
            {
                SqlMapper.EndSession();
            }
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName},Tables:{tables.Count()} QueryTable End! ----");
            return tables;
        }
    }
}