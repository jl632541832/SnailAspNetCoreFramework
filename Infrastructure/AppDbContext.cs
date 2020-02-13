﻿using ApplicationCore;
using ApplicationCore.Entity;
using Autofac;
using DotNetCore.CAP;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Snail.Permission.Entity;
using System;
using System.Linq;
using System.Reflection;

namespace Infrastructure
{
    public partial class AppDbContext : PermissionDatabaseContext
    {
        private static bool _hasUpdateDatabase = false;
        private ICapPublisher _publisher;
        public AppDbContext(DbContextOptions<AppDbContext> options, ICapPublisher publisher)
            : base(options)
        {
            _publisher = publisher;
            ensureMigration();
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
        {
            ensureMigration();
        }

        private void ensureMigration()
        {
            if (!_hasUpdateDatabase)
            {
                //数据库的migration和hangfire的创表要运行两次程序后才能完成，todo 修复
                _hasUpdateDatabase = true;
                try
                {
                    Database.Migrate();
                }
                catch 
                {
                }
            }
        }

        public virtual DbSet<SampleEntity> SampleEntity { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 自动应用所有的IEntityTypeConfiguration配置
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override int SaveChanges()
        {
            //统一在数据库上下文的操作前，触发缓存实体的数据清空。
            if (_publisher!=null)
            {
                this.ChangeTracker.Entries().Where(a => Attribute.IsDefined(a.Entity.GetType(), typeof(EnableEntityCacheAttribute))).Select(a => a.Entity.GetType().Name).Distinct().ToList().ForEach(entityName =>
                {
                    _publisher.Publish(EventConstant.EntityChange, new EntityChangeEvent{EntityName=entityName });
                });
            }
           
            return base.SaveChanges();
        }
    }
}
